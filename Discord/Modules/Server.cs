using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;

namespace SR.Discord.Modules
{
    public class Server : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// Player data.
        /// </summary>
        private struct Player(string ping, string points, string nickname)
        {
            public int points = int.Parse(points);
            public int ping = int.Parse(ping);
            public string nickname = nickname;
        }

        /// <summary>
        /// Speedrun server information.
        /// </summary>
        /// <returns></returns>
        [SlashCommand("speedrun", "Speedrun server information.")]
        public async Task Speedrun() =>
            await Query("Speedrun", "213.32.18.205", 28960);

        /// <summary>
        /// Deathrun server information.
        /// </summary>
        /// <returns></returns>
        [SlashCommand("deathrun", "Deathrun server information.")]
        public async Task Deathrun() =>
            await Query("Deathrun", "213.32.18.205", 28962);

        /// <summary>
        /// BattleRoyale server information.
        /// </summary>
        /// <returns></returns>
        [SlashCommand("battleroyale", "BattleRoyale server information.")]
        public async Task BattleRoyale() => 
            await Query("Battle Royale", "213.32.18.205", 28964);

        /// <summary>
        /// Query server informations.
        /// </summary>
        /// <param name="host">The server host ip.</param>
        /// <param name="port">The server port.</param>
        /// <returns></returns>
        private async Task Query(string name, string host, int port)
        {
            StringBuilder response = new();

            using Socket client = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Connect(IPAddress.Parse(host), port);

            byte[] buffer = new byte[65565];
            byte[] header = [0xFF, 0xFF, 0xFF, 0xFF];
            byte[] cmd = Encoding.UTF8.GetBytes("getstatus");

            using MemoryStream stream = new();
            using BinaryWriter packet = new(stream);
            packet.Write(header);
            packet.Write(cmd);

            client.Send(stream.ToArray(), SocketFlags.None);
            do
            {
                int bytes = client.Receive(buffer);
                response.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            }
            while (client.Available > 0);

            var lines = response.ToString().Split("\n");

            var info = lines[1].Substring(1)
                .Split("\\")
                .Chunk(2)
                .GroupBy(x => x[0])
                .ToDictionary(g => g.Key, g => g.First()[1]);

            var players = lines.Skip(2)
                .Select(line => string.Join(" ", line.Split(" ").Skip(2)).Replace("\"", ""))
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();

            var map = !string.IsNullOrEmpty(info["mapname"]) ? info["mapname"] : "Undefined";
            var playersMessage = players.Count > 0 ? string.Join("\n", players) : "No players online";

            Embed em = new EmbedBuilder()
                .WithTitle(name)
                .WithColor(new Color(164, 22, 248))
                .WithThumbnailUrl("https://cdn.discordapp.com/icons/335075122467700740/8152834be097199cff8d46a2ae1e5588.png")
                .AddField("Map", map)
                .AddField("Players", playersMessage)
                .Build();

            await RespondAsync(embed: em);
        }
    }
}
