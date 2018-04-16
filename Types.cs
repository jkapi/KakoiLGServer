using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    enum PacketTypes
    {
        LOGINSESSID = 0, JOINROOM = 1, LEAVEROOM = 2, GETROOM = 3, ROOMLIST = 4, CREATEROOM = 5,
        MINIGAME = 6, SETMOVE = 7, DOMOVE = 8, MOUSE = 9, PLAYER = 10, TICK = 11, SWAT = 12
    }

    enum DataTypes
    {
        BYTE = 0, FLOAT = 1, STRING = 2, INT = 3, EMPTY = 4
    }

    enum MinigameTypes
    {
        None = 0, MainGame = 1, FlySwat = 2, ClimbTheMountain = 3, DinoCollectStuff = 4, FollowTheLeader = 5
    }
}
