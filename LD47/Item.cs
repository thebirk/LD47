using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    public class Item
    {
        public static readonly Item JailKey = new Item("Jail Key");
        public static readonly Item OldBaton = new Item("Old Baton")
        {
            WeaponBonus = 2
        };

        public string Name { get; private set; }
        public int WeaponBonus { get; private set; }
        public int ShieldBonus { get; private set; }

        public Item(string name, bool stackable = true)
        {
            Name = name;
        }

        public virtual void Consume(Actor actor) { }
    }
}
