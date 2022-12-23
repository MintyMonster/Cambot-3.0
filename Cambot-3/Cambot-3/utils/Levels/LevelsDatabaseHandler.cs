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
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Cambot_3.utils.Levels
{
    public class LevelsDatabaseHandler
    {
        private static readonly PlayerLevelsEntities _db = new PlayerLevelsEntities();
        private static Dictionary<ulong, PlayerObject> Players = new Dictionary<ulong, PlayerObject>();

        public static double GetPlayerExperience(SocketUser user) => Players[user.Id].Experience;
        public static int GetPlayerLevel(SocketUser user) => Players[user.Id].Level;

        public static void HandleCommandExperience(ulong id, string username, SocketInteractionContext context)
        {
            if (!(Players.ContainsKey(id)))
                Players.Add(id, new PlayerObject() { ID = id, Username = username, Experience = 15, Level = 1 });
            else
            {
                double levelXp = Players[id].Level + (Players[id].Level * 100) * 1.2;

                if((Players[id].Experience + 15) >= levelXp)
                {
                    Players[id].Level += 1;
                    Players[id].Experience = ((Players[id].Experience + 15) - levelXp);
                    SendLevelUp(context);
                    Logger.Medium($"Player {Players[id].Username}, level: {Players[id].Level}, experience: {Players[id].Experience}");
                }
                else
                {
                    Players[id].Experience += 15;
                    Logger.Medium($"Player {Players[id].ID} {Players[id].Username}, experience added: {Players[id].Experience}");
                }
            }

            Players.ToList().ForEach(x => Logger.Medium(x.Key.ToString()));

        }

        public static void LoadAllPlayers()
        {
            Logger.Low("Loading players....");
            try
            {
                foreach (var player in _db.PlayersDB)
                {
                    if (player != null && _db.PlayersDB.Count() != 0)
                    {
                        Players.Add((ulong)player.UserID, new PlayerObject()
                        {
                            ID = (ulong)player.UserID,
                            Username = player.Username,
                            Level = player.Level,
                            Experience = player.Experience
                        });
                    }
                    else
                    {
                        Logger.Medium("No players exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
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
                    await _db.PlayersDB.AddAsync(new LevelsLeaderboardModel()
                    {
                        UserID = p.Value.ID, // Weird id?
                        Username = p.Value.Username,
                        Experience = p.Value.Experience,
                        Level = p.Value.Level
                    });
                    Logger.Low($"Player added, {p.Key}, {p.Value.Username}, {p.Value.Experience}, {p.Value.Level}");
                }
            }

            await _db.SaveChangesAsync();
        }

        public static string ExperienceBar(SocketUser user)
        {
            StringBuilder sb = new StringBuilder();
            double levelXp = Players[user.Id].Level + (Players[user.Id].Level * 100) * 1.2;
            double percentage = (Players[user.Id].Experience / levelXp) * 100;
            sb.Append("`[");

            for (double i = 0; i <= 100; i += 5)
            {
                if (i <= percentage) sb.Append("/");
                else sb.Append("-");
            }

            sb.Append($"]` **{Math.Round(percentage)}%**");
            return sb.ToString();
        }

        public static async void SendLevelUp(SocketInteractionContext context)
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
            embed.Description = $"{context.User.Username} just leveled up to **level {GetPlayerLevel(context.User)}**!\nIt's **{Players[context.User.Id].Level + (Players[context.User.Id].Level * 100) * 1.2} experience** to **level {GetPlayerLevel(context.User) + 1}**!";
            embed.Color = new Color(127, 109, 188);
            embed.Author = author;
            embed.Footer = footer;
            embed.WithCurrentTimestamp();

            await context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
