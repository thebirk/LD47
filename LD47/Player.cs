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
            HP = 10;
            Strength = 5;
            Level = 1;
            Experience = 0;
            XpToLevelUp = 20;

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
            base.Die();
            throw new EndGameException(this);
        }
    }
}
