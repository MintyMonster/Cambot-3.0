using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.Database
{
    public class PointsLeaderboardModel
    {
        [Key]
        public string UserID { get; set; }
        public string Username { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
    }
}