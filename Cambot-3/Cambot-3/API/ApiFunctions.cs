using Cambot_3.API.ApiModels;
using Cambot_3.API.ApiModels.InternationalSpaceStation;
using Cambot_3.API.ApiModels.Trefle;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Cambot_3.API
{
    public class ApiFunctions
    {
        // Dad joke command
        public static async Task<DadJokeModel> GetDadJoke()
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://icanhazdadjoke.com/"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<DadJokeModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // ISS command: Timestamp
        public static async Task<IssTimeStamp> GetIssTimeStamp()
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("http://api.open-notify.org/iss-now.json"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<IssTimeStamp>() : throw new Exception(_response.ReasonPhrase);
        }

        // ISS command: Longitude and Latitude
        public static async Task<LongLatISS> GetIssPosition()
        {
            using(HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("http://api.open-notify.org/iss-now.json"))
            {
                if(!(_response.IsSuccessStatusCode))
                    throw new Exception(_response.ReasonPhrase);

                ISSPosition _position = await _response.Content.ReadAsAsync<ISSPosition>();
                return _position.Iss_Position;
            }
        }

        // ISS command: The people in the ISS
        public static async Task<PeopleRoot> GetPeopleInSpace()
        {
            using(HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("http://api.open-notify.org/astros.json"))
                return _response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<PeopleRoot>(await _response.Content.ReadAsStringAsync()) : throw new Exception(_response.ReasonPhrase);
        }

        // Astronomy picture of the day
        public static async Task<ApodModel> GetApodImage(string key)
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync($"https://api.nasa.gov/planetary/apod?api_key={key}")) 
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<ApodModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Mars Rover images
        public static async Task<MarsRoot> GetMarsRoverImages(string key)
        {
            string url = $"https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol={new Random().Next(1, (3680 + ((DateTime.Now - new DateTime(2022, 12, 17)).Days)))}&page={new Random().Next(1, 3)}&api_key={key}";

            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync(url))
                return _response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<MarsRoot>(await _response.Content.ReadAsStringAsync()) : throw new Exception(_response.ReasonPhrase);
        }

        // Cats images
        public static async Task<CatsModel> GetRandomCatImage()
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://aws.random.cat/meow?ref=apilist.fun"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<CatsModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Fox images
        public static async Task<FoxModel> GetRandomFoxImage()
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://randomfox.ca/floof/?ref=apilist.fun"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<FoxModel>() :  throw new Exception(_response.ReasonPhrase);
        }

        // Dog images
        public static async Task<DogModel> GetRandomDogImages()
        {
            using(HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://random.dog/woof.json"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<DogModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Catfact
        public static async Task<CatFactModel> GetRandomCatFact()
        {
            using(HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://catfact.ninja/fact"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<CatFactModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Red Panda
        public static async Task<RedPandaModel> GetRandomRedPandaImage()
        {
            using(HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://some-random-api.ml/animal/red_panda"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<RedPandaModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Raccoon
        public static async Task<RaccoonModel> GetRandomRaccoonImage()
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://some-random-api.ml/animal/raccoon"))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<RaccoonModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Cryptocurrencies
        public static async Task<CoinsRoot> GetCryptos()
        {
            using(HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync("https://api.coinlore.net/api/tickers/"))
                return _response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<CoinsRoot>(await _response.Content.ReadAsStringAsync()) : throw new Exception(_response.ReasonPhrase);
        }

        // Trefle Page 
        public static async Task<TreflePageRoot> GetTreflePage(string key)
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync($"https://trefle.io/api/v1/plants?page={new Random().Next(1, 18879)}&token={key}"))
                return _response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<TreflePageRoot>(await _response.Content.ReadAsStringAsync()) : throw new Exception(_response.ReasonPhrase);
        }

        // Trefle
        public static async Task<TrefleRoot> GetTrefle(string key, string query)
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync($"https://trefle.io/api/v1/plants/search?q={query}&token={key}"))
                return _response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<TrefleRoot>(await _response.Content.ReadAsStringAsync()) : throw new Exception(_response.ReasonPhrase);
        }

        // Year fact
        public static async Task<NumbersModel> GetYearFact(string query)
        {
            string url = string.IsNullOrEmpty(query) ? "http://numbersapi.com/random/year?json=true" : $"http://numbersapi.com/{query}/year?json=true";

            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync(url))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<NumbersModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Math fact
        public static async Task<NumbersModel> GetMathFact(string query)
        {
            string url = string.IsNullOrEmpty(query) ? "http://numbersapi/random/math?json=true" : $"http://numbersapi/{query}/math?json=true";

            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync(url))
                return _response.IsSuccessStatusCode ? await _response.Content.ReadAsAsync<NumbersModel>() : throw new Exception(_response.ReasonPhrase);
        }

        // Open weather map | Current weather
        public static async Task<OWMRoot> GetCurrentWeather(string query, string key)
        {
            using (HttpResponseMessage _response = await ApiHelper.ApiClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={query}&units=metric&appid={key}"))
                return _response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<OWMRoot>(await _response.Content.ReadAsStringAsync()) : throw new Exception(_response.ReasonPhrase);
        }
    }
}