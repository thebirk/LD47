using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    public class Tile
    {
        public static readonly Dictionary<char, Tile> Tiles = new Dictionary<char, Tile>
        {
            {' ', new Tile{
                Name = "Nothing",
                Character = ' ',
                Solid = false,
            }},
            {'#', new Tile{
                Name = "Wall",
                Character = '#',
            }},
            {'¤', new Tile{
                Name = "Barrel",
                Character = '¤',
            }},
            {'H', new Tile{
                Name = "Ladder",
                Character = 'H',
                Solid = false,
            }},
        };

        public char Character { get; set; }
        public ConsoleColor Foreground { get; set; } = ConsoleColor.White;
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;
        public bool Solid { get; set; } = true;
        public string Name { get; set; }
    }
}
