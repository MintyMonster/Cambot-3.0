using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.API.ApiModels
{
    public class PeopleRoot
    {
        public string Message { get; set; }
        public int Number { get; set; }
        public List<PeopleData> People { get; set; }
    }

    public class PeopleData
    {
        public string Name { get; set; }
        public string Craft { get; set; }
    }
}
