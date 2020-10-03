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
            var room = new Room(RoomProto.Town);

            var player = new Player(introResult.PlayerName);
            //player.X = 1;
            //player.Y = 3;
            player.X = 10;
            player.Y = 10;
            room.Add(player);

            var enemy = new Actor();
            enemy.HP = 10;
            enemy.Character = 'G';
            enemy.Name = "Grunt";
            enemy.Strength = 2;
            //enemy.X = 14;
            //enemy.Y = 1;
            enemy.X = 17;
            enemy.Y = 19;
            room.Add(enemy);

            room.Draw();
            Display.Draw();

            bool pauseEnemy = true;
            while (true)
            {
                if(!pauseEnemy) enemy.Think();
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
