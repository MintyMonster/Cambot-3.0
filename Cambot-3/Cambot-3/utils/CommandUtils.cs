using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cambot_3.utils
{
    public class CommandUtils
    {
        public class CommandType
        {
            public string name { get; set; }
            public CommandParams parameter { get; set; } // none, req, opt
            public string emoji { get; set; }
            public string title { get; set; }
            public string usage { get; set; }
        };

        public enum CommandParams
        {
            None,
            Optional,
            Required
        }

        public enum CommandName
        {
            Help,
            Dadjoke,
            Apod,
            Iss,
            Mars,
            Yearfact,
            Mathfact,
            Weather,
            Cat,
            Dog,
            Fox,
            Raccoon,
            Redpanda,
            TopCrypto,
            Crypto,
            Catfact,
            Plants,
            Hug,
            Stats,
            Spacepeople,
            Add,
            Suggestion,
            Bug,
            Info,
            Roll,
            Level,
            Leaderboard
        }

        private static Dictionary<CommandName, CommandType> commands = new Dictionary<CommandName, CommandType>()
        {
            { CommandName.Help, new CommandType { name = "help", parameter = CommandParams.Optional, emoji = ":newspaper:", title = "You've already found me, silly!", usage = "Usage: **/help [*Optional: Command Name*]**" } },
            { CommandName.Dadjoke, new CommandType { name = "dadjoke", parameter = CommandParams.None, emoji = ":rofl:", title = "In need of a random dad joke? :rofl:", usage = "Usage: **/dadjoke**"} },
            { CommandName.Apod, new CommandType { name = "apod", parameter = CommandParams.None, emoji = ":first_quarter_moon_with_face:", title = "Get the astronomy picture of the day! :first_quarter_moon_with_face:", usage = "Usage: **/apod**"} },
            { CommandName.Iss, new CommandType { name = "iss", parameter = CommandParams.None, emoji = ":first_quarter_moon_with_face:", title = "Where's the international space station? :first_quarter_moon_with_face:", usage = "Usage: **/iss**"} },
            { CommandName.Mars, new CommandType { name = "mars", parameter = CommandParams.None, emoji = ":ringed_planet:", title = "Wow! Pictures of Mars! :ringed_planet:", usage = "Usage: **/mars**" } },
            { CommandName.Yearfact, new CommandType { name = "yearfact", parameter = CommandParams.Optional, emoji = ":calendar:", title = "Where did 2020 even go?! :calendar:", usage = "Usage: **/yearfact [*Optional: year*]**"} },
            { CommandName.Mathfact, new CommandType { name = "mathfact", parameter = CommandParams.Optional, emoji = ":1234:", title = "Some say numbers are boring... :1234:", usage = "Usage: **/mathfact [*Optional: number*]**"} },
            { CommandName.Weather, new CommandType { name = "weather", parameter = CommandParams.Required, emoji = ":cloud_rain:", title = $"Why's it always raining... :cloud_rain:", usage = "Usage: **/weather [*Required: City*]**" } },
            { CommandName.Cat, new CommandType { name = "cat", parameter = CommandParams.None, emoji = ":cat:", title = "Feline friends! :cat:", usage = "Usage: **/cat**"} },
            { CommandName.Dog, new CommandType { name = "dog", parameter = CommandParams.None, emoji = ":dog:", title = "Woofers! :dog:", usage = "Usage: **/dog**"} },
            { CommandName.Fox, new CommandType { name = "fox", parameter = CommandParams.None, emoji = ":fox:", title = "Pandora's fox. Haha, get it? :fox:", usage = "Usage: **/fox**"} },
            { CommandName.Raccoon, new CommandType { name = "Raccoon", parameter = CommandParams.None, emoji = ":raccoon:", title = "Trash pandas! :raccoon:", usage = "Usage: **/raccoon**"} },
            { CommandName.Redpanda, new CommandType { name = "redpanda", parameter = CommandParams.None, emoji = ":panda_face:", title = "These are bear-y cute! :panda_face:", usage = "Usage: **/redpanda**"} },
            { CommandName.TopCrypto, new CommandType { name = "topcrypto", parameter = CommandParams.None, emoji = ":dollar:", title = "The top 25 cryptocurrencies right now! :dollar:", usage = "Usage: **/topcrypto**"} },
            { CommandName.Crypto, new CommandType { name = "crypto", parameter = CommandParams.Optional, emoji = ":dollar:", title = "Want to know about a specific coin? :dollar:", usage = "Usage: **/crypto [*Optional: name, Default: Bitcoin*]**"} },
            { CommandName.Catfact, new CommandType { name = "catfact", parameter = CommandParams.None, emoji = ":cat:", title = "Feline friend facts! :cat:", usage = "Usage: **/catfact**"} },
            { CommandName.Plants, new CommandType { name = "plants", parameter = CommandParams.Optional, emoji = ":potted_plant:", title = "Plants! :potted_plant:", usage = "Usage: **/plants [*Optional: plantName*]**"} },
            { CommandName.Stats, new CommandType { name = "stats", parameter = CommandParams.None, emoji = ":bar_chart:", title = "How's Cambot doing? :bar_chart:", usage = "Usage: **/stats**"} },
            { CommandName.Spacepeople, new CommandType { name = "spacepeople", parameter = CommandParams.None, emoji = ":astronaut:", title = "Who's in space right now? :astronaut:", usage = "Usage: **/spacepeople**"} },
            { CommandName.Add, new CommandType { name = "add", parameter = CommandParams.None, emoji = ":arrow_right:", title = "Yay, more friends!", usage = "Usage: **/add**" } },
            { CommandName.Suggestion, new CommandType { name = "suggestion", parameter = CommandParams.Required, emoji = ":white_check_mark:", title = "Have a suggestion?", usage = "Usage: **/suggestion [*Required: title, idea*]**"} },
            { CommandName.Bug, new CommandType { name = "bugsplat", parameter = CommandParams.Required, emoji = ":x:", title = "Found a bug?", usage = "Usage: **/bugsplat [*Required: title, bug*]**"} },
            { CommandName.Info, new CommandType { name = "info", parameter = CommandParams.None, emoji = ":grey_question:", title = "Want to learn more about me? :grey_question:", usage = "Usage: **/info**" } },
            { CommandName.Roll, new CommandType { name = "roll", parameter = CommandParams.Required, emoji = ":100:", title = "Random numbers without the dice! :100:", usage = "Usage: **/roll [*Required: range*]**"} },
            { CommandName.Level, new CommandType { name = "level", parameter = CommandParams.Optional, emoji = ":arrow_up_small:", title = "Want to see your progression? :arrow_up_small:", usage = "Usage: **/level [*Optional: user*]**"} },
            { CommandName.Leaderboard, new CommandType { name = "leaderboard", parameter = CommandParams.None, emoji = ":signal_strength:", title = "Want to see the top levels? :signal_strength:", usage = "Usage: **/leaderboard**"} }
        };

        public static Dictionary<CommandName, CommandType> GetCommands()
        {
            return commands;
        }

    }
}
