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
            Debug.WriteLine("OnException() fired!" + exception.Message);
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

                // Check telemetry data and session info
                if (irsdk != null && irsdk.Data != null && irsdk.Data.SessionInfo != null && irsdk.Data.SessionInfo.DriverInfo != null)
                {

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _referenceWindow.ClearGrid();
                    });

                    //total number of drivers in the session
                    var totalDrivers = irsdk.Data.SessionInfo.DriverInfo.Drivers.Count;

                    //Driver caridx
                    int DriverIdx = irsdk.Data.SessionInfo.DriverInfo.DriverCarIdx;

                    //DriverLapTime for delta
                    var DriverLapTime = irsdk.Data.GetFloat("CarIdxEstTime", DriverIdx);

                    //driver lap distance
                    var DriverLapDist = irsdk.Data.GetFloat("CarIdxLapDistPct", DriverIdx);

                    //list of all drivers ordered by distance
                    var driversOrdered = new List<(int CarIdx, float LapDistPct)>();


                    //initiate lapdistpct variable
                    float lapDistPct = 0;


                    // Loop through all drivers
                    for (int i = 0; i < totalDrivers - 1; i++)
                    {
                        carIdx = i;
                        lapDistPct = irsdk.Data.GetFloat("CarIdxLapDistPct", carIdx);

                        var tempOffset = 0.5 - DriverLapDist;

                        //check they are on track
                        if (lapDistPct != -1)
                        {
                            lapDistPct = lapDistPct + (float)tempOffset;

                            if (lapDistPct < 0)
                            {
                                lapDistPct = lapDistPct + 1;
                            }
                            if (lapDistPct > 1)
                            {
                                lapDistPct = lapDistPct - 1;
                            }

                            // Add to the list
                            driversOrdered.Add((carIdx, lapDistPct));
                        }
                    }

                    // Sort the list by lap distance percentage in descending order
                    driversOrdered = driversOrdered.OrderByDescending(d => d.LapDistPct).ToList();

                    // Find the index of the players car
                    int playerIndex = driversOrdered.FindIndex(d => d.CarIdx == DriverIdx);

                    var lapdistdriver = irsdk.Data.GetFloat("CarIdxLapDistPct", DriverIdx);

                    //how many placeholders should be added
                    int range = 3 - playerIndex;

                    //if range less than 0 make it 0
                    if (range < 0) { range = 0; }

                    //Decides if placeholders are needed
                    if (range > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //loops for amount necessary
                            for (int x = 0; x < range; x++)
                            {
                                // Add placeholders for empty spaces
                                _referenceWindow.Placeholder();
                            }
                        });
                    }

                    //loop through the 8 closest drivers
                    for (int i = 0; i < 8; i++)
                    {

                        //work out current car
                        int carIndex = i + playerIndex - 3;

                        //check index exists
                        if (carIndex >= 0 && carIndex < driversOrdered.Count)
                        {
                            //assign current car index to caridx
                            (int DriverIndex, float tempvar1) = driversOrdered[carIndex];

                            carIdx = DriverIndex;

                            //lap dist pct of current car
                            lapDistPct = irsdk.Data.GetFloat("CarIdxLapDistPct", carIdx);

                            //estimated lap time
                            var estLapTime = irsdk.Data.GetFloat("CarIdxEstTime", carIdx);

                            var carPosition = irsdk.Data.GetInt("CarIdxPosition", carIdx);



                            double delta = estLapTime - DriverLapTime;

                            //use est and percent to get delta
                            delta = Math.Round(delta, 1);

                            // Retrieve the driver information
                            var driverInfo = irsdk.Data.SessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == carIdx);

                            if (driverInfo != null)
                            {
                                // Retrieve Player Info
                                var driverName = driverInfo.TeamName; // Player Name
                                var carName = driverInfo.CarScreenName; // Car Make/Model
                                var carNumber = driverInfo.CarNumber; // Car Number
                                var LicenseLevel = driverInfo.LicString; // License Level

                                var SafetyRating = driverInfo.LicSubLevel; // Safety Rating
                                double SafetyRatingString = Math.Round(((double)SafetyRating / 100), 1);

                                //convert irating from 1234 to "1.2k"
                                decimal iRating = Math.Round((decimal)driverInfo.IRating / 1000, 1); // iRating
                                string iRatingString = iRating.ToString() + "k";



                                Application.Current.Dispatcher.Invoke(() =>
                                {

                                    //add driver to UI
                                    _referenceWindow.AddDriver("P" + carPosition.ToString(), driverName, SafetyRatingString.ToString(), LicenseLevel, iRatingString, delta.ToString());
                                });

                            }
                        }
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
