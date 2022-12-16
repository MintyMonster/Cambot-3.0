using Cambot_3.API;
using Cambot_3.API.ApiModels;
using Cambot_3.API.ApiModels.InternationalSpaceStation;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.Modules
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }

        // Convert all Unix timestamps from API
        
        public DateTime ConvertUnix(long unix)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unix);
        }

        // Custom embeds styled for CamBot
        public Embed CreateCustomEmbed(string title, string content, string image = null, string credit = null, string creditIconUrl = null, string iconUrl = null, string author = null)
        {
            var footerBuilder = new EmbedFooterBuilder();
            footerBuilder.Text = credit == null ? string.Empty : credit;
            footerBuilder.IconUrl = creditIconUrl == null ? string.Empty : creditIconUrl;
            footerBuilder.Build();
            
            var authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.IconUrl = iconUrl == null ? string.Empty : iconUrl;
            authorBuilder.Name = author == null ? string.Empty : author;
            authorBuilder.Build();
            
            var embed = new EmbedBuilder();
            embed.Title = title;
            embed.Description = content;
            embed.Color = new Color(127, 109, 188);
            embed.ImageUrl = image == null ? string.Empty : image;
            embed.Author = authorBuilder;
            embed.Footer = footerBuilder;
            embed.WithCurrentTimestamp();

            return embed.Build(); ;
        }

        
        //////////////////////////////////////////////////
        //COMMANDS START
        //////////////////////////////////////////////////
        
        // Test command
        [SlashCommand("test", "this is a test command")]
        public async Task TestCommand() => await RespondAsync("This is a test");

        // Dad joke command
        [SlashCommand("dadjoke", "Get a random Dad joke!")]
        public async Task DadJokeCommand()
        {
            DadJokeModel response = await ApiFunctions.GetDadJoke();
            await RespondAsync($"{response.Joke}");
        }

        // International Space Station: Positioning and People
        [SlashCommand("iss", "Get the current position of the International Space Station!")]
        public async Task GetIssPositionCommand()
        {
            IssTimeStamp _timeStamp = await ApiFunctions.GetIssTimeStamp();
            LongLatISS _response = await ApiFunctions.GetIssPosition();
            PeopleRoot _people = await ApiFunctions.GetPeopleInSpace();
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"**Timestamp:** {ConvertUnix(_timeStamp.Timestamp)}\n**Longitude:** {_response.Longitude}\n**Latitude:** {_response.Latitude} \n\n**Astronauts on the ISS:**");
            _people.People.ForEach(x => { if (x.Craft == "ISS") sb.AppendLine($"- {x.Name}"); }); 
            
            await RespondAsync(null, null, false, 
                embed: CreateCustomEmbed("ISS Position: ", sb.ToString(), 
                null, "open-notify.org", null, Context.User.GetAvatarUrl(ImageFormat.Png, 128), Context.User.Username));
        }
    }
}

