using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KakoiLGServer
{
    class PlayerList : IEnumerable
    {
        Player[] players;

        public PlayerList(int size)
        {
            players = new Player[size];
        }

        public int Count { get {
                int ret = 0;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] != null)
                    {
                        ret++;
                    }
                }
                return ret;
        } }

        public bool Join(Player player)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == null)
                {
                    players[i] = player;
                    player.PositionInRoom = i;
                    return true;
                }
            }
            return false;
        }

        public bool Move(Player player, int position)
        {
            if (players[position] == null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] == player)
                    {
                        players[i] = null;
                        players[position] = player;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Contains(Player player)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == player)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Leave(Player player)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == player)
                {
                    players[i] = null;
                    return true;
                }
            }
            return false;
        }

        public List<Player> SortByHighestScore()
        {
            List<Player> realplayers = new List<Player>();
            foreach(Player p in players)
            {
                if (p != null)
                {
                    realplayers.Add(p);
                }
            }
            if (Count != 0)
                return realplayers.OrderByDescending(o => o.Score).ToList();
            else
                return realplayers;
        }

        public IEnumerator GetEnumerator()
        {
            List<Player> output = new List<Player>();
            foreach(Player player in players)
                if (player != null)
                    output.Add(player);
            return output.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<Player> output = new List<Player>();
            foreach (Player player in players)
                if (player != null)
                    output.Add(player);
            return output.GetEnumerator();
        }

        public Player this[int i]
        {
            get { return players[i]; }
            set { players[i] = value; }
        }

        public int this[Player p]
        {
            get
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] == p)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
}
