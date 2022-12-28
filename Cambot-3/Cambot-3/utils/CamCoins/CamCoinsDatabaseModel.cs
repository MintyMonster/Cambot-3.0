using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.utils.CamCoins
{
    public class CamCoinsDatabaseModel
    {
        [Key]
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public double Money { get; set; }
        public double CamCoins { get; set; }
        public int Rigs { get; set; }
        public int GPUs { get; set; }
        public double NetWorth { get; set; }
        public int Health { get; set; }
        public int Food { get; set; }
        public int Water { get; set; }
        public int Happiness { get; set; }
    }
}
