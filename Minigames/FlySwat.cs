using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer.Minigames
{
    class FlySwat
    {
        // TODO: Anti-cheat inbouwen

        public static Dictionary<int, Player> Swatted = new Dictionary<int, Player>();
        static Dictionary<Room, List<Fly>> flydictionary = new Dictionary<Room, List<Fly>>();
        static Random rand = new Random();

        public static void InitRoom(Room room)
        {
            if (flydictionary.ContainsKey(room))
            {
                flydictionary[room] = new List<Fly>();
            }
            else
            {
                flydictionary.Add(room, new List<Fly>());
            }
        }

        public static void StopRoom(Room room)
        {
            if (flydictionary.ContainsKey(room))
            {
                flydictionary.Remove(room);
            }
        }

        public static void Tick(Room room, List<Player> players)
        {
            if (flydictionary.ContainsKey(room))
            {
                List<Fly> flies = flydictionary[room];
                if (rand.Next(0, 40) == 1 && flies.Count < 255)
                {
                    flies.Add(new Fly());
                }

                for (int i = flies.Count - 1; i >= 0; i--) 
                {
                    flies[i].Tick();
                    if (Swatted.Keys.Contains(flies[i].ID))
                    {
                        int tempid = flies[i].ID;
                        flies.Remove(flies[i]);
                        Swatted.Remove(tempid);
                    }
                }

                if (room.Data != null)
                {
                    room.Data.Close();
                    room.Data.Dispose();
                }
                room.Data = new MemoryStream((2 * flies.Count + 1) * 4);
                BinaryWriter writer = new BinaryWriter(room.Data);
                writer.Write((int)flies.Count);
                foreach (Fly fly in flies)
                {
                    writer.Write((int)fly.ID);
                    writer.Write((float)fly.X);
                    writer.Write((float)fly.Y);
                }
            }
        }

        class Fly
        {
            private static Random rand = new Random();

            public float X;
            public float Y;
            float gotox;
            float gotoy;
            float dx;
            float dy;
            public int ID;
            private static int counter = 0;

            public Fly()
            {
                switch(rand.Next(0,4))
                {
                    case 0: X = -20; Y = rand.Next(0, 1080); break;
                    case 1: X = 1920; Y = rand.Next(0, 1080); break;
                    case 2: Y = -20; X = rand.Next(0, 1920); break;
                    case 3: Y = 1080; X = rand.Next(0, 1920); break;
                }
                gotox = rand.Next(100, 1820);
                gotoy = rand.Next(100, 900);
                float speed = rand.Next(40, 150);
                dx = (gotox - X) / speed;
                dy = (gotoy - Y) / speed;
                ID = counter * 1000 + (int)(Program.tick % 999);
            }

            public void Tick()
            {
                X += dx;
                Y += dy;
                if (GetDistanceToGo() < 20 | rand.Next(30) < 2)
                {
                    gotox = rand.Next(100, 1820);
                    gotoy = rand.Next(100, 900);
                    float speed = rand.Next(40, 150);
                    dx = (gotox - X) / speed;
                    dy = (gotoy - Y) / speed;
                }
            }

            private double GetDistanceToGo()
            {//d= sqrt((x 2 ​ −x 1 ​ )^2 +(y 2 ​ −y 1 ​ )^2 ​)
                return Math.Sqrt((X - gotox) * (X - gotox) + (Y - gotoy) * (Y - gotoy));
            }
        }
    }
}
