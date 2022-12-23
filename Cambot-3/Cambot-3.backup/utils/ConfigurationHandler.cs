using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3
{
    // Global configuration handler
    public static class ConfigurationHandler
    {
        private static IConfiguration _config;

        public static string GetConfigKey(string key)
        {
            _config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "ConfigurationFile.json").Build();
            return _config.GetValue<string>(key);
        }
    }
}
