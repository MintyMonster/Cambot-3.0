using Cambot_3.API;
using Cambot_3.API.ApiModels;
using Cambot_3.API.ApiModels.InternationalSpaceStation;
using Cambot_3.API.ApiModels.Trefle;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.Modules
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private DiscordSocketClient _client;
        private IServiceProvider _services;

        public SlashCommands(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
        }

        // For weather for the Americans
        private string ConvertToFahrenheit(float temp) => ((temp * 1.8) + 32).ToString("N1");

        // Convert all Unix timestamps from API
        private DateTime ConvertUnix(long unix)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unix);
        }

        // Get random hugging GIF
        private string GetHugGif()
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

        private Embed CreateCustomEmbed(string title, string content, SocketUser user)
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

        private List<string> commandNames = new List<string>()
            {
                ":newspaper: help", ":first_quarter_moon_with_face: apod", ":rofl: dadjoke", ":first_quarter_moon_with_face: iss", ":ringed_planet: mars",
            ":calendar: yearfact", ":1234: mathfact", ":cloud_rain: weather", ":cat: cat", ":dog: dog", ":fox: fox", ":panda_face: redpanda", ":raccoon: raccoon",
                ":dollar: topcrypto", ":dollar: crypto", ":cat: catfact", ":potted_plant: plant", ":hugging: hug", ":bar_chart: stats", ":astronaut: spacepeople"
                // Add rest of added commands
            };

        // Help command responses
        
        private async Task GetDadJokeHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "In need of a random dad joke? :rofl:", content: "Usage: **/dadjoke**", user: Context.User));
        private async Task GetApodHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Get the astronomy picture of the day! :first_quarter_moon_with_face:", content: "Usage: **/apod**", user: Context.User));
        private async Task GetISSHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Where's the International Space station? :first_quarter_moon_with_face:", content: "Usage: **/iss**", user: Context.User));
        private async Task GetMarsHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Wow! Pictures from Mars! :ringed_planet:", content: "Usage: **/mars**", user: Context.User));
        private async Task GetYearFactHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Where did 202 even go?! :calendar:", content: "Usage: **/yearfact [*Optional: year*]**", user: Context.User));
        private async Task GetMathFactHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Some say numbers are boring... :1234:", content: "Usage: **/mathfact [*Optional: number*]**", user: Context.User));
        private async Task GetWeatherHelp() => await RespondAsync(embed: CreateCustomEmbed(title: $"It's {(new Random().Next(0, 11) < 5 ? "hot :hot_face: out!" : "cold :cold_face: out!").ToString()}", content: "Usage: **/weather [*Required: City*]**", user: Context.User));
        private async Task GetCatHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Feline friends! :cat:", content: "Usage: **/cat**", user: Context.User));
        private async Task GetDogHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Woofers! :dog:", content: "Usage: **/dog**", user: Context.User));
        private async Task GetFoxHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Pandora's fox. Haha, get it? :fox:", content: "Usage: **/fox**", user: Context.User));
        private async Task GetRaccoonHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Trash pandas! :raccoon:", content: "Usage: **/raccoon**", user: Context.User));
        private async Task GetRedPandaHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "These are bear-y cute! :panda_face:", content: "Usage: **/redpanda**", user: Context.User));
        private async Task GetTopCryptoHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "The top 25 Cryptocurrencies right now! :dollar:", content: "Usage: **/topcrypto**", user: Context.User));
        private async Task GetCryptoHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Want to know about a specific coin? :dollar:", content: "Usage: **/crypto [*Optional: name, Default: Bitcoin*]**", user: Context.User));
        private async Task GetCatFactHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Feline friend facts! :cat:", content: "Usage: **/catfact**", user: Context.User));
        private async Task GetPlantsHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Plants! :potted_plant:", content: "Usage: **/plants [*Optional: plantName*]**", user: Context.User));
        private async Task GetHugHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Awwww, hugs! :hugging:", content: "Usage: **/hug [*Required: mention*]**", user: Context.User));
        private async Task GetStatsHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "How's Cambot doing? :bar_chart:", content: "Usage: **/stats**", user: Context.User));
        private async Task GetSpacePeopleHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "Who's in space right now? :astronaut:", content: "Usage: **/spacepeople**", user: Context.User));
        private async Task GetHelpHelp() => await RespondAsync(embed: CreateCustomEmbed(title: "You've already found me silly! :newspaper:", content: "Usage: **/help [*Optional: Command Name*]**", user: Context.User));
        // Add info, leaderboard, help, points, contact, addtoserver, updates, and others?


        //////////////////////////////////////////////////
        // COMMANDS START
        //////////////////////////////////////////////////

        // Help command
        [SlashCommand("help", "Need some help?")]
        public async Task GetHelp([Remainder] string command = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Cambot has some awesome commands!\nHere are some to name!\n");
                commandNames.ForEach(x => sb.AppendLine($"\t- **{x}**"));
                sb.AppendLine($"\nWant to know about a specific command?\nExample: **/help weather**");

                await RespondAsync(embed: CreateCustomEmbed(title: "All the commands!", content: sb.ToString(), credit: "Cambot", user: Context.User, timestamp: true));
            }
            else
            {
                switch (command)
                {
                    case "help":
                        GetHelpHelp();
                        break;

                    case "apod":
                        GetApodHelp();
                        break;

                    case "dadjoke":
                        GetDadJokeHelp();
                        break;

                    case "iss":
                        GetISSHelp();
                        break;

                    case "mars":
                        GetMarsHelp();
                        break;

                    case "yearfact":
                        GetYearFactHelp();
                        break;

                    case "mathfact":
                        GetMathFactHelp();
                        break;

                    case "weather":
                        GetWeatherHelp();
                        break;

                    case "cat":
                        GetCatHelp();
                        break;

                    case "dog":
                        GetDogHelp();
                        break;

                    case "fox":
                        GetFoxHelp();
                        break;

                    case "raccoon":
                        GetRaccoonHelp();
                        break;

                    case "redpanda":
                        GetRedPandaHelp();
                        break;

                    case "topcrypto":
                        GetTopCryptoHelp();
                        break;

                    case "crypto":
                        GetCryptoHelp();
                        break;

                    case "catfact":
                        GetCatFactHelp();
                        break;

                    case "plants":
                        GetPlantsHelp();
                        break;

                    case "hug":
                        GetHugHelp();
                        break;

                    case "stats":
                        GetStatsHelp();
                        break;

                    case "spacepeople":
                        GetSpacePeopleHelp();
                        break;
                }
            }

        }

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
        [Alias("hugs")]
        public async Task GiveHug(SocketUser user)
        {
            await RespondAsync(embed: CreateCustomEmbed(null, $"{Context.User.Mention} hugs {user.Mention}!!", GetHugGif(), "Cambot", user: Context.User));
        }

        // Get random cat image
        [SlashCommand("cat", "Get a random cat picture!")]
        [Alias("cats")]
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
        [Alias("foxes")]
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
        [Alias("dogs")]
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
        [Alias("cfact")]
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
    }
}