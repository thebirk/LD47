using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LD47
{
    enum PlayerFovKnowledge
    {
        Unexplored,
        Remember,
        CanSee,
    }

    public class Room
    {
        public readonly RoomProto Proto;
        private readonly List<Actor> actors;
        private readonly Tile[] tiles;
        private readonly PlayerFovKnowledge[] playerFov;
        private readonly PlayerFovKnowledge[] prevPlayerFov;
        private readonly Dictionary<Actor, Tuple<int, int>> playerMemory;

        public Room(RoomProto proto)
        {
            Proto = proto;
            actors = new List<Actor>();

            playerMemory = new Dictionary<Actor, Tuple<int, int>>();
            playerFov = new PlayerFovKnowledge[Proto.Width * Proto.Height];
            prevPlayerFov = new PlayerFovKnowledge[Proto.Width * Proto.Height];
            tiles = new Tile[Proto.Width * Proto.Height];
            for (int y = 0; y < Proto.Height; y++)
            {
                var row = Proto.Rows[y];
                for (int x = 0; x < Proto.Width; x++)
                {
                    var tile = Tile.Tiles.GetValueOrDefault(row[x]);

                    if (tile == null)
                    {
                        throw new Exception($"Unknown character in room proto: '{row[x]}'");
                    }
                    tiles[x + y * Proto.Width] = tile;
                }
            }
        }

        public void Add(Actor actor)
        {
            actor.Room = this;
            actors.Add(actor);
        }

        public void Remove(Actor actor)
        {
            actors.Remove(actor);
            actor.Room = null;
        }

        public void Tick()
        {
            foreach (var actor in actors)
            {
                if (!actor.Alive) continue;

                //TODO: energy stuff
                actor.Think();
            }

            actors.RemoveAll(x => !x.Alive);
        }

        private void CalculatePlayerFOV()
        {
            Array.Copy(playerFov, prevPlayerFov, playerFov.Length);
            for(int i = 0; i < playerFov.Length; i++)
            {
                var prev = prevPlayerFov[i];
                if(prev == PlayerFovKnowledge.CanSee || prev == PlayerFovKnowledge.Remember)
                {
                    playerFov[i] = PlayerFovKnowledge.Remember;
                }
                else
                {
                    playerFov[i] = PlayerFovKnowledge.Unexplored;
                }
            }

            //wtf? just have the player be global 4head
            var player = actors.Single(x => x.GetType() == typeof(Player));

            playerFov[(player.X) + (player.Y) * Proto.Width] = PlayerFovKnowledge.CanSee;

            var playerSight = 250;
            var edges = Utils.BresenhamCircle(player.X, player.Y, playerSight);

            foreach (var edge in edges)
            {
                Utils.VisitBresenhamLine(player.X, player.Y, edge.Item1, edge.Item2, (x, y) =>
                {
                    if (x < 0 || x >= Proto.Width || y < 0 || y >= Proto.Height) return true;

                    var tile = tiles[x + y * Proto.Width];
                    playerFov[x + y * Proto.Width] = PlayerFovKnowledge.CanSee;

                    if(tile.Solid)
                    {
                        return true;
                    }
                    return false;
                });
            }
        }

        public void Draw()
        {
            var sw = new Stopwatch();
            sw.Start();
            CalculatePlayerFOV();
            sw.Stop();
            Debug.Trace($"CalculatePlayerFOV took {sw.Elapsed.TotalMilliseconds}ms");

            int xOffset = Display.Width / 2 - Proto.Width / 2;
            int yOffset = Display.Height / 2 - Proto.Height / 2;

            for (int y = 0; y < Proto.Height; y++)
            {
                for (int x = 0; x < Proto.Width; x++)
                {
                    var tile = tiles[x + y * Proto.Width];
                    var fov = playerFov[x + y * Proto.Width];
                    if (fov == PlayerFovKnowledge.CanSee)
                    {
                        Display.Put(xOffset + x, yOffset + y, tile.Character, tile.Foreground, tile.Background);
                    }
                    else if(fov == PlayerFovKnowledge.Remember)
                    {
                        Display.Put(xOffset + x, yOffset + y, tile.Character, ConsoleColor.Gray, tile.Background);
                    }
                    else
                    {
                        Display.Put(xOffset + x, yOffset + y, '░', ConsoleColor.White, ConsoleColor.Black);
                    }
                }
            }

            foreach (var actor in actors)
            {
                var fov = playerFov[actor.X + actor.Y * Proto.Width];
                var tile = tiles[actor.X + actor.Y * Proto.Width];

                if (fov == PlayerFovKnowledge.CanSee)
                {
                    Display.Put(xOffset + actor.X, yOffset + actor.Y, actor.Character, actor.ForegroundColor, actor.BackgroundColor ?? tile.Background);
                    playerMemory[actor] = new Tuple<int, int>(actor.X, actor.Y);
                }
                else
                {
                    var memory = playerMemory.GetValueOrDefault(actor);
                    if (memory != null)
                    {
                        if (fov == PlayerFovKnowledge.CanSee)
                        {
                            playerMemory.Remove(actor);
                        }
                        else
                        {
                            if(actor.CanForget)
                            {
                                Display.Put(xOffset + memory.Item1, yOffset + memory.Item2, '?', ConsoleColor.Gray, actor.BackgroundColor ?? tile.Background);
                            }
                            else
                            {
                                Display.Put(xOffset + actor.X, yOffset + actor.Y, actor.Character, actor.ForegroundColor, actor.BackgroundColor ?? tile.Background);
                            }
                        }
                    }
                }
            }

            var player = GetPlayer();
            var playerTile = GetTile(player.X, player.Y);
            Display.Put(xOffset + player.X, yOffset + player.Y, player.Character, player.ForegroundColor, player.BackgroundColor ?? playerTile.Background);
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= Proto.Width || y < 0 || y >= Proto.Height)
            {
                return null;
            }

            return tiles[x + y * Proto.Width];
        }

        public Actor GetActorAt(int x, int y)
        {
            if (x < 0 || x >= Proto.Width || y < 0 || y >= Proto.Height)
            {
                return null;
            }

            foreach (var actor in actors)
            {
                if (actor.X == x && actor.Y == y)
                {
                    return actor;
                }
            }

            return null;
        }

        public Player GetPlayer()
        {
            return (Player) actors.Single((x) => x.GetType() == typeof(Player));
        }
    }
}
