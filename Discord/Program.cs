using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

namespace SR.Discord
{
    /// <summary>
    /// The application.
    /// </summary>
    public static class Program
    {
        private static ServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Start the discord bot.
        /// </summary>
        public static async Task Main()
        {
            string token = AppContext.BaseDirectory + ".token";

            if (!File.Exists(token))
                throw new FileNotFoundException($"File '{token}' is missing.");

            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMessages,
                AlwaysDownloadUsers = true,
            };

            ServiceProvider = new ServiceCollection()
               .AddSingleton(config)
               .AddSingleton<DiscordSocketClient>()
               .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
               .BuildServiceProvider();

            var client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
            var interactionService = ServiceProvider.GetRequiredService<InteractionService>();

            client.Ready += OnReady;
            client.InteractionCreated += OnInteraction;
            client.Log += OnLog;

            await client.LoginAsync(TokenType.Bot, File.ReadAllText(token));
            await client.StartAsync();
            await interactionService.AddModulesAsync(typeof(Program).Assembly, ServiceProvider);

            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Ready callback.
        /// </summary>
        /// <returns></returns>
        private static async Task OnReady()
        {
            await ServiceProvider.GetRequiredService<InteractionService>().RegisterCommandsGloballyAsync();
        }

        /// <summary>
        /// On socket interaction.
        /// </summary>
        /// <param name="interaction">The interaction.</param>
        /// <returns></returns>
        private static async Task OnInteraction(SocketInteraction interaction)
        {
            var interactionService = ServiceProvider.GetRequiredService<InteractionService>();
            var client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
            var ctx = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        /// <summary>
        /// Log callback.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private static Task OnLog(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
