using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    public class Utils
    {
        public static List<Tuple<int, int>> BresenhamCircle(int centerX, int centerY, int radius)
        {
            var edges = new List<Tuple<int, int>>();

            var d = (5 - radius * 4) / 8;
            var x = 0;
            var y = radius;

            do
            {
                /*if (centerX + x >= 0 && centerX + x <= maxWidth - 1 && centerY + y >= 0 && centerY + y <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX + x, centerY + y));
                /*if (centerX + x >= 0 && centerX + x <= maxWidth - 1 && centerY - y >= 0 && centerY - y <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX + x, centerY - y));
                /*if (centerX - x >= 0 && centerX - x <= maxWidth - 1 && centerY + y >= 0 && centerY + y <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX - x, centerY + y));
                /*if (centerX - x >= 0 && centerX - x <= maxWidth - 1 && centerY - y >= 0 && centerY - y <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX - x, centerY - y));
                /*if (centerX + y >= 0 && centerX + y <= maxWidth - 1 && centerY + x >= 0 && centerY + x <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX + y, centerY + x));
                /*if (centerX + y >= 0 && centerX + y <= maxWidth - 1 && centerY - x >= 0 && centerY - x <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX + y, centerY - x));
                /*if (centerX - y >= 0 && centerX - y <= maxWidth - 1 && centerY + x >= 0 && centerY + x <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX - y, centerY + x));
                /*if (centerX - y >= 0 && centerX - y <= maxWidth - 1 && centerY - x >= 0 && centerY - x <= maxHeight - 1)*/ edges.Add(new Tuple<int, int>(centerX - y, centerY - x));

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);

            return edges;
        }

        public delegate bool VisitBresenhamCallback(int x, int y);
        public static void VisitBresenhamLine(int x0, int y0, int x1, int y1, VisitBresenhamCallback callback)
        {
            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);

            var sx = x0 < x1 ? 1 : -1;
            var sy = y0 < y1 ? 1 : -1;

            var err = (dx > dy ? dx : -dy) / 2;
            int err2;

            while(true)
            {
                if(callback(x0, y0))
                {
                    return;
                }

                if (x0 == x1 && y0 == y1)
                {
                    return;
                }

                err2 = err;
                if (err2 > -dx)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (err2 < dy)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}
