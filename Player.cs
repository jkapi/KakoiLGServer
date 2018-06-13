using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    class Player
    {
        public string Name;
        public int Id;

        public float MouseX = 0;
        public float MouseY = 0;
        public float MouseDX = 0;
        public float MouseDY = 0;

        public NetConnection Connection;

        public int PositionInRoom;

        public int MainboardMoves = 0;
        public int Score = 0;

        public Player(string name, int id, NetConnection connection)
        {
            Name = name;
            Id = id;
            Connection = connection;
        }
    }
}
