using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3
{
    public class SlashCommandsHandler
    {

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public SlashCommandsHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitialiseAsync()
        {
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;

            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo info, IInteractionContext context, IResult result)
        {
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo info, IInteractionContext context, IResult result)
        {
            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
        {
            return Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            Console.WriteLine("Interaction found");
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(context, _services);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);

                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
