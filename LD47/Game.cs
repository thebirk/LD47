using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LD47
{
    public class Game
    {
        private readonly IntroResult introResult;

        public Game(IntroResult introResult)
        {
            this.introResult = introResult;
        }

        public void Run()
        {
            var room = new Room(RoomProto.StartRoom);
            var player = new Player(introResult.PlayerName);
            player.X = 1;
            player.Y = 3;
            room.Add(player);

            var enemy = new Actor();
            enemy.HP = 10;
            enemy.Character = 'G';
            enemy.Name = "Grunt";
            enemy.X = 14;
            enemy.Y = 1;
            room.Add(enemy);

            room.Draw();
            Display.Draw();

            while (true)
            {
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
                }

                var sw = new Stopwatch();
                sw.Start();
                room.Draw();
                Display.Draw();
                sw.Stop();
                Debug.Trace($"Drawing took {sw.Elapsed.TotalMilliseconds}ms");
            }
        }
    }
}
