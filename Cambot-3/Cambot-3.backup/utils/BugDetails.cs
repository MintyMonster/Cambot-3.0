using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.utils
{
    public class BugDetails
    {
        public string title { get; set; }
        public string description { get; set; }
        public string userName;
        public ulong id;
        public string guild;
        public ulong guildId;
        public DateTime currentTime;
    }
}
