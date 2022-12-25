using Cambot_3.utils.Admin;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.Modules
{
    public class DevCommands : ModuleBase
    {
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public DevCommands(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            var client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _client = client;
        }

        [Command("test")]
        public async Task DevTestCommand()
        {
            await ReplyAsync("Dev test command");
        }
    }
}
