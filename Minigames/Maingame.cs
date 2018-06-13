using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer.Minigames
{
    class Maingame
    {
        static Dictionary<Room, Stopwatch> stoptimers = new Dictionary<Room, Stopwatch>();
        internal static void Init(Room room)
        {
            if (room.Data != null)
                room.Data.Dispose();
        }

        internal static void Tick(Room room, PlayerList players)
        {
            if (players.Count > 0)
            {
                room.Data = new MemoryStream(66);
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        room.Data.WriteByte((byte)(room.mainboard[x, y] + 1));
                    }
                room.Data.WriteByte((byte)room.currentMainboardPlayer);
                room.Data.WriteByte((byte)room.Players[room.currentMainboardPlayer].MainboardMoves);
                room.Data.Flush();
            }

            if (stoptimers.ContainsKey(room))
            {
                if (stoptimers[room].ElapsedMilliseconds > 1000)
                {
                    stoptimers[room].Stop();
                    stoptimers.Remove(room);
                    room.StartMinigame(MinigameTypes.GameSelect);
                }
            }
        }

        internal static void Stop(Room room)
        {

        }

        internal static void Chat(string text, Player player)
        {
            foreach(Room room in Program.Rooms)
            {
                if (room.Players.Contains(player))
                {
                    room.ChatMessages.Add(new KeyValuePair<string, string>(player.Name, text));
                    foreach (Player reciever in room.Players)
                    {
                        NetOutgoingMessage outmsg = Program.Server.CreateMessage();
                        outmsg.Write((short)PacketTypes.CHAT);
                        outmsg.Write((string)player.Name);
                        outmsg.Write((string)text);
                        outmsg.Write((int)player.PositionInRoom);
                        reciever.Connection.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 8);
                    }
                }
            }
        }

        internal static void doMove(byte x, byte y, Player player)
        {
            foreach (Room room in Program.Rooms)
            {
                if (room.Players.Contains(player))
                {
                    DoMove(x, y, room.currentMainboardPlayer, room);
                }
            }
        }

        static void DoMove(int px, int py, int pc, Room r)
        {
            var board = r.mainboard;
            List<Point> directions = new List<Point>() {
                new Point(-1,-1), new Point(0,-1), new Point(1,-1),
                new Point(-1, 0), /*   niets    */ new Point(1, 0),
                new Point(-1, 1), new Point(0, 1), new Point(1, 1)
            };
            foreach (Point direction in directions)
            {
                int length = CheckDirection(new Point(px, py), direction, pc,r);
                if (length > 0)
                {
                    for (int i = 1; i <= length; i++)
                    {
                        board[px + (direction.X * i), py + (direction.Y * i)] = pc;
                    }
                }
            }
            r.Players[r.currentMainboardPlayer].MainboardMoves--;
            if (r.Players[r.currentMainboardPlayer].MainboardMoves < 1)
            {
                r.currentMainboardPlayer++;
                if (r.currentMainboardPlayer > r.Players.Count - 1)
                {
                    r.currentMainboardPlayer = 0;

                    stoptimers.Add(r, Stopwatch.StartNew());
                }
            }

        }

        static int CheckDirection(Point lc, Point direction, int pc, Room r)
        {
            var board = r.mainboard;
            int i = 1;
            int steps = 0;
            lc += direction;
            while (lc.X >= 0 && lc.Y >= 0 && lc.X < 8 && lc.Y < 8 && i > 0)
            {
                if (board[lc.X, lc.Y] == pc || board[lc.X, lc.Y] == pc + 4)
                {
                    steps = i;
                    i = -1;
                }
                if (board[lc.X, lc.Y] == -1)
                {
                    i = -1;
                }
                i++;
                lc += direction;
            }
            return steps;
        }

        static bool CanPlace(int x, int y, Room r)
        {
            return GetBoardPlaceFilled(x - 1, y - 1, r) || GetBoardPlaceFilled(x, y - 1,r) || GetBoardPlaceFilled(x + 1, y - 1,r) ||
                   GetBoardPlaceFilled(x - 1, y, r) || GetBoardPlaceFilled(x, y,r) || GetBoardPlaceFilled(x + 1, y,r) ||
                   GetBoardPlaceFilled(x - 1, y + 1,r) || GetBoardPlaceFilled(x, y + 1,r) || GetBoardPlaceFilled(x + 1, y + 1,r);
        }

        static int GetBoardPlace(int x, int y, Room r)
        {
            var board = r.mainboard;
            if (x < 0 || x > 7 || y < 0 || y > 7)
            {
                return -1;
            }
            else
            {
                return board[x, y];
            }
        }

        static bool GetBoardPlaceFilled(int x, int y, Room r)
        {
            var board = r.mainboard;
            if (x < 0 || x > 7 || y < 0 || y > 7)
            {
                return false;
            }
            else
            {
                return board[x, y] != -1;
            }
        }

        class Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static Point operator +(Point p1, Point p2)
            {
                return new Point(p1.X + p2.X, p1.Y + p2.Y);
            }
        }
    }
}
