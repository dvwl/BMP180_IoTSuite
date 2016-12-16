using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;

namespace BMP180_IoTSuite.Common
{
    public class LocationManager
    {
        private static Geolocator geolocator = new Geolocator();
        public static Geopoint currentlocation;

        public static async Task GetCurrentPositionAsync()
        {
            try
            {
                // no need to be that accurate
                geolocator.DesiredAccuracyInMeters = 500;

                // the device is not going to move around
                // so will set the maximum age / validity for location to be an hour
                // we'll timeout in 10 seconds if we can't get the location data
                var pos = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(60),
                    timeout: TimeSpan.FromSeconds(10));

                currentlocation = pos.Coordinate.Point;
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("Location service is disable in your settings.");
                await dialog.ShowAsync();
            }
        }


    }
}
