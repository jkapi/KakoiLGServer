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
        public List<KeyValuePair<string, string>> ChatMessages = new List<KeyValuePair<string, string>>();
        public int[,] mainboard = new int[8, 8];

        public Stopwatch Timer;

        public MemoryStream Data = null;
        public int currentMainboardPlayer = 0;

        public Room(string name, bool locked, string password = "")
        {
            Name = name;
            Locked = locked;
            Password = password;
            ID = id;
            id++;
            Timer = new Stopwatch();
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    mainboard[x, y] = -1;
            mainboard[3, 3] = 0;
            mainboard[3, 4] = 1;
            mainboard[4, 3] = 2;
            mainboard[4, 4] = 3;
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
                case MinigameTypes.MainGame: Minigames.Maingame.Init(this); break;
                case MinigameTypes.FlySwat: Minigames.FlySwat.InitRoom(this); break;
                case MinigameTypes.GameSelect: Minigames.GameSelect.Init(this); break;
                case MinigameTypes.TapWhite: Minigames.TapWhite.Init(this); break;
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
