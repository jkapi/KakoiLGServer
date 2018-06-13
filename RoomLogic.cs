using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    class RoomLogic
    {
        static Dictionary<MinigameTypes, Dictionary<PacketTypes, Action<Room, PlayerList>>> MinigameHandlers = 
                    new Dictionary<MinigameTypes, Dictionary<PacketTypes, Action<Room, PlayerList>>>
                    { { MinigameTypes.FlySwat, new Dictionary<PacketTypes, Action<Room, PlayerList>>(){
                            { PacketTypes.TICK, Minigames.FlySwat.Tick }
                      }},
                      { MinigameTypes.MainGame, new Dictionary<PacketTypes, Action<Room, PlayerList>>(){
                            { PacketTypes.TICK, Minigames.Maingame.Tick }
                      }},
                      { MinigameTypes.GameSelect, new Dictionary<PacketTypes, Action<Room, PlayerList>>(){
                          { PacketTypes.TICK, Minigames.GameSelect.Tick }
                      }},
                      { MinigameTypes.TapWhite, new Dictionary<PacketTypes, Action<Room, PlayerList>>(){
                          { PacketTypes.TICK, Minigames.TapWhite.Tick }
                      }},
                    };

        public static void HandleGetRoom(NetIncomingMessage inc, Player player, List<Room> rooms)
        {

        }

        public static void HandleRoomList(NetIncomingMessage inc, List<Room> Rooms)
        {
            NetOutgoingMessage outmsg = Program.Server.CreateMessage();

            outmsg.Write((short)PacketTypes.ROOMLIST);
            outmsg.Write((int)Rooms.Count - 1);
            foreach (Room room in Rooms)
            {
                if (!room.isHub)
                {
                    outmsg.Write((int)room.ID);
                    outmsg.Write((string)room.Name);
                    outmsg.Write((bool)room.Locked);
                    outmsg.Write((int)room.Players.Count);
                    foreach (Player p in room.Players)
                    {
                        outmsg.Write(p.Name);
                    }
                }
            }

            inc.SenderConnection.SendMessage(outmsg, NetDeliveryMethod.ReliableUnordered, 0);
        }

        public static void HandleJoinRoom(NetIncomingMessage inc, Player player, List<Room> rooms)
        {
            inc.ReadByte();
            inc.ReadInt32();
            int id = inc.ReadInt32();
            Room joinRoom = null;

            foreach (Room room in rooms)
            {
                // Remove player from current room if it is in a room
                if (room.Players.Contains(player))
                {
                    if (room.ID == id)
                    {
                        return;
                    }
                    room.Players.Leave(player);
                }

                if (room.ID == id)
                {
                    joinRoom = room;
                }
            }
            
            if (joinRoom != null)
            {
                joinRoom.Players.Join(player);
            }

            joinRoom.StartMinigame(MinigameTypes.MainGame);
        }

        public static void HandleLeaveRoom(NetIncomingMessage inc, Player player, List<Room> rooms)
        {
            // Remove player from current room if it is in a room;
            foreach (Room room in rooms)
            {
                if (room.Players.Contains(player))
                {
                    room.Players.Leave(player);
                }
            }
        }
        
        public static void HandleCreateRoom(NetIncomingMessage inc, Player player, List<Room> rooms)
        {
            inc.ReadByte();
            inc.ReadInt32();
            string name = inc.ReadString();
            bool canCreate = true;
            NetOutgoingMessage outmsg = Program.Server.CreateMessage();
            foreach (Room room in rooms)
            {
                if (room.Name == name)
                {
                    canCreate = false;
                    break;
                }
            }

            if (canCreate == true)
            {
                // Remove player from current room if it is in a room;
                foreach (Room room in rooms)
                {
                    if (room.Players.Contains(player))
                    {
                        room.Players.Leave(player);
                    }
                }
                Room newRoom = new Room(name, false);
                newRoom.Players.Join(player);
                rooms.Add(newRoom);
                outmsg.Write(true);
            }
            else
            {
                outmsg.Write(false);
            }
            player.Connection.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public static void Tick(List<Room> rooms, Dictionary<int, Player> players)
        {
            // Remove empty rooms
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if (rooms[i].Players.Count == 0 && rooms[i].isStatic == false)
                {
                    rooms.Remove(rooms[i]);
                }
            }

            foreach(Room room in rooms)
            {
                if (!room.isHub)
                {
                    NetOutgoingMessage data = Program.Server.CreateMessage();
                    data.Write((short)PacketTypes.GETROOM);
                    data.Write((int)room.Players.Count);
                    foreach (Player p in room.Players)
                    {
                        if (p != null)
                        {
                            data.Write(p.Id);
                            data.Write(p.Name);
                            data.Write(p.MouseX);
                            data.Write(p.MouseY);
                            data.Write(p.MouseDX);
                            data.Write(p.MouseDY);
                            data.Write(p.PositionInRoom);
                            data.Write(p.Score);
                        }
                    }
                    if (room.CurrentMinigame != MinigameTypes.None)
                    {
                        MinigameHandlers[room.CurrentMinigame][PacketTypes.TICK](room, room.Players);
                    }
                    if (room.Data != null && room.Data.CanRead)
                    {
                        try
                        {
                            data.Write((int)room.Data.Length);
                            room.Data.Position = 0;
                            for (int i = 0; i < room.Data.Length; i++)
                            {
                                data.Write((byte)room.Data.ReadByte());
                            }
                        }
                        catch
                        {
                            // Though it is very unlikely, this can happen, client side won't bother if it doesn't get it.

                        }
                    }
                    else
                    {
                        data.Write((int)0);
                    }
                    foreach (Player player in room.Players)
                    {
                        NetOutgoingMessage outmsg = Program.Server.CreateMessage();
                        outmsg.Write(data.Data);
                        if (player != null)
                        {
                            player.Connection.SendMessage(outmsg, NetDeliveryMethod.ReliableUnordered, 0);
                        }
                    }
                }
            }
        }
    }
}
