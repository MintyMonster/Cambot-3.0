using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambot_3.API.ApiModels
{
    public class MarsCamera
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rover_Id { get; set; }
        public string Full_Name { get; set; }
    }

    public class MarsRover
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Landing_Date { get; set; }
        public string Launch_Date { get; set; }
        public string Status { get; set; }
    }

    public class MarsPhoto
    {
        public int Id { get; set; }
        public int Sol { get; set; }
        public MarsCamera Camera { get; set; }
        public string Img_Src { get; set; }
        public string Earth_Date { get; set; }
        public MarsRover Rover { get; set; } 
    }

    public class MarsRoot
    {
        public List<MarsPhoto> photos { get; set; }
    }
}
