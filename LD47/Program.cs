using System;
using System.Diagnostics;

namespace LD47
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Trace("Debug works :)");

            while(true)
            {
                Display.Init(80, 25);

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

                }
            }
        }
    }
}
