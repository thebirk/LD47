using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LD47
{
    public class IntroResult
    {
        public string PlayerName { get; set; }
    }

    public class IntroSequence
    {
        public static string[] FirstNames =
        {
            "Robin",
            "Kim",
            "Max",
            "John",
            "Ava",
            "Ada",
            "Alia",
            "Buck",
            "Flash",
            "Kerr",
            "Keyan",
        };
        public static string[] LastNames =
        {
            "Morales",
            "Smith",
            "Müller",
            "Whitlock",
            "West",
            "Finch",
            "Lee",
        };

        public static string RandomPlayerName()
        {
            var rand = new Random();

            return $"{FirstNames[rand.Next(0, FirstNames.Length)]} {LastNames[rand.Next(0, LastNames.Length)]}";
        }

        public static void WriteOneByOne(string str, int ms)
        {
            foreach (var ch in str)
            {
                Console.Write(ch);
                Thread.Sleep(ms);
            }
        }

        public static IntroResult Run()
        {
            var result = new IntroResult();

            Display.Clear(true);
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;

            WriteOneByOne("Welcome Agent: ", 100);
            result.PlayerName = Console.ReadLine();
            result.PlayerName = result.PlayerName.Trim();

            if (result.PlayerName == string.Empty)
            {
                result.PlayerName = RandomPlayerName();
            }

            Console.WriteLine();

            WriteOneByOne($"Agent {result.PlayerName}, last night at 0400 hours we had a break in.", 50);
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(400);
            WriteOneByOne($"We are going to need your help to retrieve the L.O.O.P that was stolen.", 50);
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(400);
            WriteOneByOne($"The Loop Oriented Oscillating Programmer is crucial to our survival on this alien planet", 50);
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(400);
            WriteOneByOne($"Without it we are doomed", 50);
            WriteOneByOne($"...", 200);
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(700);
            WriteOneByOne("Godspeed agent, we are counting on you...", 100);
            Console.WriteLine();

            Display.ReadKey();

            Display.Clear(true);

            return result;
        }
    }
}
