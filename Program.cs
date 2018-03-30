using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    class Program
    {
        // Server object
        public static NetServer Server;
        // Configuration object
        static NetPeerConfiguration Config;

        public static long tick;

        static void Main(string[] args)
        {
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            Config = new NetPeerConfiguration("Kakoi");

            // Set server port
            Config.Port = 25000;

            // Max client amount
            Config.MaximumConnections = 200;

            // Enable New messagetype. Explained later
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Config.ConnectionTimeout = 10.0f;

            // Create new server based on the configs just defined
            Server = new NetServer(Config);

            // Start it
            Server.Start();

            // Eh..
            Console.WriteLine("Server Started");
            
            List<Player> Players = new List<Player>();
            Room HubRoom = new Room("Hub", false) { isStatic = true , isHub = true};
            List<Room> Rooms = new List<Room>();
            Rooms.Add(HubRoom);
            Rooms.Add(new Room("DebugRoom", false) { isStatic = true });
            Rooms.Add(new Room("DebugLockedRoom", true, "test") { isStatic = true });

            // Object that can be used to store and read messages
            NetIncomingMessage inc;

            // Check time
            DateTime time = DateTime.Now;

            // Create timespan of 20ms
            TimeSpan timetopass = new TimeSpan(0, 0, 0, 0, 20);

            // Write to con..
            Console.WriteLine("Waiting for new connections and updateing world state to current ones");

            // Main loop
            // This kind of loop can't be made in XNA. In there, its basically same, but without while
            // Or maybe it could be while(new messages)
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        break;
                    }
                }
                // Server.ReadMessage() Returns new messages, that have not yet been read.
                // If "inc" is null -> ReadMessage returned null -> Its null, so dont do this :)
                while ((inc = Server.ReadMessage()) != null)
                {
                    Player player = null;
                    foreach (Player p in Players)
                    {
                        if (p.Connection != inc.SenderConnection)
                            continue;
                        else
                            player = p;
                    }
                    // Theres few different types of messages. To simplify this process, i left only 2 of em here
                    switch (inc.MessageType)
                    {
                        // If incoming message is Request for connection approval
                        // This is the very first packet/message that is sent from client
                        // Here you can do new player initialisation stuff
                        case NetIncomingMessageType.ConnectionApproval:

                            // Read the first byte of the packet
                            // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                            if (inc.ReadInt16() == (short)PacketTypes.LOGINSESSID)
                            {
                                Console.WriteLine("Incoming LOGIN");

                                // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                                inc.SenderConnection.Approve();
                                Player newplayer = new Player(inc.ReadString(), inc.SenderConnection);
                                Players.Add(newplayer);
                                HubRoom.Players.Add(newplayer);
                                
                                // Create message, that can be written and sent
                                NetOutgoingMessage outmsg = Server.CreateMessage();

                                // first we write byte
                                outmsg.Write((short)PacketTypes.LOGINSESSID);

                                // TODO: verstuur ok

                                // Now, packet contains:
                                // Byte = packet type
                                // Int = how many players there is in game
                                // character object * how many players is in game

                                // Send message/packet to all connections, in reliably order, channel 0
                                // Reliably means, that each packet arrives in same order they were sent. Its slower than unreliable, but easyest to understand
                                Server.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                

                                // Debug
                                Console.WriteLine("Approved new connection and updated the world status");
                            }

                            break;
                        // Data type is all messages manually sent from client
                        // ( Approval is automated process )
                        case NetIncomingMessageType.Data:
                            // Read first byte
                            PacketTypes MessageType = (PacketTypes)inc.ReadInt16();
                            if (MessageType == PacketTypes.MOUSE)
                            {
                                inc.ReadByte(); // Read type byte
                                inc.ReadInt32();
                                player.MouseX = inc.ReadFloat();
                                player.MouseY = inc.ReadFloat();
                                player.MouseDX = inc.ReadFloat();
                                player.MouseDY = inc.ReadFloat();
                            }
                            switch(MessageType)
                            {
                                case PacketTypes.ROOMLIST: RoomLogic.HandleRoomList(inc, Rooms); break;
                                case PacketTypes.GETROOM: RoomLogic.HandleGetRoom(inc, player, Rooms); break;
                                case PacketTypes.JOINROOM: RoomLogic.HandleJoinRoom(inc, player, Rooms); break;
                                case PacketTypes.LEAVEROOM: RoomLogic.HandleLeaveRoom(inc, player, Rooms); break;
                                case PacketTypes.CREATEROOM: RoomLogic.HandleCreateRoom(inc, player, Rooms); break;
                                case PacketTypes.SWAT: inc.ReadByte(); inc.ReadInt32(); Minigames.FlySwat.Swatted.Add(inc.ReadInt32(), player); break;
                                default: break;
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            // In case status changed
                            // It can be one of these
                            // NetConnectionStatus.Connected;
                            // NetConnectionStatus.Connecting;
                            // NetConnectionStatus.Disconnected;
                            // NetConnectionStatus.Disconnecting;
                            // NetConnectionStatus.None;

                            // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                            Console.WriteLine(inc.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)inc.SenderConnection.Status);
                            if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                // TODO: Remove player
                                foreach (Player p in Players)
                                {
                                    // If stored connection ( check approved message. We stored ip+port there, to character obj )
                                    // Find the correct character
                                    if (p.Connection == inc.SenderConnection)
                                        player = p;

                                }
                                if (player != null)
                                {
                                    foreach (Room room in Rooms)
                                    {
                                        if (room.Players.Contains(player)) { room.Players.Remove(player); }
                                    }
                                    Players.Remove(player);
                                }
                            }
                            break;
                        default:
                            // As i statet previously, theres few other kind of messages also, but i dont cover those in this example
                            // Uncommenting next line, informs you, when ever some other kind of message is received
                            //Console.WriteLine("Not Important Message");
                            break;
                    }
                } // If New messages

                // if 20ms has passed
                if ((time + timetopass) < DateTime.Now)
                {
                    // If there is even 1 client
                    if (Server.ConnectionsCount != 0)
                    {
                        RoomLogic.Tick(Rooms, Players);
                        tick++;
                    }
                    // Update current time
                    time = DateTime.Now;
                }
                
                // Sleep the server to lower cpu usage
                if (Server.ConnectionsCount > 0)
                {
                    System.Threading.Thread.Sleep(1);
                }
                else
                {
                    System.Threading.Thread.Sleep(20);
                }
            }
        }
    }
}
