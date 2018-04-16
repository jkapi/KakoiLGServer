using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    class Room
    {
        static int id = 0;
        public PlayerList Players = new PlayerList(4);
        public string Name;
        public bool Locked = false;
        public string Password = "";
        public bool isStatic = false;
        public MinigameTypes CurrentMinigame { get { return currentminigame; }}
        private MinigameTypes currentminigame;
        public bool isHub = false;
        public int ID;

        public Stopwatch Timer;

        public MemoryStream Data = null;

        public Room(string name, bool locked, string password = "")
        {
            Name = name;
            Locked = locked;
            Password = password;
            ID = id;
            id++;
            Timer = new Stopwatch();
        }

        public void StartMinigame(MinigameTypes minigame)
        {
            switch(currentminigame)
            {
                case MinigameTypes.FlySwat: Minigames.FlySwat.StopRoom(this); break;
                default: break;
            }
            currentminigame = minigame;
            switch(currentminigame)
            {
                case MinigameTypes.FlySwat: Minigames.FlySwat.InitRoom(this); break;
                default: break;
            }
            NetOutgoingMessage data = Program.Server.CreateMessage();
            data.Write((short)PacketTypes.MINIGAME);
            data.Write((short)minigame);
            foreach (Player player in Players)
            {
                NetOutgoingMessage outmsg = Program.Server.CreateMessage();
                outmsg.Write(data.Data);
                player.Connection.SendMessage(outmsg, NetDeliveryMethod.ReliableUnordered, 0);
            }
        }
    }
}
