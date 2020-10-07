using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LD47
{
    public class Glyph
    {
        public ConsoleColor Foreground { get; set; } = ConsoleColor.White;
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;
        public char Character { get; set; } = ' ';

        public Glyph() { }

        public Glyph(Glyph other)
        {
            Character = other.Character;
            Foreground = other.Foreground;
            Background = other.Background;
        }

        public bool Equals(char character, ConsoleColor foreground, ConsoleColor background)
        {
            return Character == character &&
                Foreground == foreground &&
                Background == background;
        }

        public override bool Equals(object obj)
        {
            return obj is Glyph glyph &&
                   Foreground == glyph.Foreground &&
                   Background == glyph.Background &&
                   Character == glyph.Character;
        }

        public override string ToString()
        {
            return $"'{Character}', {Foreground}, {Background}";
        }
    }

    public static class Display
    {
        static Glyph[] glyphs;
        static Glyph[] prevGlyphs;

        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static bool DebugCache { get; set; } = false;

        public static void Init(int width, int height)
        {
            Width = width;
            Height = height;

            glyphs = new Glyph[Width * Height];
            prevGlyphs = new Glyph[Width * Height];
            Clear();

            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);
            Console.Clear();
            Console.CursorVisible = false;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "LD47";

            Draw(true);
        }

        public static void Clear(bool withRedraw = false, bool clearPrev = false)
        {
            for (int i = 0; i < glyphs.Length; i++)
            {
                glyphs[i] = new Glyph { };
                if(clearPrev) prevGlyphs[i] = new Glyph { Character = '\x00' };
            }

            if (withRedraw)
            {
                Draw();
            }
        }

        public static void ClearArea(int xStart, int yStart, int width, int height, bool withRedraw = false, bool clearPrev = false)
        {
            for (int y = yStart; y < yStart + height; y++)
            {
                for (int x = xStart; x < xStart + width; x++)
                {
                    glyphs[x + y * Width].Character = ' ';
                    if(clearPrev) prevGlyphs[x + y * Width].Character = '\x00';
                }
            }

            if (withRedraw)
            {
                Draw();
            }
        }

        public static void Draw(bool forceRedraw = false)
        {
            var count = 0;

            if (Console.WindowWidth != Width || Console.WindowHeight != Height)
            {
                Console.Clear();
                Console.SetWindowSize(Width, Height);
                Console.SetBufferSize(Width, Height);

                forceRedraw = true;
            }

            var rand = new Random();
            var bg = (ConsoleColor)rand.Next(0, 8);
            ConsoleColor? activeForeground = null, activeBackground = null;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var glyph = glyphs[x + y * Width];
                    var prev = prevGlyphs[x + y * Width];

                    if (!glyph.Equals(prev) || forceRedraw)
                    {
                        Console.SetCursorPosition(x, y);
                        if (activeForeground != glyph.Foreground) Console.ForegroundColor = glyph.Foreground;
                        //if (activeBackground != glyph.Foreground)
                        {
                            if (DebugCache)
                            {
                                Console.BackgroundColor = bg;
                            }
                            else
                            {
                                Console.BackgroundColor = glyph.Background;
                            }
                        }

                        Console.Write(glyph.Character);
                        count++;

                        activeForeground = glyph.Foreground;
                        activeBackground = glyph.Background;
                    }
                }
            }

            Console.CursorVisible = false;

            for (int i = 0; i < glyphs.Length; i++)
            {
                prevGlyphs[i] = new Glyph(glyphs[i]);
            }

            Debug.Trace($"Updated {count} glyphs during draw");
        }

        public static void Put(int x, int y, char character, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            var equal = false;
            var g = glyphs[x + y * Width];
            if (g.Character == character &&
                g.Foreground == foreground &&
                g.Background == background)
            {
                equal = true;
            }

            glyphs[x + y * Width].Character = character;
            glyphs[x + y * Width].Foreground = foreground;
            glyphs[x + y * Width].Background = background;
        }

        public static void WriteLine(string text, int x, int y, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            var xOffset = x;
            for (int i = 0; i < text.Length; i++, xOffset++)
            {
                if (xOffset < 0 || xOffset >= Width || y < 0 || y >= Height)
                {
                    return;
                }

                var ch = text[i];
                Put(xOffset, y, ch, foreground, background);
            }
        }

        public static void BeepAsync(int freq = 800, int ms = 200)
        {
            Task.Run(() => Console.Beep(freq, ms));
        }

        public static ConsoleKeyInfo ReadKey()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            return Console.ReadKey(true);
        }
    }
}
