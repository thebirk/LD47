using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    public class Door : Actor
    {
        public Item Key { get; set; }

        public Door(Item key = null)
        {
            Name = "Door";
            HP = 1; // we need to be Actor.Alive
            Solid = true;
            Key = key;
            Character = '-';
            ForegroundColor = ConsoleColor.DarkYellow;
            Faction = Faction.Thing;
            CanForget = false;
        }

        public override int OnHit(Actor other, int dmg)
        {
            Debug.Trace("Door: ouch");

            if (other.CanOperateDoors && Solid && (Key == null || other.Inventory.Has(Key)))
            {
                Solid = false;
                Character = '.';
            }
            else
            {
                if (other.IsPlayer)
                {
                    
                }
            }

            return 0;
        }

        public override void Action(Actor other)
        {
            if (other.CanOperateDoors && (Key == null || other.Inventory.Has(Key)))
            {
                Solid = !Solid;
                Character = Solid ? '-' : '.';
            }
        }

        public override void Think()
        {
            // doors dont think
        }
    }
}
