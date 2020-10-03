using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace LD47
{
    public class RoomProto
    {
        public static readonly RoomProto Town = new RoomProto("levels/town.txt");
        public static readonly RoomProto StartRoom = new RoomProto("levels/rooms/start.txt");
        public static readonly RoomProto JailRoom = new RoomProto("levels/rooms/jailroom.txt");

        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Name { get; private set; }
        public List<string> Rows { get; private set; }

        public RoomProto(string path)
        {
            StreamReader reader = new StreamReader(path);
            Name = reader.ReadLine();

            Rows = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                
                if (line == string.Empty)
                {
                    break;
                }
                Rows.Add(line);
            }

            Debug.Trace($"Rows: {Rows.Count}");

            Height = Rows.Count;
            Width = Rows[0].Length;

            var length = Rows[0].Length;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (row.Length != length)
                {
                    throw new Exception($"Uneven line length. At row {i} in {path}");
                }
            }
        }

        public override string ToString()
        {
            return $"{Name}: {Width}x{Height}";
        }
    }
}
