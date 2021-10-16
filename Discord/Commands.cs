using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Iswenzz.SR.Discord
{
    /// <summary>
    /// Contains all bot commands callback.
    /// </summary>
    public class Commands : ModuleBase
    {
        /// <summary>
        /// Send a <see cref="EmbedBuilder"/> helper message.
        /// </summary>
        [Command("help")]
        public async Task Help()
        {
            EmbedBuilder em = new EmbedBuilder
            {
                ThumbnailUrl = "https://cdn.discordapp.com/icons/335075122467700740/8152834be097199cff8d46a2ae1e5588.png",
                Color = new Color(164, 22, 248),
                Title = "Help Commands"
            };
            em.AddField("!times <map name> <speed> <way>", "Show map times: <speed> = 210 or 190, <way> is ns0 ns1 ns2.. or s0 s1 s2..", false);
            em.AddField("!pb <discord name>", "Show personal best: <discord name> = @Iswenzz#3906", false);
            em.AddField("!speedrun", "Show the current map and players connected in SR Speedrun", false);
            em.Build();

            await Context.Channel.SendMessageAsync("", false, em);
        }

        /// <summary>
        /// Send a specific sr leaderboard.
        /// </summary>
        /// <param name="map">Map name (i.e: mp_dr_lolz)</param>
        /// <param name="speed">Player speed (i.e: 210)</param>
        /// <param name="way">Way id (i.e: ns0)</param>
        /// <returns></returns>
        [Command("times")]
        public async Task Times(string map, string speed, string way)
        {
            MapTimes times = new MapTimes();
            await Context.Channel.SendMessageAsync("", false, times.SendTimes(map, speed, way));
        }

        /// <summary>
        /// Query Sr- Speedrun server information.
        /// </summary>
        /// <returns></returns>
        [Command("speedrun")]
        public async Task Speedrun()
        {
            Server serv = new Server();
            await Context.Channel.SendMessageAsync("", false, serv.Query("213.32.18.205", 28960));
        }

        /// <summary>
        /// Stop the Speedrun BOT.
        /// </summary>
        /// <returns></returns>
        [RequireOwner]
        [Command("stop")]
        public async Task Stop()
        {
            await Task.Delay(1);
            Environment.Exit(0);
        }
    }
}