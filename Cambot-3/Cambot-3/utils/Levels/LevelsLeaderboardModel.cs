using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.utils.Levels
{
    public class LevelsLeaderboardModel
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; }
    }
}
