using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.API.ApiModels
{
    public class OWMCoord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class OWMWeather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class OWMMain
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double pressure { get; set; }
        public double humidity { get; set; }
    }

    public class OWMWind
    {
        public double speed { get; set; }
        public int deg { get; set; }
    }

    public class OWMClouds
    {
        public int all { get; set; }
    }

    public class OWMSys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class OWMRoot
    {
        public OWMCoord coord { get; set; }
        public List<OWMWeather> weather { get; set; }
        public string @base { get; set; }
        public OWMMain main { get; set; }
        public int visibility { get; set; }
        public OWMWind wind { get; set; }
        public OWMClouds clouds { get; set; }
        public int dt { get; set; }
        public OWMSys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }
}
