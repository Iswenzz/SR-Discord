using Discord.Commands;

using System;
using System.Threading.Tasks;

namespace Iswenzz.SR.Discord.Discord.Commands
{
    public class Stop : ModuleBase
    {
        /// <summary>
        /// Stop the Speedrun BOT.
        /// </summary>
        /// <returns></returns>
        [RequireOwner]
        [Command("stop")]
        public async Task StopBot()
        {
            await Task.Delay(1);
            Environment.Exit(0);
        }
    }
}
