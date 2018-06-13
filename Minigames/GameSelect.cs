using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer.Minigames
{
    class GameSelect
    {
        static Dictionary<Room, Stopwatch> doneTimers = new Dictionary<Room, Stopwatch>();

        internal static void Tick(Room room, PlayerList players)
        {
            int countDone = 0;
            foreach(Player player in players)
            {
                if (player.MouseX != Math.Round(player.MouseX) && player.MouseY != Math.Round(player.MouseY))
                {
                    countDone++;
                }
            }
            if (countDone == players.Count)
            {
                if (!doneTimers.ContainsKey(room))
                {
                    doneTimers.Add(room, Stopwatch.StartNew());
                }
                if (players.Count > 0)
                {
                    if (doneTimers[room].ElapsedMilliseconds > 3000)
                    {
                        doneTimers[room].Stop();
                        doneTimers.Remove(room);
                        int newroom = (int)Math.Round((players[0].MouseY - Math.Truncate(players[0].MouseY)) * 10);
                        switch (newroom)
                        {
                            //case 1: room.StartMinigame(MinigameTypes.ClimbTheMountain); break;
                            case 3: room.StartMinigame(MinigameTypes.TapWhite); break;
                            //case 4: room.StartMinigame(MinigameTypes.Quiz); break;
                            //case 5: room.StartMinigame(MinigameTypes.DinoCollectStuff); break;
                            case 2:
                            default: room.StartMinigame(MinigameTypes.FlySwat); break;
                        }
                    }
                }
            }
        }

        internal static void Init(Room room)
        {
            room.Data = null;
            room.Data = new MemoryStream(8);
            var writer = new BinaryWriter(room.Data);
            Random r = new Random();
            writer.Write((float)(r.NextDouble() * 5 + 3));
            writer.Write((float)(r.NextDouble() * 0.6 + 0.3));
            writer.Flush();
        }
    }
}
