using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    public enum Faction
    {
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

        public Room Room { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public char Character { get; set; }
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public bool Alive { get { return HP > 0; } }

        public bool IsPlayer { get; protected set; }

        public Actor() { }

        public int Hit(int dmg)
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
            Room.Remove(this);
        }

        public void MeleeAttack(Actor other)
        {
            var dmg = Strength;
            other.Hit(dmg);

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

        public void Move(int dx, int dy)
        {
            var dstX = X + dx;
            var dstY = Y + dy;

            var tile = Room.GetTile(dstX, dstY);
            if (tile == null)
            {
                return;
            }

            if (tile.Solid)
            {
                if (IsPlayer)
                {
                    Display.BeepAsync(300, 100);
                }
                return;
            }

            var actor = Room.GetActorAt(dstX, dstY);
            if (actor != null)
            {
                MeleeAttack(actor);

                if (!actor.Alive)
                {
                    X += dx;
                    Y += dy;
                }
            }
            else
            {
                X += dx;
                Y += dy;
            }

            return;
        }
    }
}
