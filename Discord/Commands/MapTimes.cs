using Discord;
using Discord.Commands;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Iswenzz.SR.Discord.Commands
{
    class MapTimes : ModuleBase
    {
        /// <summary>
        /// Send a specific sr leaderboard.
        /// </summary>
        /// <param name="map">Map name (i.e: mp_dr_lolz)</param>
        /// <param name="speed">Player speed (i.e: 210)</param>
        /// <param name="way">Way id (i.e: ns0)</param>
        /// <returns></returns>
        [Command("times")]
        public async Task SendTimes(string map, string speed, string way)
        {
            string text = "";
            string timeFolder = "/home/cod4/mods/adr_speedrun/map_times/";
            using (StreamReader reader = new StreamReader(
                timeFolder + map + "_fastesttimes_" + way + "_" + speed + ".txt"))
                text = reader.ReadToEnd();
            string[] line = text.Split('\n');

            EmbedBuilder em = new EmbedBuilder
            {
                ThumbnailUrl = "https://cdn.discordapp.com/icons/335075122467700740/8152834be097199cff8d46a2ae1e5588.png",
                Color = new Color(164, 22, 248),
                Title = map + " " + speed + " " + way,
                Description = "\n"
            };

            for (int i = 0; i < line.Length; i++)
            {
                if (i == 20)
                    break;

                string[] tkn = line[i].Split('\\');
                em.Description += "#" + (i + 1) + "‌‌ ‌‌ ‌‌ ‌‌ ‌‌ ‌‌‌‌ ‌‌ " 
                    + RealTime(Convert.ToInt32(tkn[2])) + "‌‌ ‌‌ ‌‌ ‌‌ ‌‌ ‌‌‌‌ ‌‌ " + tkn[3] + "\n";
            }

            em.Build();
            await Context.Channel.SendMessageAsync("", false, em);
        }

        /// <summary>
        /// Convert time origin to readable time.
        /// </summary>
        /// <param name="time">Time origin.</param>
        /// <returns>Readable time string.</returns>
        private static string RealTime(int time)
        {
            int miliseconds = time;
            int minutes = time / 60000;
            miliseconds %= 60000;
            int seconds = miliseconds / 1000;
            miliseconds %= 1000;

            return minutes + ":" + seconds + "." + miliseconds;
        }
    }
}