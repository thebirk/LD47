using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LD47
{
    public class ItemStack
    {
        public Item Item { get; set; }
        public int Count { get; set; }
    }

    public class Inventory
    {
        public List<ItemStack> Items { get; private set; }

        public Inventory()
        {
            Items = new List<ItemStack>();
        }

        public void Add(Item item, int count = 1)
        {
            var entry = Items.SingleOrDefault(x => x.Item == item);
            if(entry != null)
            {
                entry.Count += count;
            }
            
            Items.Add(new ItemStack
            {
                Item = item,
                Count = count,
            });
        }

        public void Remove(Item item)
        {
            var entry = Items.SingleOrDefault(x => x.Item == item);
            if (entry != null)
            {
                entry.Count -= 1;
                if (entry.Count == 0)
                {
                    Items.Remove(entry);
                }
            }
        }

        public bool Has(Item item)
        {
            return Items.Any(x => x.Item == item);
        }
    }
}
