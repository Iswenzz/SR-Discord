using System;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Iswenzz.SR.Discord
{
    public class Program
    {
        private CommandService Commands { get; set; }
        private DiscordSocketClient Client { get; set; }
        private IServiceProvider Services { get; set; }

        public static void Main() => new Program().Start().GetAwaiter().GetResult();

        /// <summary>
        /// Start the discord BOT.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            Client = new DiscordSocketClient();
            Commands = new CommandService();
            Services = new ServiceCollection().BuildServiceProvider();
            string token_path = AppContext.BaseDirectory + "token";

            if (!File.Exists(token_path))
                throw new FileNotFoundException($"File '{token_path}' is missing.");

            await InstallCommands();
            await Client.LoginAsync(TokenType.Bot, File.ReadAllText(token_path));
            await Client.StartAsync();

            Console.WriteLine("Speedrun BOT - Started");

            await Task.Delay(-1);
        }

        /// <summary>
        /// Setup commands callback.
        /// </summary>
        /// <returns></returns>
        private async Task InstallCommands()
        {
            Client.MessageReceived += HandleCommand;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Commands callback (process user messages).
        /// </summary>
        /// <param name="messageParam">Message from user.</param>
        /// <returns></returns>
        private async Task HandleCommand(SocketMessage messageParam)
        {
            int argPos = 0;
            SocketUserMessage message = messageParam as SocketUserMessage;

            if (message == null || message.Channel.Id != 335742969753632788)
                return;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos)))
                return;

            CommandContext context = new CommandContext(Client, message);
            IResult result = await Commands.ExecuteAsync(context, argPos, Services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}