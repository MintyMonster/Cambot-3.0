using Discord.Net;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Bson;
using System.Threading;
using Cambot_3.Modules;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;

namespace Cambot_3
{
    class Program
    {
        private DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvidor;
        //private IConfiguration _config;
        private string token = "MTA1MTI0NTc5OTE4NDU0Mzc3NA.GQFiD7.ICKPCqtSFjDnYG7ty3vOd_3suwv6X0iESrAYC0"; // "Cambot"
        public Program() => _serviceProvidor = ConfigureServices();
        public static void Main(String[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            
            var client = _serviceProvidor.GetRequiredService<DiscordSocketClient>();
            var slashCommands = _serviceProvidor.GetRequiredService<InteractionService>();

            await _serviceProvidor.GetRequiredService<SlashCommandsHandler>().InitialiseAsync();

            client.Log += async (LogMessage message) =>
            {
                Console.WriteLine($"{DateTime.Now}: {message.Message}");
                await Task.CompletedTask;
            };

            client.Ready += async () =>
            {

                client.Guilds.ToList().ForEach(async x =>
                {
                    await slashCommands.RegisterCommandsToGuildAsync(x.Id);
                });

                Console.WriteLine($"{DateTime.Now} => Cambot is now online!");
                await Task.CompletedTask;
            };

            slashCommands.Log += async (LogMessage message) =>
            {
                Console.WriteLine(message.Message);
                await Task.CompletedTask;
            };

            // Find way to remove slashCommands on loggedOut
            //client.LoggedOut += async () => client.Guilds.ToList().ForEach(async x => await slashCommands.RemoveModuleAsync(await Task.));

            client.JoinedGuild += async (SocketGuild guild) => await slashCommands.RegisterCommandsToGuildAsync(guild.Id);
            

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("Under construction...");

            API.ApiHelper.InitialiseClient();

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
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<SlashCommandsHandler>()
                .AddSingleton<SlashCommands>()
                .BuildServiceProvider();
        }
    }
}
