using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    class Room
    {
        static int id = 0;
        public List<Player> Players = new List<Player>();
        public string Name;
        public bool Locked = false;
        public string Password = "";
        public bool isStatic = false;
        public MinigameTypes CurrentMinigame { get { return currentminigame; }}
        private MinigameTypes currentminigame;
        public bool isHub = false;
        public int ID;

        public MemoryStream Data = null;

        public Room(string name, bool locked, string password = "")
        {
            Name = name;
            Locked = locked;
            Password = password;
            ID = id;
            id++;
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
        }
    }
}
