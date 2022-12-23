using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.API.ApiModels
{
    public class NumbersModel
    {
        public string Text { get; set; }
        public int Number { get; set; }
        public bool Found { get; set; }
        public string Type { get; set; }
    }
}
