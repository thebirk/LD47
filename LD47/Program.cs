using System;
using System.Diagnostics;
using System.Threading;

namespace LD47
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Trace("Debug works :)");

            while(true)
            {
                Display.Init(80, 35);

                IntroResult introResult;
                if (false)
                {
                    introResult = IntroSequence.Run();
                }
                else
                {
                    introResult = new IntroResult
                    {
                        PlayerName = IntroSequence.RandomPlayerName()
                    };
                }

                Display.ReadKey();
                Game game = new Game(introResult);
                try
                {
                    game.Run();
                } catch (EndGameException e) {
                    Display.Clear(true);

                    // draw game over screen
                    Console.SetCursorPosition(0, 0);
                    Utils.WriteOneByOne($"Agent {e.Player.Name} was found dead in {e.Player.Room.Proto.Name} by a rescue team.", 10);
                    Console.WriteLine();
                    Utils.WriteOneByOne($"The rescue team did not manage to recover the L.O.O.P and the agency has recruited another agent.", 10);
                    Console.WriteLine();
                    Thread.Sleep(700);
                    Console.WriteLine();
                    Utils.WriteOneByOne($"LVL: {e.Player.Level}", 10);
                    Console.WriteLine();
                    Utils.WriteOneByOne($"HP:  {e.Player.MaxHP}", 10);
                    Console.WriteLine();
                    Utils.WriteOneByOne($"STR: {e.Player.MaxStrength}", 10);
                    Console.WriteLine();

                    Thread.Sleep(400);
                    Console.WriteLine();
                    Utils.WriteOneByOne($"Press any key to try again as another agent", 5);

                    Display.ReadKey();
                }
            }
        }
    }
}
