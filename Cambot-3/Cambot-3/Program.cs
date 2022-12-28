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
using Discord.Commands;
using Cambot_3.Services;
using System.Data.SQLite;
using Cambot_3.utils.Logging;

namespace Cambot_3
{
    class Program
    {
        private readonly IServiceProvider _serviceProvidor;
        private readonly IConfiguration _config;
        private Timer _timer;
        private Timer _databaseTimer;

        //private IConfiguration _config;
        public Program()
        {
            _serviceProvidor = ConfigureServices();

            _config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "ConfigurationFile.json").Build();
        }
        public static void Main(String[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() // Main
        {
            try
            {
                var client = _serviceProvidor.GetRequiredService<DiscordSocketClient>(); // Get DiscordSocketclient
                var slashCommands = _serviceProvidor.GetRequiredService<InteractionService>(); // Get SlashCommandHandler service

                await _serviceProvidor.GetRequiredService<SlashCommandsHandler>().InitialiseAsync(); // Initialise Slash commands
                await _serviceProvidor.GetRequiredService<CommandHandler>().InitialiseAsync(); // Initialise text commands

                // Logging
                client.Log += async (LogMessage message) => // Client logging
                {
                    Logger.Info($"Client: {message.Message}");
                    await Task.CompletedTask;
                };

                client.Connected += async () => // Connection event log
                {
                    Logger.Low("Connected to Discord Gateway.");
                    await Task.CompletedTask;
                };

                // Once client is ready
                client.Ready += async () => // Ready event log
                { 
                    client.Guilds.ToList().ForEach(async x =>
                    {
                        await slashCommands.RegisterCommandsToGuildAsync(x.Id); // Register commands for every guild found
                    });
                    Logger.Low($"{DateTime.Now} => Cambot is now online!");
                    await Task.CompletedTask;
                };
                
                client.Disconnected += async (message) => Logger.Fatal($"Cambot Disconnected\n{message.Message}"); // Show when disconnected
                client.LoggedOut += async () => Logger.Fatal("Cambot Logged out"); // Show when logged out

                // Logging for all commands
                slashCommands.Log += async (LogMessage message) => // Log slash commands
                {
                    Logger.Info($"Command: {message.Message}");
                    await Task.CompletedTask;
                };


                // Ensure files exist + log
                if (!File.Exists(AppContext.BaseDirectory + "suggestions.json")) File.Create(AppContext.BaseDirectory + "suggestions.json").Close(); // If not exists, create suggestions.json
                if (!File.Exists(AppContext.BaseDirectory + "bugs.json")) File.Create(AppContext.BaseDirectory + "bugs.json").Close(); // If not exists, create bugs.json

                if (File.Exists(AppContext.BaseDirectory + "suggestions.json")
                    && File.Exists(AppContext.BaseDirectory + "bugs.json")
                    && File.Exists(AppContext.BaseDirectory + "CamCoins.db")
                    && File.Exists(AppContext.BaseDirectory + "PlayersDB.db"))
                    Logger.Low("Found required files..."); // User validation
                else
                    throw new Exception("One or more files seem to be missing. Have they been created?");

                // Save Bugs and Suggestions to JSON every 10 minues
                _timer = new Timer( _ => // Timer to save the Suggestions and bugs to JSON
                {
                    JsonHandler.SendSuggestionsToJson(); // Suggestions pushing handler
                    JsonHandler.SendBugsToJson(); // Bugs pushing handler
                    Logger.Low("Json saved.");

                }, null, (int)TimeSpan.FromMinutes(10).TotalMilliseconds, (int)TimeSpan.FromMinutes(10).TotalMilliseconds);

                LevelsDatabaseHandler.LoadAllPlayers(); // Load all players into a dictionary of PlayerObjects

                _databaseTimer = new Timer( _ => // Push all data saved in the dictionary to database on timer
                {
                    LevelsDatabaseHandler.PushToDatabase();
                    Logger.Low("Players pushed to database");

                }, null, (int)TimeSpan.FromMinutes(10).TotalMilliseconds, (int)TimeSpan.FromMinutes(10).TotalMilliseconds);

                // Upon joining a guild, add 
                client.JoinedGuild += async (SocketGuild guild) => await slashCommands.RegisterCommandsToGuildAsync(guild.Id); // Register all commands upon joining a guild

                await client.LoginAsync(TokenType.Bot, ConfigurationHandler.GetConfigKey("Token")); // Login
                await client.StartAsync(); // Start the bot
                await client.SetGameAsync("Under construction..."); // Set visible game

                API.ApiHelper.InitialiseClient(); // Initialise API handler
            }
            catch(Exception ex) { Logger.Fatal(ex); }
            finally { Logger.Low("Everything is now initialised!"); } 

            await Task.Delay(Timeout.Infinite); // Run until told to
        }

        private IServiceProvider ConfigureServices() // Grab services
        {
            return new ServiceCollection()
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = Discord.GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
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
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
