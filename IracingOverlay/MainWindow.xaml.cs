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

/// Import the IRSDKSharper library
using IRSDKSharper;

namespace IracingOverlay
{
    public partial class MainWindow : Window
    {
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
        }

        private void ReferenceUnchecked(object sender, RoutedEventArgs e)
        {
            if (_referenceWindow != null)
            {
                _referenceWindow.Close();
                _referenceWindow = null;
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

            Debug.WriteLine($"OnSessionInfo fired! Track name is {trackName}.");
        }

        private void OnTelemetryData()
        {
            int carIdx = 0;

            // Check telemetry data and session info
            if (irsdk != null && irsdk.Data != null && irsdk.Data.SessionInfo != null && irsdk.Data.SessionInfo.DriverInfo != null)
            {


                var totalDrivers = irsdk.Data.SessionInfo.DriverInfo.Drivers.Count;

                carIdx = totalDrivers - 1;

                // Get the lap distance percentage
                var lapDistPct = irsdk.Data.GetFloat("CarIdxLapDistPct", carIdx);

                // Retrieve the driver information
                var driverInfo = irsdk.Data.SessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == carIdx);

                if (driverInfo != null)
                {
                    // Retrieve Player Info
                    var driverName = driverInfo.TeamName; // Player Name
                    var carName = driverInfo.CarScreenName; // Car Make/Model
                    var carNumber = driverInfo.CarNumber; // Car Number
                    var LicenseLevel = driverInfo.LicLevel; // License Level
                    var iRating = driverInfo.IRating; // iRating

                    // Update Info on Window
                    Dispatcher.Invoke(() =>
                    {
                        Delta.Text = $"Distance: {totalDrivers}, Name: {driverName}, Number: {iRating}.";
                    });
                }
                else
                {
                    // driver info not found
                    Dispatcher.Invoke(() =>
                    {
                        Delta.Text = $"Distance: {lapDistPct}. Driver information not found.";
                    });
                }
            }
            else
            {
                // telemetry data session info is null
                Dispatcher.Invoke(() =>
                {
                    Delta.Text = "Telemetry data or session info is not available.";
                });
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
