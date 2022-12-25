using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.Services
{
    public  class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;


        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += async (command, context, result) =>
            {
                if (!command.IsSpecified) Logger.Error($"Command failed - {context.User.Username}");
                if (result.IsSuccess) Logger.Low($"Command succes - {context.User.Username}");
            };

            _client.MessageReceived += async (rawMessage) =>
            {
                try
                {
                    SocketCommandContext context = new SocketCommandContext(_client, rawMessage as SocketUserMessage);
                    SocketUserMessage message = rawMessage as SocketUserMessage;
                    int argPos = 0;
                    char prefix = '^';

                    if (!(rawMessage is SocketUserMessage)) return;
                    if (message.Source != MessageSource.User) return;
                    if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos))) return;
                    if (context.User != _client.GetUser(ulong.Parse(ConfigurationHandler.GetConfigKey("DevKey")))) return; // Ensure it's only dev use

                    Logger.Error("Admin command used.");
                    await _commands.ExecuteAsync(context, argPos, _services);
                }catch(Exception ex)
                {
                    Logger.Fatal(ex);
                }
                
            };
        }

        public async Task InitialiseAsync()
        {
            Logger.Low("Commands initialising...");
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), _services);
            Logger.Low("Commands initialised!");
        }

    }
}
