using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    enum PacketTypes
    {
        LOGINUSERPASS, LOGINSESSID, JOINROOM, LEAVEROOM, GETROOM, ROOMLIST, CREATEROOM,
        MINIGAME, SETMOVE, DOMOVE, MOUSE, PLAYER, TICK, SWAT
    }

    enum DataTypes
    {
        BYTE, FLOAT, STRING, INT, EMPTY
    }

    enum MinigameTypes
    {
        None, MainGame, FlySwat, ClimbTheMountain, DinoCollectStuff, FollowTheLeader
    }
}
