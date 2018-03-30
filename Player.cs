﻿using Lidgren.Network;
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

        public float MouseX = 0;
        public float MouseY = 0;
        public float MouseDX = 0;
        public float MouseDY = 0;

        public NetConnection Connection;

        public Player(string name, NetConnection connection)
        {
            Name = name;
            Connection = connection;
        }
    }
}