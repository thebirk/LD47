using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LD47
{
    public class LogMessage
    {
        public string Text { get; set; }
        public ConsoleColor Foreground { get; set; } = ConsoleColor.White;

        public LogMessage(string text, ConsoleColor foreground = ConsoleColor.White)
        {
            Text = text;
            Foreground = foreground;
        }
    }

    public class Game
    {
        private readonly IntroResult introResult;
        public static Game Instance { get; private set; }
        public List<List<LogMessage>> LogMessages { get; set; } = new List<List<LogMessage>>();
        public Player Player { get; set; }

        public Game(IntroResult introResult)
        {
            this.introResult = introResult;
            Instance = this;
        }

        public void DrawStatus()
        {
            Display.ClearArea(0, Display.Height - 10, Display.Width, 10);

            var yOffset = Display.Height - 10;
            Display.WriteLine($"Name: {Player.Name}", 0, yOffset + 0);
            Display.WriteLine($" HP: {Player.HP,2}/{Player.MaxHP,2}", 0, yOffset + 1);
            Display.WriteLine($"STR: {Player.Strength,2}/{Player.MaxStrength,2}", 0, yOffset + 2);

            if (LogMessages.Count >= 10)
            {
                var toRemove = LogMessages.Count - 10;
                LogMessages.RemoveRange(0, toRemove);
            }

            Debug.Trace($"LogMessages.Count: {LogMessages.Count}");

            int y = 0;
            for (int i = LogMessages.Count - 1; i >= 0; i--)
            {
                var msg = LogMessages[i];
                var x = 40;
                foreach (var comp in msg)
                {
                    Display.WriteLine(comp.Text, x, yOffset + y, comp.Foreground);
                    x += comp.Text.Length;
                }

                y++;
                if (y >= 10) break;
            }
        }

        public void Log(List<LogMessage> msg)
        {
            LogMessages.Add(msg);
            DrawStatus();
        }

        public void Run()
        {
            var room = new Room(RoomProto.Town);
            //var room = new Room(RoomProto.JailRoom);

            var player = new Player(introResult.PlayerName);
            //player.X = 1;
            //player.Y = 3;
            player.X = 10;
            player.Y = 10;
            //player.X = 22;
            //player.Y = 17;
            player.Inventory.Add(Item.JailKey);
            Player = player;
            room.Add(player);

            var door = new Door(Item.JailKey);
            door.X = 44;
            door.Y = 12;
            room.Add(door);

            for (int i = 0; i < 5; i++)
            {
                var enemy = new Actor();
                enemy.HP = 2;
                enemy.Character = 'G';
                enemy.Name = "Grunt";
                enemy.Strength = 2;
                //enemy.X = 14;
                //enemy.Y = 1;
                enemy.X = 17 + i;
                enemy.Y = 19;
                room.Add(enemy);
            }

            room.Draw();
            DrawStatus();
            Display.Draw();

            bool pauseEnemy = true;
            while (true)
            {
                room.Tick();
                var key = Display.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        player.Move(0, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        player.Move(0, 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        player.Move(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        player.Move(1, 0);
                        break;
                    case ConsoleKey.K:
                        player.Die();
                        break;
                    case ConsoleKey.P:
                        pauseEnemy = !pauseEnemy;
                        break;
                    case ConsoleKey.Enter:
                        player.DoAction();
                        break;
                }

                var sw = new Stopwatch();
                sw.Start();
                room.Draw();
                DrawStatus();
                Display.Draw();
                sw.Stop();
                Debug.Trace($"Drawing took {sw.Elapsed.TotalMilliseconds}ms");
            }
        }
    }
}
