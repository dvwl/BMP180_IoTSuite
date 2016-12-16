using BMP180_IoTSuite.Common;
using BMP180_IoTSuite.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using static BMP180_IoTSuite.Models.Bmp180Sensor;

namespace BMP180_IoTSuite.ViewModels
{
    public class Bmp180SensorViewModel : BaseViewModel
    {
        private string deviceName;
        private double temperature, pressure, altitude;
        private string lastUpdated;
        private Bmp180Sensor _bmp180;
        private DispatcherTimer _periodicTimer;

        static DeviceClient deviceClient;
        static string iotHubUri = "projectalpha-hub.azure-devices.net";
        static string deviceKey;

        public string DeviceName
        {
            get
            {
                return deviceName;
            }

            set
            {
                deviceName = value;
                OnPropertyChanged();
            }
        }

        public double Temperature
        {
            get
            {
                return temperature;
            }

            set
            {
                temperature = value;
                OnPropertyChanged();
            }
        }

        public double Pressure
        {
            get
            {
                return pressure;
            }

            set
            {
                pressure = value;
                OnPropertyChanged();
            }
        }

        public double Altitude
        {
            get
            {
                return altitude;
            }

            set
            {
                altitude = value;
                OnPropertyChanged();
            }
        }

        public string LastUpdated
        {
            get
            {
                return lastUpdated;
            }

            set
            {
                lastUpdated = value;
                OnPropertyChanged();
            }
        }

        public Bmp180SensorViewModel()
        {    
            InitializeSensors();
            GetCurrentLocationAsync();
        }

        // TODO:
        // get device key from IoT Hub
        // get device name from User (from view)
        // save device key and device name into local storage
        // add owner name to json message
        // test location data of LocationManager
        // reverse geocode to get street address, etc
        // change visibility of textblock on view

        private async void InitializeSensors()
        {
            // Initialize the BMP180 Sensor
            try
            {
                _bmp180 = new Bmp180Sensor();
                await _bmp180.InitializeAsync();
            }
            catch (Exception ex)
            {
                LastUpdated = "Device Error! " + ex.Message;
            }

            if (_bmp180 == null)
                return;


            // get sensor data every 10 seconds
            _periodicTimer = new DispatcherTimer();
            _periodicTimer.Interval = TimeSpan.FromSeconds(10);
            _periodicTimer.Tick += _periodicTimer_Tick;
            _periodicTimer.Start();
                       
        }

        private async void _periodicTimer_Tick(object sender, object e)
        {
            Bmp180SensorData sensorData = null;

            // Read and format Sensor data
            try
            {
                sensorData = await _bmp180.GetSensorDataAsync(Bmp180AccuracyMode.UltraHighResolution);

                Temperature = sensorData.Temperature;
                Pressure = sensorData.Pressure;
                Altitude = sensorData.Altitude;
                LastUpdated = DateTime.UtcNow.ToString();
            }
            catch (Exception)
            {
                LastUpdated = "Sensor Error! Can't get values from sensor.";
            }

            if (sensorData != null)
            {
                // reference: https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted#create-a-simulated-device-app
                // send to IoT Hub
                SendDeviceToCloudMessagesAsync(sensorData);
            }
        }

        private async void SendDeviceToCloudMessagesAsync(Bmp180SensorData sensorData)
        {

            var messageString = JsonConvert.SerializeObject(new WeatherDTO
            {
                // bestAccuracyLocationName 
                dateTime = LastUpdated,
                temperature = sensorData.Temperature,
                pressure = sensorData.Pressure,
                altitude = sensorData.Altitude,
                latitude = LocationManager.currentlocation.Position.Latitude,
                longitude = LocationManager.currentlocation.Position.Longitude,
            });
            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));
            await deviceClient.SendEventAsync(message);
        }

        private async void GetCurrentLocationAsync()
        {
            await LocationManager.GetCurrentPositionAsync();
        }
    }
}
