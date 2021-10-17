using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Iswenzz.SR.Discord.Commands
{
    public class Server : ModuleBase
    {
        private Dictionary<string, string> Info { get; set; } = new Dictionary<string, string>();
        private List<QueryPlayer> Players { get; set; } = new List<QueryPlayer>();

        public readonly byte[] PACKET_HEADER = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Query Sr- Speedrun server information.
        /// </summary>
        /// <returns></returns>
        [Command("speedrun")]
        public async Task Speedrun() =>
            await Context.Channel.SendMessageAsync("", false, Query("213.32.18.205", 28960));

        /// <summary>
        /// Query server informations.
        /// </summary>
        /// <param name="gameServerIP">The gameserver host ip.</param>
        /// <param name="gameServerPort">The gameserver port.</param>
        /// <returns></returns>
        public EmbedBuilder Query(string gameServerIP, int gameServerPort)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Connect(IPAddress.Parse(gameServerIP), gameServerPort);

            byte[] packet_send = new byte[16];
            byte[] cmd = Encoding.ASCII.GetBytes("getstatus");

            Buffer.BlockCopy(PACKET_HEADER, 0, packet_send, 0, PACKET_HEADER.Length);
            Buffer.BlockCopy(cmd, 0, packet_send, PACKET_HEADER.Length, cmd.Length);

            StringBuilder response = new StringBuilder();
            byte[] bufferRecv = new byte[65565];

            _ = new IPEndPoint(IPAddress.Parse(gameServerIP), 0);
            client.Send(packet_send, SocketFlags.None);

            do
            {
                int bytesReceived = client.Receive(bufferRecv);
                response.Append(Encoding.ASCII.GetString(bufferRecv, 0, bytesReceived));
            } 
            while (client.Available > 0);

            string[] raw = response.ToString().Split("\n");
            string rawInfo = raw[1];
            string[] temp = rawInfo.Split("\\");

            for (int i = 1; i < temp.Length; i += 2)
                Info.Add(temp[i], Regex.Replace(temp[i + 1], "(\\^)([0-9])", ""));

            for (int i = 2; i < raw.Length; i++)
            {
                string[] rawPlayer = raw[i].Split(" ");
                string name = "";

                if (rawPlayer.Length < 2)
                    continue;

                for (int z = 2; z < rawPlayer.Length; z++)
                {
                    if (z != rawPlayer.Length)
                        name += rawPlayer[z] + " ";
                    else
                        name += rawPlayer[z];
                }
                QueryPlayer player = new QueryPlayer(rawPlayer[1], rawPlayer[0], name.Replace("\"", ""));
                Players.Add(player);
            }

            EmbedBuilder em = new EmbedBuilder
            {
                Title = Info["sv_hostname"].ToString(),
                Color = new Color(164, 22, 248),
            };
            em.AddField("Map", Info["mapname"].ToString(), false);

            string playerDetails = "No players Online";
            if (Players.Count > 0)
            {
                playerDetails = "";
                foreach (QueryPlayer p in Players)
                    playerDetails += p.nickname + "\n";
            }
            em.AddField("Players", playerDetails, false);
            em.Build();

            return em;
        }

        /// <summary>
        /// Player data from server query.
        /// </summary>
        public struct QueryPlayer
        {
            public int points;
            public int ping;
            public string nickname;

            public QueryPlayer(string ping, string points, string nickname)
            {
                this.ping = int.Parse(ping);
                this.points = int.Parse(points);
                this.nickname = nickname;
            }
        }
    }
}