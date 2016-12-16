using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP180_IoTSuite.Models
{

    public class WeatherDTO
    {
        public string bestAccuracyLocationName { get; set; }
        public string dateTime { get; set; }
        public double temperature { get; set; }
        public double pressure { get; set; }
        public double altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

}
