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
using System.IO;
using Cambot_3.utils.JSON;
using Cambot_3.utils.Levels;

namespace Cambot_3
{
    class Program
    {
        private DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvidor;
        private readonly IConfiguration _config;
        private Timer _timer;
        private Timer _databaseTimer;

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
            try
            {
                var client = _serviceProvidor.GetRequiredService<DiscordSocketClient>();
                var slashCommands = _serviceProvidor.GetRequiredService<InteractionService>();

                await _serviceProvidor.GetRequiredService<SlashCommandsHandler>().InitialiseAsync();

                // Logging
                client.Log += async (LogMessage message) =>
                {
                    Logger.Info($"Client: {message.Message}");
                    await Task.CompletedTask;
                };

                client.Connected += async () =>
                {
                    Logger.Low("Connected to Discord Gateway.");
                    await Task.CompletedTask;
                };

                // Once client is ready
                client.Ready += async () =>
                { 
                    client.Guilds.ToList().ForEach(async x =>
                    {
                        await slashCommands.RegisterCommandsToGuildAsync(x.Id);
                    });
                    Logger.Low($"{DateTime.Now} => Cambot is now online!");
                    await Task.CompletedTask;
                };

                // Logging for all commands
                slashCommands.Log += async (LogMessage message) =>
                {
                    Logger.Info($"Command: {message.Message}");
                    await Task.CompletedTask;
                };


                // Ensure files exist + log
                if (!File.Exists(AppContext.BaseDirectory + "suggestions.json")) File.Create(AppContext.BaseDirectory + "suggestions.json").Close();
                if (!File.Exists(AppContext.BaseDirectory + "bugs.json")) File.Create(AppContext.BaseDirectory + "bugs.json").Close();
                if (File.Exists(AppContext.BaseDirectory + "suggestions.json") && File.Exists(AppContext.BaseDirectory + "bugs.json")) Logger.Low("Found required files...");

                // Save Bugs and Suggestions to JSON every 10 minues
                _timer = new Timer( _ =>
                {
                    JsonHandler.SendSuggestionsToJson();
                    JsonHandler.SendBugsToJson();
                    Logger.Low("Json saved.");

                }, null, (int)TimeSpan.FromMinutes(10).TotalMilliseconds, (int)TimeSpan.FromMinutes(10).TotalMilliseconds);

                LevelsDatabaseHandler.LoadAllPlayers();

                _databaseTimer = new Timer(_ =>
                {
                    LevelsDatabaseHandler.PushToDatabase();
                    Logger.Low("Players pushed to database");

                }, null, (int)TimeSpan.FromSeconds(25).TotalMilliseconds, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);

                // Upon joining a guild, add 
                client.JoinedGuild += async (SocketGuild guild) => await slashCommands.RegisterCommandsToGuildAsync(guild.Id);

                await client.LoginAsync(TokenType.Bot, ConfigurationHandler.GetConfigKey("Token"));
                await client.StartAsync();
                await client.SetGameAsync("Under construction...");

                API.ApiHelper.InitialiseClient();
            }
            catch(Exception ex) { Logger.Fatal(ex); }
            finally { Logger.Low("Everything is now initialised!"); }

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
                .AddSingleton<JsonHandler>()
                .AddSingleton<PlayerLevelsEntities>()
                .AddSingleton<LevelsDatabaseHandler>()
                .BuildServiceProvider();
        }
        
    }
}
