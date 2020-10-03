using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace LD47
{
    public enum Faction
    {
        Thing,
        Player,
        Alien,
    }

    public enum MoveResultType
    {
        CollisionWall,
        CollisionActor,
        Moved,
    }

    public class MoveResult
    {
        public MoveResultType Type { get; set; }
        public Actor Actor { get; set; }

        public MoveResult(MoveResultType type)
        {
            Type = type;
        }
    }

    public class Actor
    {
        public string Name { get; set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int MaxStrength { get; set; }
        public int Strength { get; set; }
        public int MaxStamina { get; set; }
        public int Stamina { get; set; }
        public int XpReward { get; set; }
        public Faction Faction { get; set; } = Faction.Alien;
        public bool Solid { get; set; } = true;
        public bool CanOperateDoors { get; set; } = false;
        public bool CanForget { get; set; } = true;

        public Room Room { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public char Character { get; set; }
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor? BackgroundColor { get; set; } = null;

        public bool Alive { get { return HP > 0; } }

        public bool IsPlayer { get; protected set; }

        public Actor() { }

        public virtual int OnHit(Actor other, int dmg)
        {
            // shield stuff

            HP -= dmg;
            if (HP <= 0)
            {
                Die();
            }

            return dmg;
        }

        public virtual void Die()
        {
            // do something I guess?
        }

        public virtual void Action(Actor other)
        {

        }

        public void MeleeAttack(Actor other)
        {
            //TODO: Check faction
            if (Faction == other.Faction) return; // No friendly fire

            var dmg = Strength;
            other.OnHit(this, dmg);

            if (!other.Alive)
            {
                Debug.Trace($"{Name} punched {other.Name} to death.");

                if (IsPlayer)
                {
                    var p = (Player)this;
                    p.AddXp(other.XpReward);
                }
            }
        }

        public bool Move(int dx, int dy)
        {
            var dstX = X + dx;
            var dstY = Y + dy;

            var tile = Room.GetTile(dstX, dstY);
            if (tile == null)
            {
                return false;
            }

            if (tile.Solid)
            {
                if (IsPlayer)
                {
                    //Display.BeepAsync(300, 100);
                }
                return false;
            }

            var actor = Room.GetActorAt(dstX, dstY);
            if (actor != null && actor.Solid)
            {
                MeleeAttack(actor);

                if (!actor.Alive)
                {
                    X += dx;
                    Y += dy;
                    return true;
                }

                return false;
            }
            else
            {
                X += dx;
                Y += dy;
            }

            return true;
        }

        public List<Actor> GetNeighbours()
        {
            Actor GetNeighbour(int x, int y)
            {
                if (X+x < 0 || X+x >= Room.Proto.Width || Y+y < 0 || Y+y >= Room.Proto.Height) return null;

                return Room.GetActorAt(X + x, Y + y);
            }

            var neighbours = new List<Actor>();
            neighbours.Add(GetNeighbour(-1, -1));
            neighbours.Add(GetNeighbour(0, -1));
            neighbours.Add(GetNeighbour(1, -1));
            neighbours.Add(GetNeighbour(1, 0));
            neighbours.Add(GetNeighbour(-1, 0));
            neighbours.Add(GetNeighbour(-1, 1));
            neighbours.Add(GetNeighbour(0, 1));
            neighbours.Add(GetNeighbour(1, 1));
            neighbours.RemoveAll((x) => x == null);

            return neighbours;
        }

        bool HasDest = false;
        Tuple<int, int> Dest;
        List<Tuple<int, int>> Path;

        public virtual void Think()
        {
            if (!HasDest)
            {
                var player = Room.GetPlayer();
                Dest = new Tuple<int, int>(player.X, player.Y);
                var sw = new Stopwatch();
                sw.Start();
                Path = AStarPath(X, Y, player.X, player.Y);
                sw.Stop();
                Debug.Trace($"A* took: {sw.Elapsed.TotalMilliseconds}ms");
                if (Path != null)
                {
                    HasDest = true;
                    Debug.Trace("Found path?");
                    Debug.Trace(string.Join(", ", Path));
                }
                else
                {
                    Debug.Trace("no path?");
                }
            }
            else
            {
                if (X == Dest.Item1 && Y == Dest.Item2)
                {
                    HasDest = false;
                    return;
                }

                var next = Path[0];
                Path.RemoveAt(0);

                var dx = next.Item1 - X > 0 ? 1 : next.Item1 - X < 0 ? -1 : 0;
                var dy = next.Item2 - Y > 0 ? 1 : next.Item2 - Y < 0 ? -1 : 0;

                if (!Move(dx, dy))
                {
                    HasDest = false;
                }
            }
        }

        class AStarNode
        {
            public Tuple<int, int> Pos { get; set; }
            public AStarNode Prev { get; set; }
            public float GCost { get; set; }
            public float FCost { get; set; }
            public float Cost { get; set; }
        }

        public List<Tuple<int, int>> AStarPath(int x0, int y0, int x1, int y1)
        {
            var end = new Tuple<int, int>(x1, y1);
            var open = new List<AStarNode>{ new AStarNode{
                Pos = new Tuple<int, int>(x0, y0),
                Prev = null,
                GCost = 0,
                FCost = 0,
            }};
            var closed = new List<AStarNode>();

            AStarNode GetNeighbour(AStarNode current, int dx, int dy)
            {
                int x = current.Pos.Item1 + dx;
                int y = current.Pos.Item2 + dy;
                if (x < 0 || x >= Room.Proto.Width || y < 0 || y >= Room.Proto.Height) return null;

                var diag = dx == 0 || dy == 0;
                var cost = diag ? MathF.Sqrt(1) : 1;

                if (Room.GetTile(x, y).Solid)
                {
                    return null;
                }

                if(Room.GetActorAt(x, y) != null)
                {
                    cost += 2;
                }

                return new AStarNode
                {
                    Pos = new Tuple<int, int>(x, y),
                    Prev = current,
                    Cost = cost
                };
            }

            while (open.Count > 0)
            {
                AStarNode current = open[0];
                foreach (var n in open)
                {
                    if (n.FCost < current.FCost) current = n;
                }
                open.Remove(current);
                closed.Add(current);

                if (current.Pos.Item1 == end.Item1 && current.Pos.Item2 == end.Item2)
                {
                    var path = new List<Tuple<int, int>>();
                    while (current.Prev != null)
                    {
                        path.Add(current.Pos);
                        current = current.Prev;
                    }
                    path.Reverse();
                    return path;
                }

                var children = new List<AStarNode>();
                children.Add(GetNeighbour(current, -1, -1));
                children.Add(GetNeighbour(current, 0, -1));
                children.Add(GetNeighbour(current, 1, -1));

                children.Add(GetNeighbour(current, 1, 0));
                children.Add(GetNeighbour(current, -1, 0));

                children.Add(GetNeighbour(current, -1, 1));
                children.Add(GetNeighbour(current, 0, 1));
                children.Add(GetNeighbour(current, 1, 1));

                children.RemoveAll((x) => x == null);

                foreach (var child in children)
                {
                    if (closed.SingleOrDefault(x => x.Pos.Item1 == child.Pos.Item1 && x.Pos.Item2 == child.Pos.Item2) != null)
                    {
                        continue;
                    }


                    child.GCost = current.GCost + child.Cost;
                    var dx = Math.Abs(x1 - child.Pos.Item1);
                    var dy = Math.Abs(y1 - child.Pos.Item2);
                    var h = MathF.Sqrt(dx * dx + dy * dy);
                    child.FCost = child.GCost + h;

                    var exists = open.SingleOrDefault(x => x.Pos.Item1 == child.Pos.Item1 && x.Pos.Item2 == child.Pos.Item2);
                    if (exists != null)
                    {
                        if (child.GCost < exists.GCost)
                        {
                            exists.GCost = child.GCost;
                            exists.FCost = child.FCost;
                            exists.Prev = child.Prev;
                        }
                        continue;
                    }

                    open.Add(child);
                }
            }

            return null;
        }
    }
}
