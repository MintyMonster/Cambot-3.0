using Cambot_3.utils.Logging;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.utils.Levels
{
    public class LevelsDatabaseHandler
    {
        private static readonly PlayerLevelsEntities _db = new PlayerLevelsEntities();
        private static Dictionary<ulong, PlayerObject> Players = new Dictionary<ulong, PlayerObject>();

        // Private
        private static bool PlayerExists(ulong id) => Players.ContainsKey(id);
        private static void CreatePlayer(IInteractionContext context) => 
            Players.Add(context.User.Id, new PlayerObject() { ID = context.User.Id, Username = context.User.Username, Experience = 15, Level = 1 });
        private static void CreatePlayer(ulong id, string username, int level, double experience) => 
            Players.Add(id, new PlayerObject() { ID = id, Username = username, Experience = experience, Level = level });
        private static void AddExperience(ulong id) => Players[id].Experience += 7;
        private static void SetExperience(ulong id, double levelXp) => Players[id].Experience = ((Players[id].Experience + 7) - levelXp);
        private static void AddLevel(ulong id) => Players[id].Level++;
        private static double GetLevelExperience(ulong id) => Players[id].Level + (Players[id].Level * 100) * 1.2;
        private static async void AddToDatabase(ulong id, string username, int level, double experience) =>
            await _db.PlayersDB.AddAsync(new LevelsLeaderboardModel() { UserID = id, Username = username, Experience = experience, Level = level });
        // Public
        public static double GetPlayerExperience(ulong id) => PlayerExists(id) ? Players[id].Experience : 15;
        public static int GetPlayerLevel(ulong id) => PlayerExists(id) ? Players[id].Level : 1;
        public static int GetPlayerCount() => Players.Count;

        public static void ResetPlayers()
        {
            Players.ToList().ForEach(x =>
            {
                x.Value.Level = 1;
                x.Value.Experience = 0;
                Logger.Error($"{x.Value.Username} purged");
            });
        }

        public static void HandleCommandExperience(ulong id, string username, IInteractionContext context)
        {

            if (!PlayerExists(id)) CreatePlayer(context);
            else
            {
                if(GetPlayerExperience(id) + 7 >= GetLevelExperience(id))
                {
                    SetExperience(id, GetLevelExperience(id));
                    AddLevel(id);
                    SendLevelUp(context);
                    Logger.Medium($"Player {Players[id].Username}, level: {Players[id].Level}, experience: {Players[id].Experience}");
                }
                else
                {
                    AddExperience(id);
                    Logger.Medium($"Player {Players[id].ID} {Players[id].Username}, experience added: {Players[id].Experience}");

                }
            }
        }

        public static void LoadAllPlayers()
        {
            Logger.Low("Loading players....");
            try
            {
                foreach (var player in _db.PlayersDB)
                    if (player != null && _db.PlayersDB.Count() > 0)
                        CreatePlayer(player.UserID, player.Username, player.Level, player.Experience);
            }catch(Exception e)
            {
                Logger.Fatal(e.Message);
            }
            Logger.Low("Players loaded!");
        }

        

        public static async void PushToDatabase()
        {
            foreach (var p in Players)
            {
                if ((_db.PlayersDB.Where(x => p.Value.ID == (ulong)x.UserID).Any()))
                {
                    var player = _db.PlayersDB.Where(x => (ulong)x.UserID == p.Value.ID).FirstOrDefault();
                    player.Level = p.Value.Level;
                    player.Experience = p.Value.Experience;
                }
                else
                {
                    AddToDatabase(p.Value.ID, p.Value.Username, p.Value.Level, p.Value.Experience);
                    Logger.Low($"Player added, {p.Key}, {p.Value.Username}, {p.Value.Experience}, {p.Value.Level}");
                }
            }

            await _db.SaveChangesAsync();
        }

        public static string ExperienceBar(SocketUser user)
        {
            StringBuilder sb = new StringBuilder();
            
            double percentage = (Players[user.Id].Experience / GetLevelExperience(user.Id)) * 100;
            sb.Append("`[");

            for (double i = 0; i <= 200; i += 5)
            {
                if (i <= (percentage * 2)) sb.Append("/");
                else sb.Append("-");
            }

            sb.Append($"]` **{Math.Round(percentage)}%**");
            return sb.ToString();
        }

        public static async void SendLevelUp(IInteractionContext context)
        {
            var footer = new EmbedFooterBuilder();
            footer.Text = "Cambot";
            footer.Build();

            var author = new EmbedAuthorBuilder();
            author.IconUrl = context.User.GetAvatarUrl(ImageFormat.Png, 128);
            author.Name = context.User.Username;
            author.Build();

            var embed = new EmbedBuilder();
            embed.Title = "Level up!";
            embed.Description = $"{context.User.Username} just leveled up to **level {GetPlayerLevel(context.User.Id)}**!\nIt's **{Players[context.User.Id].Level + (Players[context.User.Id].Level * 100) * 1.2} experience** to **level {GetPlayerLevel(context.User.Id) + 1}**!";
            embed.Color = new Color(127, 109, 188);
            embed.Author = author;
            embed.Footer = footer;
            embed.WithCurrentTimestamp();

            await context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        public static string GetLeaderBoardDescending()
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;

            foreach(var player in Players.OrderByDescending(x => x.Value.Level))
            {
                if (i >= 26) break;
                string place = i == 1 ? ":first_place:" : i == 2 ? ":second_place:" : i == 3 ? ":third_place:" : $"{i.ToString()})";
                sb.AppendLine($"{place} **{player.Value.Username} - Level {player.Value.Level}** ({player.Value.Experience}xp)");
                i++;
            }

            return sb.ToString();
        }
    }
}
