using System;
using System.Collections.Generic;
using System.Text;

namespace LD47
{
    class EndGameException : Exception
    {
        public Player Player { get; set; }

        public EndGameException(Player player)
        {
            Player = player;
        }
    }
}
