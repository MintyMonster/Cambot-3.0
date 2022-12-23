using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using Discord.WebSocket;

namespace Cambot_3.utils
{
    public class JsonHandler
    {

        private static List<SuggestionDetails> suggestionCache = new List<SuggestionDetails>();
        private static List<BugDetails> bugCache = new List<BugDetails>();
        private static string suggestPath = AppContext.BaseDirectory + "suggestions.json";
        private static string bugsPath = AppContext.BaseDirectory + "bugs.json";

        public static void AddSuggestion(SuggestionDetails suggestion) => suggestionCache.Add(suggestion);
        public static void AddBug(BugDetails bug) => bugCache.Add(bug);

        public static void SendSuggestionsToJson()
        {
            var _data = File.ReadAllText(suggestPath);
            var _returnList = JsonConvert.DeserializeObject<List<SuggestionDetails>>(_data) ?? new List<SuggestionDetails>();

            suggestionCache.ForEach(x => _returnList.Add(x));
            suggestionCache.Clear();

            _data = JsonConvert.SerializeObject(_returnList);
            File.WriteAllText(suggestPath, _data);
        }

        public static void SendBugsToJson()
        {
            var _data = File.ReadAllText(bugsPath);
            var _returnList = JsonConvert.DeserializeObject<List<BugDetails>>(_data) ?? new List<BugDetails>();

            bugCache.ForEach(x => _returnList.Add(x));
            suggestionCache.Clear();

            _data = JsonConvert.SerializeObject(_returnList);
            File.WriteAllText(bugsPath, _data);
        }
    }
}
