using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.utils.CommandStuffs
{
    public class CommandsUsedLeaderboardHandler
    {
        private static CommandsUsedLeaderboardHandler _cmdInstance;
        private CommandsUsedLeaderboardHandler() { }

        public static CommandsUsedLeaderboardHandler Instance
        {
            get { return _cmdInstance ?? new CommandsUsedLeaderboardHandler(); } // "an item with the same key has already been added" Exception
        }

        private static Dictionary<CommandName, int> _commands = new Dictionary<CommandName, int>()
        {
            { CommandName.Help, 0 },
            { CommandName.Dadjoke, 0 },
            { CommandName.Apod, 0 },
            { CommandName.Iss, 0 },
            { CommandName.Mars, 0 },
            { CommandName.Yearfact, 0 },
            { CommandName.Mathfact, 0 },
            { CommandName.Weather, 0 },
            { CommandName.Cat, 0 },
            { CommandName.Dog, 0 },
            { CommandName.Fox, 0 },
            { CommandName.Raccoon, 0 },
            { CommandName.Redpanda, 0 },
            { CommandName.TopCrypto, 0 },
            { CommandName.Crypto, 0 },
            { CommandName.Catfact, 0 },
            { CommandName.Plants, 0 },
            { CommandName.Hug, 0 },
            { CommandName.Stats, 0 },
            { CommandName.Spacepeople, 0 },
            { CommandName.Add, 0 },
            { CommandName.Suggestion, 0 },
            { CommandName.Bug, 0 },
            { CommandName.Bug, 0 },
            { CommandName.Info, 0 },
            { CommandName.Roll, 0 },
            { CommandName.Level, 0 },
            { CommandName.Leaderboard, 0 }
        };

        public void AddCommandUse(CommandName name) => _commands[name] += 1;
        public int Get(CommandName name) => _commands[name];
        public Dictionary<CommandName, int> GetDict() => _commands;

        public string GetCommandsUsedDescending()
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;

            foreach (var command in _commands.OrderByDescending(x => x.Value))
            {
                string place = i == 1 ? ":first_place:" : i == 2 ? ":second_place:" : i == 3 ? ":third_place:" : $"{i.ToString()})";
                sb.AppendLine($"{place} **{command.Key.ToString()}** - {command.Value} uses");
                i++;
            }

            return sb.ToString();
        }
    }
}
