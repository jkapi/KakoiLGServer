using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer.Minigames
{
    class TapWhite
    {
        internal static void Tick(Room room, PlayerList players)
        {
            foreach(Player player in players)
                if (player != null)
                    if (player.MouseX == 1)
                    {
                        player.Score++;
                        player.MouseX = 0;
                    }
        }

        internal static void Init(Room room)
        {
            foreach(Player player in room.Players)
                if (player != null)
                    player.Score = 0;
        }
    }
}
