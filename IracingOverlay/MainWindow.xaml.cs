using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IracingOverlay.UI;

/// Import the IRSDKSharper library
using IRSDKSharper;

namespace IracingOverlay
{
    public partial class MainWindow : Window
    {
        bool ReferenceWindowOpen = false;

        public bool IsReferenceChecked { get; set; }

        private Reference _referenceWindow;

        private IRacingSdk irsdk;

        public MainWindow()
        {
            InitializeComponent();

            // create an instance of IRacingSdk
            irsdk = new IRacingSdk();

            // hook up our event handlers
            irsdk.OnException += OnException;
            irsdk.OnConnected += OnConnected;
            irsdk.OnDisconnected += OnDisconnected;
            irsdk.OnSessionInfo += OnSessionInfo;
            irsdk.OnTelemetryData += OnTelemetryData;
            irsdk.OnStopped += OnStopped;
            irsdk.OnDebugLog += OnDebugLog;

            // this means fire the OnTelemetryData event every 30 data frames (2 times a second)
            irsdk.UpdateInterval = 30;

            // lets go!
            irsdk.Start();
        }

        private void ReferenceChecked(object sender, RoutedEventArgs e)
        {
            _referenceWindow = new Reference();
            _referenceWindow.Show();
            ReferenceWindowOpen = true;
        }

        private void ReferenceUnchecked(object sender, RoutedEventArgs e)
        {
            if (_referenceWindow != null)
            {
                _referenceWindow.Close();
                _referenceWindow = null;
                ReferenceWindowOpen = false;
            }
        }

        #region EventHandlers

        //debugging stuff

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            irsdk.Stop();
        }

        private void OnException(Exception exception)
        {
            Debug.WriteLine("OnException() fired!");
        }

        private void OnConnected()
        {
            Debug.WriteLine("OnConnected() fired!");
        }

        private void OnDisconnected()
        {
            Debug.WriteLine("OnDisconnected() fired!");
        }

        private void OnSessionInfo()
        {
            var trackName = irsdk.Data.SessionInfo.WeekendInfo.TrackName;
        }

        private void OnTelemetryData()
        {

            #region Relative
            if (ReferenceWindowOpen) {

            int carIdx = 0;
            double delta = 0.0;

                // Check telemetry data and session info
                if (irsdk != null && irsdk.Data != null && irsdk.Data.SessionInfo != null && irsdk.Data.SessionInfo.DriverInfo != null)
                {

                    //total number of drivers in the session
                    var totalDrivers = irsdk.Data.SessionInfo.DriverInfo.Drivers.Count;

                    //Driver caridx
                    int DriverIdx = irsdk.Data.SessionInfo.DriverInfo.DriverCarIdx;



                    //list of all drivers ordered by distance
                    var driversOrdered = new List<(int CarIdx, float LapDistPct)>();

                    carIdx = totalDrivers - 1;

                    //initiate lapdistpct variable
                    float lapDistPct = 0;

                    // Loop through all drivers
                    for (int i = 0; i < totalDrivers - 1; i++)
                    {
                        carIdx = i;
                        lapDistPct = irsdk.Data.GetFloat("CarIdxLapDistPct", carIdx);

                        // Add to the list
                        driversOrdered.Add((carIdx, lapDistPct));
                    }

                    // Find the index of the players car
                    int playerIndex = driversOrdered.FindIndex(d => d.CarIdx == DriverIdx);

                    // Sort the list by lap distance percentage in descending order
                    driversOrdered = driversOrdered.OrderByDescending(d => d.LapDistPct).ToList();



                    //estimated lap time
                    var estLapTime = irsdk.Data.GetFloat("CarIdxEstTime", carIdx);

                    //use est and percent to get delta
                    delta = Math.Round(lapDistPct * estLapTime, 1);

                    //convert delta to string
                    string deltaString = delta.ToString() + "s";

                    carIdx = DriverIdx;

                    // Retrieve the driver information
                    var driverInfo = irsdk.Data.SessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == carIdx);

                    if (driverInfo != null)
                    {
                        // Retrieve Player Info
                        var driverName = driverInfo.TeamName; // Player Name
                        var carName = driverInfo.CarScreenName; // Car Make/Model
                        var carNumber = driverInfo.CarNumber; // Car Number
                        var LicenseLevel = driverInfo.LicLevel; // License Level
                        var SafetyRating = driverInfo.LicSubLevel; // Safety Rating
                        var iRating = driverInfo.IRating; // iRating
                    }
                }
                #endregion
            }
        }


        private void OnStopped()
        {
            Debug.WriteLine("OnStopped() fired!");
        }

        private void OnDebugLog(string message)
        {
            Debug.WriteLine(message);
        }

        #endregion

    }
}
