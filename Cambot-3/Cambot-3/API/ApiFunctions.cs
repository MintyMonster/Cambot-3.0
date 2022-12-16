using Cambot_3.API.ApiModels;
using Cambot_3.API.ApiModels.InternationalSpaceStation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
    }
}
