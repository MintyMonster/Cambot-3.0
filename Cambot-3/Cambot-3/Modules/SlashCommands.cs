using Cambot_3.API;
using Cambot_3.API.ApiModels;
using Cambot_3.API.ApiModels.InternationalSpaceStation;
using Cambot_3.API.ApiModels.Trefle;
using Cambot_3.utils;
using Cambot_3.utils.JSON;
using Cambot_3.utils.Levels;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.Modules
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private Dictionary<CommandUtils.CommandName, CommandUtils.CommandType> _commandDict;

        public SlashCommands(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _commandDict = CommandUtils.GetCommands();
        }

        // For weather for the Americans
        private string ConvertToFahrenheit(float temp) => ((temp * 1.8) + 32).ToString("N1");

        // Convert all Unix timestamps from API
        private DateTime ConvertUnix(long unix) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix);

        // Get random hugging GIF
        private string GetRandomHugGif()
        {
            List<string> gifs = new List<string>()
            {
                "https://media.giphy.com/media/od5H3PmEG5EVq/giphy.gif",
                "https://media.giphy.com/media/lrr9rHuoJOE0w/giphy.gif",
                "https://media.giphy.com/media/PHZ7v9tfQu0o0/giphy.gif",
                "https://media.giphy.com/media/IRUb7GTCaPU8E/giphy.gif",
                "https://media.giphy.com/media/BXrwTdoho6hkQ/giphy.gif",
                "https://media.giphy.com/media/ZQN9jsRWp1M76/giphy.gif",
                "https://media.giphy.com/media/118TCy0aWhHLOM/giphy.gif",
                "https://media.giphy.com/media/EGauSkKQZuXxS/giphy.gif",
                "https://media.giphy.com/media/l2QDM9Jnim1YVILXa/giphy.gif"
            };

            return gifs[new Random().Next(0, gifs.Count)];
        }

        private string GetRandomOshawott()
        {
            List<string> gifs = new List<string>()
            {
                "https://tenor.com/en-GB/view/oshawott-pokemon-bulbasaur-totodile-gif-4756214",
                "https://tenor.com/en-GB/view/oshawott-in-love-heart-eyes-gif-20060141",
                "https://tenor.com/en-GB/view/oshawott-pokemon-gif-22740673",
                "https://tenor.com/en-GB/view/oshawott-hug-gif-25713780",
                "https://tenor.com/en-GB/view/oshawott-gif-19406357",
                "https://tenor.com/en-GB/view/pokemon-oshawott-gif-22740584",
                "https://tenor.com/en-GB/view/pokemon-oshawott-gif-24644605"
            };

            return gifs[new Random().Next(0, gifs.Count)];
        }

        // Custom embeds styled for CamBot
        private Embed CreateCustomEmbed(string title = null, string content = null, string image = null, string credit = null, SocketUser user = null, bool timestamp = true)
        {
            var footerBuilder = new EmbedFooterBuilder();
            footerBuilder.Text = credit ?? string.Empty;
            footerBuilder.Build();

            var authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.IconUrl = user.GetAvatarUrl(ImageFormat.Png, 128) ?? Context.Guild.IconUrl;
            authorBuilder.Name = user.Username ?? string.Empty;
            authorBuilder.Build();

            var embed = new EmbedBuilder();
            embed.Title = title ?? string.Empty;
            embed.Description = content ?? string.Empty;
            embed.Color = new Color(127, 109, 188);
            if (image != null) embed.ImageUrl = image;
            embed.Author = authorBuilder;
            embed.Footer = footerBuilder;
            if (timestamp) embed.WithCurrentTimestamp();

            return embed.Build();
        }

        private Embed CreateCustomEmbed(string title, string content, SocketUser user = null)
        {
            var authorBuilder = new EmbedAuthorBuilder();
            authorBuilder.IconUrl = user.GetAvatarUrl(ImageFormat.Png, 128) ?? Context.Guild.IconUrl;
            authorBuilder.Name = user.Username ?? string.Empty;
            authorBuilder.Build();

            var embed = new EmbedBuilder();
            embed.Title = title;
            embed.Description = content;
            embed.Color = new Color(127, 109, 188);
            embed.Author = authorBuilder;

            return embed.Build();
        }

        private Embed CreateCustomEmbed(string title, string content, Color color)
        {

            var embed = new EmbedBuilder();
            embed.Title = title;
            embed.Description = content;
            embed.Color = color;

            return embed.Build();
        }

        private Embed CreateCustomEmbed(string title, string content)
        {
            var embed = new EmbedBuilder();
            embed.Title = title;
            embed.Description = content;

            return embed.Build();
        }


        //////////////////////////////////////////////////
        // COMMANDS START
        //////////////////////////////////////////////////

        // Help command
        [SlashCommand("help", "Need some help?")]
        public async Task GetHelp([Remainder] string command = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                StringBuilder sbNo = new StringBuilder();
                StringBuilder sbReq = new StringBuilder();
                StringBuilder sbOpt = new StringBuilder();

                _commandDict.ToList().ForEach(x =>
                {
                    if (x.Value.parameter == CommandUtils.CommandParams.None) sbNo.AppendLine($"\t**- {x.Value.emoji} {x.Value.name}**");
                    else if (x.Value.parameter == CommandUtils.CommandParams.Optional) sbOpt.AppendLine($"\t**- {x.Value.emoji} {x.Value.name}**");
                    else sbReq.AppendLine($"\t**- {x.Value.emoji} {x.Value.name}**");

                });

                await RespondAsync(embed: CreateCustomEmbed(title: "All the commands!", content: $"Cambot has some pretty awesome commands!\nHere's a list of them:\n\n___Commands with no parameters:___\n{sbNo}" +
                    $"\n___Commands with optional parameters:___\n{sbOpt}\n___Commands with required parameters:___\n{sbReq}\n\nWant to know about a specific command?\nExample: **/help weather**",user: Context.User));
            } 
            else
            {
                string title = string.Empty;
                string content = string.Empty;

                _commandDict.ToList().ForEach(x =>
                {
                    if (x.Value.name == command)
                    {
                        title = x.Value.title;
                        content = x.Value.usage;
                    }
                    else
                    {
                        title = ":x: Command not found...";
                        content = $"**{command}** wasn't found. Are you sure you spelt it correctly?";
                    }
                });

                await RespondAsync(embed: CreateCustomEmbed(title, content, Context.User));
            }
        }

        [SlashCommand("info", "Want to learn more about me?")]
        public async Task GetInfo()
            => await RespondAsync(embed: CreateCustomEmbed(title: "Who am I?", content: "I am Cambot. My entire purpose is to provide you, my friend, with random information from the world of the internet! " +
                "I am currently in beta, and therefore I am not the final product that the world desires, but alas, " +
                "my developer is working hard everyday to make sure I am up to date and getting new and creative ways to feed you information\n\nTrouble getting started? Use **/help** to figure me out!\nWant to add me to your server? **/add**", user: Context.User));

        // Suggestion command
        [SlashCommand("suggestion", "Have an idea for Cambot? Send away!")]
        public async Task GetSuggestion(string title, string idea)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(idea)) await RespondAsync($"Please include a {(string.IsNullOrEmpty(title) ? "**title**" : "**idea**").ToString()}.");

            try
            {
                JsonHandler.AddSuggestion(new SuggestionDetails()
                {
                    title = title,
                    description = idea,
                    userName = Context.User.Username,
                    id = Context.User.Id,
                    guild = Context.Guild.Name,
                    guildId = Context.Guild.Id,
                    currentTime = DateTime.Now
                });

                await RespondAsync(embed: CreateCustomEmbed(title: $"Thanks, {Context.User.Username}!", content: "Thank you for your suggestion!", user: Context.User));
                await Context.Client.GetUserAsync(ulong.Parse(ConfigurationHandler.GetConfigKey("UserID"))).Result.SendMessageAsync(embed: CreateCustomEmbed(title: title, content: idea + $"\n\n**From:** {Context.User.Username}\n**Guild:** {Context.Guild.Name}", color: new Color(255, 255, 0)));
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }

        // Bugsplat
        [SlashCommand("bugsplat", "Found a bug?")]
        public async Task BugSplat(string title, string bug)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(bug)) await RespondAsync($"Please include a {(string.IsNullOrEmpty(title) ? "**title**" : "**bug**").ToString()}.");

            try
            {
                JsonHandler.AddBug(new BugDetails()
                {
                    title = title,
                    description = bug,
                    userName = Context.User.Username,
                    id = Context.User.Id,
                    guild = Context.Guild.Name,
                    guildId = Context.Guild.Id,
                    currentTime = DateTime.Now
                });

                await RespondAsync(embed: CreateCustomEmbed(title: $"Thanks, {Context.User.Username}!", content: "Ewww, bugs! I'll get to squishing them right away!\nThanks for your report!", user: Context.User));
                await Context.Client.GetUserAsync(ulong.Parse(ConfigurationHandler.GetConfigKey("UserID"))).Result.SendMessageAsync(embed: CreateCustomEmbed(title: title, content: bug + $"\n\n**From:** {Context.User.Username}\n**Guild:** {Context.Guild.Name}", color: new Color(255, 0, 0)));
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }
        // Roll random number
        [SlashCommand("roll", "Need a random number? Roll for one!")]
        public async Task GetDiceRoll(int range) => await RespondAsync(embed: CreateCustomEmbed(title: "Rolling....", content: $"I rolled **{new Random().Next(range)}**!", user: Context.User));

        // Add Cambot to server
        [SlashCommand("add", "Want to add Cambot to your server?")]
        public async Task AddCambot() => await RespondAsync(embed: CreateCustomEmbed(title: "Want to add Cambot to your server?", content: "You can use this link: [**https://bit.ly/invite_Cambot**] or, scan the QR code below!", 
            image: ConfigurationHandler.GetConfigKey("QRCode"), credit: "Cambot", user: Context.User, timestamp: true));


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
                null, "open-notify.org", user: Context.User));
        }

        // APOD command
        [SlashCommand("apod", "Get the Astronomy picture of the day from Nasa!")]
        public async Task GetApodImage()
        {
            ApodModel _response = await ApiFunctions.GetApodImage(ConfigurationHandler.GetConfigKey("Nasa"));

            try
            {
                await RespondAsync(null, null, false,
                embed: CreateCustomEmbed(title: _response.title.ToString(), content: $"**{_response.Copyright}**\n{_response.Date}\n\n{_response.explanation}", image: _response.url,
                    credit: "api.nasa.gov", user: Context.User));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await RespondAsync("Something went wrong... Please try again!");
            }
        }

        // Mars Rover images command
        [SlashCommand("mars", "Get Nasa's Mars Rover images!")]
        public async Task GetMarsRoverImages()
        {
            try
            {
                MarsRoot _response = await ApiFunctions.GetMarsRoverImages(ConfigurationHandler.GetConfigKey("Nasa"));
                int index = _response.photos.Count - 1;
                await RespondAsync(null, null, false,
                embed: CreateCustomEmbed(title: _response.photos[index].Rover.Name, content: $"**Date:** {_response.photos[index].Earth_Date}\n**Camera:** {_response.photos[index].Camera.Full_Name}\n**Photo ID:** {_response.photos[index].Sol}", image: _response.photos[index].Img_Src,
                credit: "api.nasa.gov", user: Context.User));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await RespondAsync("Something went wrong... Please try again");
            }
        }

        // Give hug to someone
        [SlashCommand("hug", "Send a hug to a friend!")]
        public async Task GiveHug(SocketUser user) => await RespondAsync(embed: CreateCustomEmbed(null, $"{Context.User.Mention} hugs {user.Mention}!!", GetRandomHugGif(), "Cambot", user: Context.User));

        // Get random cat image
        [SlashCommand("cat", "Get a random cat picture!")]
        public async Task GetRandomCatImage()
        {
            CatsModel _response = await ApiFunctions.GetRandomCatImage();

            if (string.IsNullOrEmpty(_response.file) || _response.file.Contains(".mp4"))
            {
                await RespondAsync("Cats!");
                await Context.Channel.SendMessageAsync(string.IsNullOrEmpty(_response.file) ? "Sorry, something went wrong... Please try again!" : _response.file);
            } else
            {
                await RespondAsync(embed: CreateCustomEmbed(image: _response.file, timestamp: true, credit: "aws.random.cat", user: Context.User));
            }
        }

        // Get a random fox image
        [SlashCommand("fox", "Get a random fox picture!")]
        public async Task GetRandomFoxImage()
        {
            FoxModel _response = await ApiFunctions.GetRandomFoxImage();

            if (string.IsNullOrEmpty(_response.image) || _response.image.Contains(".mp4"))
            {
                await RespondAsync("Foxes!");
                await Context.Channel.SendMessageAsync(string.IsNullOrEmpty(_response.image) ? "Sorry, something went wrong... Please try again!" : _response.image);
            }
            else
            {
                await RespondAsync(embed: CreateCustomEmbed(image: _response.image, credit: "randomfox.ca", timestamp: true, user: Context.User));
            }
        }

        // Get a random dog image
        [SlashCommand("dog", "Get a random dog picture!")]
        public async Task GetRandomDogImage()
        {
            DogModel _response = await ApiFunctions.GetRandomDogImages();

            if (string.IsNullOrEmpty(_response.url) || _response.url.Contains(".mp4"))
            {
                await RespondAsync("Dogs!");
                await Context.Channel.SendMessageAsync(string.IsNullOrEmpty(_response.url) ? "Sorry, something went wrong... Please try again!" : _response.url);
            }
            else
            {
                await RespondAsync(embed: CreateCustomEmbed(image: _response.url, timestamp: true, credit: "https://random.dog", user: Context.User));
            }
        }

        // Get a random Red panda image
        [SlashCommand("redpanda", "Get a random Red Panda image!")]
        public async Task GetRandomRedPandaImage()
        {
            RedPandaModel _response = await ApiFunctions.GetRandomRedPandaImage();
            await RespondAsync(embed: CreateCustomEmbed($"Let me pand this to you...", image: _response.image, credit: "some-random-api.ml", user: Context.User));
        }

        // Get a random raccoon image
        [SlashCommand("raccoon", "Get a random raccoon image")]
        public async Task GetRandomRaccoonImage()
        {
            RaccoonModel _response = await ApiFunctions.GetRandomRaccoonImage();
            await RespondAsync(embed: CreateCustomEmbed($"Here's a raccoon-mendation for you...", image: _response.image, credit: "some-random-api.ml", user: Context.User));
        }

        // Get a random CatFact
        [SlashCommand("catfact", "Get a random cat fact!")]
        public async Task GetRandomCatFact()
        {
            CatFactModel _response = await ApiFunctions.GetRandomCatFact();

            await RespondAsync(embed: CreateCustomEmbed($"(Cat) Food for thought!", $"**Fact**\n{_response.fact}", credit: "catfact.ninja", user: Context.User));
        }

        // Stats
        [SlashCommand("stats", "Get the bot's stats!")]
        public async Task GetBotStats()
        {
            //int totalUsers = 0;
            //_client.Guilds.ToList().ForEach(x => x.GetUsersAsync().ForEachAsync(y => totalUsers++));
            await RespondAsync(embed: CreateCustomEmbed(title: $"Statistics!", content: $"**Latency:** {_client.Latency}ms\n**Total guilds:** {_client.Guilds.Count()}",
                credit: "Cambot", user: Context.User, timestamp: true));
        }

        // People in space
        [SlashCommand("spacepeople", "Who's currently in space?")]
        public async Task GetPeopleInSpace()
        {
            PeopleRoot _response = await ApiFunctions.GetPeopleInSpace();
            StringBuilder sb = new StringBuilder();
            _response.People.ForEach(x => sb.AppendLine($"**Name:** {x.Name}\n **Craft:** {x.Craft}\n"));
            await RespondAsync(embed: CreateCustomEmbed(title: $"Who's in space right now?", content: sb.ToString(), credit: "open-notify.org", user: Context.User, timestamp: true));
        }

        [SlashCommand("topcrypto", "Get the top 25 cryptocurrencies!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task GetTopCryptos()
        {
            CoinsRoot _response = await ApiFunctions.GetCryptos();
            StringBuilder sb = new StringBuilder();
            _response.data.ForEach(x => { if (x.rank < 26) sb.AppendLine($"**{x.rank}**: {x.name} (${x.price_usd})"); });

            await RespondAsync(embed: CreateCustomEmbed(title: $"The Top 25 CryptoCurrencies!", content: sb.ToString(), credit: "coinlore.net", user: Context.User, timestamp: true));
        }

        [SlashCommand("crypto", "Need information about a specific crypto?", runMode: Discord.Interactions.RunMode.Async)]
        public async Task GetCrypto([Remainder] string name = "bitcoin")
        {
            CoinsRoot _response = await ApiFunctions.GetCryptos();
            var coin = _response.data.Where(x => x.name == name || x.nameid == name || x.id == name || x.symbol == name.ToUpper()).FirstOrDefault();

            await RespondAsync(embed: CreateCustomEmbed(title: $"{coin.name}", 
                content: $"**ID:** {coin.id}\n**Symbol:** {coin.symbol}\n**Rank:**{coin.rank}\n**Price:** ${coin.price_usd}\n**24 hour change:** {coin.percent_change_24h}%\n**7 Day change:** {coin.percent_change_7d}%\n**Market Cap:** ${coin.market_cap_usd}\n**Volume:** {coin.volume24}\n**Supply:** {coin.csupply}\n\nNot the coin you wanted? Please try again!",
                credit: "coinlore.net", user: Context.User, timestamp: true));
        }

        [SlashCommand("plants", "Need information about a plant? Look no further!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task GetPlants([Remainder] string plant = null) // This is so ugly please make it nicer to look at Cameron
        {
            if (string.IsNullOrEmpty(plant))
            {
                try
                {
                    TreflePageRoot _response = await ApiFunctions.GetTreflePage(ConfigurationHandler.GetConfigKey("Trefle"));
                    var plant2 = _response.data[new Random().Next(_response.data.Count)];

                    await RespondAsync(embed: CreateCustomEmbed(title: "(Random) " + (string.IsNullOrEmpty(plant2.common_name) ? plant2.scientific_name : plant2.common_name).ToString(),
                        content: $"**Common Name:** " + (string.IsNullOrEmpty(plant2.common_name) ? "N/A" : plant2.common_name).ToString() + $"\n**Scientific Name:** {plant2.scientific_name}\n" +
                        $"**Family Name:** " + (string.IsNullOrEmpty(plant2.family_common_name) ? " N/A" : plant2.family_common_name).ToString() +
                        $"\n**Taxanomic Rank:** {plant2.rank}\n**Genus:** {plant2.genus}\n**Family:** {plant2.family}\n\nWant to research a plant? Use **/plant [name]**", image: plant2.image_url, credit: "trefle.io",
                        user: Context.User, timestamp: true));

                } catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                
            } else
            {
                try // Try at the top to catch any issues in the middle because the [].common_name is really inconsistent
                {
                    TrefleRoot _response = await ApiFunctions.GetTrefle(ConfigurationHandler.GetConfigKey("Trefle"), plant);
                    StringBuilder sb = new StringBuilder();
                    bool single = false;
                    int index = 0;
                    int iter = 0;

                    _response.data.ForEach(x =>
                    {
                        string name = string.IsNullOrEmpty(x.common_name) ? x.scientific_name : x.common_name;
                        if(plant.ToLower().Equals(name.ToLower()) || plant.ToLower().Equals(x.genus.ToLower()))
                        {
                            single = true;
                            index = iter;
                        }
                        iter++;
                    });

                    if (!single || _response.data.Count > 1) 
                        _response.data.ForEach(x => sb.AppendLine($"- " + (string.IsNullOrEmpty(x.common_name) ? x.scientific_name : x.common_name).ToString()));

                    if (single)
                    {
                        string name = string.IsNullOrEmpty(_response.data[index].common_name) ? _response.data[index].scientific_name : _response.data[index].common_name;

                        await RespondAsync(embed: CreateCustomEmbed(title: (string.IsNullOrEmpty(_response.data[index].common_name) ? _response.data[index].scientific_name : _response.data[index].common_name).ToString(),
                            content: $"**Common Name:** " + (string.IsNullOrEmpty(_response.data[index].common_name) ? "N/A" : _response.data[index].common_name).ToString() + $"\n**Scientific Name:** {_response.data[index].scientific_name}\n" +
                            $"**Family Name:** " + (string.IsNullOrEmpty(_response.data[index].family_common_name) ? " N/A" : _response.data[index].family_common_name).ToString() +
                            $"\n**Taxanomic Rank:** {_response.data[index].rank}\n**Genus:** {_response.data[index].genus}\n**Family:** {_response.data[index].family}", image: _response.data[index].image_url, credit: "trefle.io",
                            user: Context.User, timestamp: true));
                    }
                    else
                    {
                        await RespondAsync(embed: CreateCustomEmbed(title: "Results: ", sb.ToString() + $"\n\nPlease refine your search. Example: **/plant {(string.IsNullOrEmpty(_response.data[0].common_name) ? _response.data[0].scientific_name : _response.data[0].common_name).ToString()}**",
                            credit: "trefle.io", user: Context.User));
                    }
                    
                }catch(Exception ex)
                {
                    Logger.Error("Fatal: " + ex.Message);
                }    
            }
        }

        [SlashCommand("yearfact", "Anybody want a random fact about a year?")]
        public async Task GetYearFact([Remainder] string year = null)
        {
            NumbersModel _response = await ApiFunctions.GetYearFact(year);

            if (_response.Found == true)
                await RespondAsync($"**Year:** {_response.Number}\n**Fact:** {_response.Text}");
            else
                await RespondAsync($"There are no facts for that year :confused: Please try again!");
        }

        [SlashCommand("mathfact", "Anybody want a random math fact?")]
        public async Task GetMathFact([Remainder] string number = null)
        {
            NumbersModel _response = await ApiFunctions.GetMathFact(number);

            if (_response.Found == true)
                await RespondAsync($"**Number:** {_response.Number}\n**Fact**: {_response.Text}");
            else
                await RespondAsync($"We couldn't find any facts about that number! :confused: Please try again!");
        }

        [SlashCommand("weather", "Want the weather in your location?")]
        public async Task GetCurrentWeather([Remainder] string city = null)
        {
            if (city == null) await RespondAsync("Please provide me with a city otherwise I can't find the weather! :confused:");

            OWMRoot _response = await ApiFunctions.GetCurrentWeather(city, ConfigurationHandler.GetConfigKey("OWM"));

            try
            {
                
                await RespondAsync(embed: CreateCustomEmbed(title: _response.name, content: $"**Coord:**\n\t**- Longitude:** {_response.coord.lon}\n\t**- Latitude:** {_response.coord.lat}" +
                $"\n\n**Wind:**\n\t**- Speed:** {_response.wind.speed} m/s\n\t**- Direction:** {_response.wind.deg}°\n\n**Conditions:**\n\t**- Temp:** {_response.main.temp}°C ({ConvertToFahrenheit((float)_response.main.temp)}°F)" +
                $"\n\t**- Feels-like:** {_response.main.feels_like}°C ({ConvertToFahrenheit((float)_response.main.feels_like)}°F)\n\t**- Minimum Temp:** {_response.main.temp_min}°C ({ConvertToFahrenheit((float)_response.main.temp_min)}°F)" +
                $"\n\t**- Maximum Temp:** {_response.main.temp_max}°C ({ConvertToFahrenheit((float)_response.main.temp_max)}°F)\n\t**- Pressure:** {_response.main.pressure}\n\t**- Humidity:** {_response.main.humidity}%\n\n**Clouds:** {_response.clouds.all}%", image: null, credit: "openweathermap.org", user: Context.User, timestamp: true));

            }catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }

        [SlashCommand("level", "Check your level")]
        public async Task GetCurrentLevel()
        {
            await RespondAsync(embed: CreateCustomEmbed(title: $"{Context.User.Username}'s stats!", content: $"You are currently **{LevelsDatabaseHandler.GetPlayerExperience(Context.User)} experience** " +
                $"into **level {LevelsDatabaseHandler.GetPlayerLevel(Context.User)}**\n\n__Progress:__\n{LevelsDatabaseHandler.ExperienceBar(Context.User)}", timestamp: true, credit: "Cambot levels", user: Context.User));
        }
    }
}