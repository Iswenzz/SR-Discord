using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace Iswenzz.SR.Discord.Discord.Commands
{
    public class Help : ModuleBase
    {
        /// <summary>
        /// Send a <see cref="EmbedBuilder"/> helper message.
        /// </summary>
        [Command("help")]
        public async Task HelpMessage()
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
    }
}
