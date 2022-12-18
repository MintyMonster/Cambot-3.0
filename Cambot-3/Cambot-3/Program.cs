using Cambot_3.Modules;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

namespace Cambot_3
{
    class Program
    {
        private DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvidor;
        private readonly IConfiguration _config;

        //private IConfiguration _config;
        //private string token = "MTA1MTI0NTc5OTE4NDU0Mzc3NA.GQFiD7.ICKPCqtSFjDnYG7ty3vOd_3suwv6X0iESrAYC0"; // "Cambot"
        public Program()
        {
            _serviceProvidor = ConfigureServices();

            _config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "ConfigurationFile.json").Build();
        }
        public static void Main(String[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var client = _serviceProvidor.GetRequiredService<DiscordSocketClient>();
            var slashCommands = _serviceProvidor.GetRequiredService<InteractionService>();

            await _serviceProvidor.GetRequiredService<SlashCommandsHandler>().InitialiseAsync();

            client.Log += async (LogMessage message) =>
            {
                Logger.Debug($"{DateTime.Now}: {message.Message}");
                await Task.CompletedTask;
            };

            client.Ready += async () =>
            {
                
                client.Guilds.ToList().ForEach(async x =>
                {
                    await slashCommands.RegisterCommandsToGuildAsync(x.Id);
                });

                Logger.Low($"{DateTime.Now} => Cambot is now online!");

                await Task.CompletedTask;
            };

            slashCommands.Log += async (LogMessage message) =>
            {
                Logger.Debug(message.Message);
                await Task.CompletedTask;
            };

            // Find way to remove slashCommands on loggedOut
            //client.LoggedOut += async () => client.Guilds.ToList().ForEach(async x => await slashCommands.RemoveModuleAsync(await Task.));

            client.JoinedGuild += async (SocketGuild guild) => await slashCommands.RegisterCommandsToGuildAsync(guild.Id);
            

            await client.LoginAsync(TokenType.Bot, ConfigurationHandler.GetConfigKey("Token"));
            await client.StartAsync();
            await client.SetGameAsync("Under construction...");

            API.ApiHelper.InitialiseClient();
            Logger.Low("Everything is now initialised!");

            await Task.Delay(Timeout.Infinite);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
                    LogGatewayIntentWarnings = false,
                    LogLevel = LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    UseInteractionSnowflakeDate = false
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<SlashCommandsHandler>()
                .AddSingleton<SlashCommands>()
                .BuildServiceProvider();
        }
        
    }
}
