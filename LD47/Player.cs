using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    public class Player : Actor
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int XpToLevelUp { get; set; }
        public bool PlayerShouldLevelUp { get; set; }

        public Player(string name)
        {
            Name = name;
            HP = MaxHP = 10;
            Strength = MaxStrength = 1;
            Level = 1;
            Experience = 0;
            XpToLevelUp = 20;
            Faction = Faction.Player;
            CanOperateDoors = true;

            Character = '@';
            ForegroundColor = ConsoleColor.Blue;

            IsPlayer = true;
        }

        public void AddXp(int xp)
        {
            Experience += xp;
            if (Experience >= XpToLevelUp)
            {
                XpToLevelUp = (int) ((float)XpToLevelUp * 1.25);
                Level += 1;
                PlayerShouldLevelUp = true;
            }
        }

        public override void Die()
        {
            // Dont call base.Die as we want to keep our Room reference for the game over screen
            //base.Die();
            throw new EndGameException(this);
        }

        public override void Think()
        {
            
        }

        public void DoAction()
        {
            var neighbours = GetNeighbours();
            foreach (var n in neighbours)
            {
                n.Action(this);
            }
        }
    }
}
