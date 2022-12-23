﻿using Cambot_3.utils.Levels;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly PlayerLevelsEntities _db;

        public SlashCommandsHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
            _db = _services.GetRequiredService<PlayerLevelsEntities>();
        }

        public async Task InitialiseAsync()
        {
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;

            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo info, IInteractionContext context, IResult result) => Task.CompletedTask;

        private Task ContextCommandExecuted(ContextCommandInfo info, IInteractionContext context, IResult result) => Task.CompletedTask;

        private Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result) =>Task.CompletedTask;

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);

                // Levelling code
                LevelsDatabaseHandler.HandleCommandExperience(_client.GetUser(context.User.Id).Id, _client.GetUser(context.User.Id).Username);
                Logger.Error(_client.GetUser(context.User.Id).Id.ToString());

                await _commands.ExecuteCommandAsync(context, _services);
            }
            catch(Exception ex)
            {
                Logger.Fatal(ex);

                await arg.RespondAsync("Something went wrong... Please try again!");

                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}