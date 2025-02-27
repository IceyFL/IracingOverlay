using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

            // this means fire the OnTelemetryData event every 12 data frames (5 times a second)
            irsdk.UpdateInterval = 12;

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
            RunRelative();
        }


        private void OnStopped()
        {
            Debug.WriteLine("OnStopped() fired!");
        }

        private void OnDebugLog(string message)
        {
            Debug.WriteLine(message);
        }

        #region Relative

        private void RunRelative()
        {
            if (ReferenceWindowOpen)
            {
                //initialize caridx
                int carIdx = 0;

                // Check telemetry data and session info
                if (irsdk != null && irsdk.Data != null && irsdk.Data.SessionInfo != null && irsdk.Data.SessionInfo.DriverInfo != null)
                {
                    //Clear Grid using different thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _referenceWindow.ClearGrid();
                    });

                    //total number of drivers in the session
                    var totalDrivers = irsdk.Data.SessionInfo.DriverInfo.Drivers.Count;

                    //Driver caridx
                    int DriverIdx = irsdk.Data.SessionInfo.DriverInfo.DriverCarIdx;

                    //driver lap distance
                    var DriverLapDist = irsdk.Data.GetFloat("CarIdxLapDistPct", DriverIdx);

                    //list of all drivers ordered by distance
                    List<(int CarIdx, float LapDistPct)> driversOrdered = SortDriverList(totalDrivers, DriverLapDist);

                    // Find the index of the players car
                    int playerIndex = driversOrdered.FindIndex(d => d.CarIdx == DriverIdx);

                    AddPlaceholders(DriverIdx, driversOrdered, playerIndex);
                    

                    //create driverInfo variable
                    var driverInfo = irsdk.Data.SessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == DriverIdx);

                    //get average lap time for delta calculations
                    int avglaptime = GetAvgLap(driverInfo, driversOrdered);


                    //DriverLapTime for delta
                    var DriverLapTime = DriverLapDist * avglaptime;


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

                            (var delta, var lapoffset) = GetDelta(carIdx, avglaptime, DriverLapTime, i);

                            // Retrieve the driver information
                            driverInfo = irsdk.Data.SessionInfo.DriverInfo.Drivers.FirstOrDefault(d => d.CarIdx == carIdx);

                            if (driverInfo != null)
                            {
                                // Retrieve Player Info
                                var driverName = driverInfo.TeamName; // Player Name
                                var carName = driverInfo.CarScreenName; // Car Make/Model
                                var carNumber = driverInfo.CarNumber; // Car Number
                                var LicenseLevel = driverInfo.LicColor;// License Level

                                //convert Safety Rating from 375 to 3.75
                                double SafetyRating = Math.Round(((double)driverInfo.LicSubLevel / 100), 1);
                                //convert irating from 1234 to "1.2k"
                                decimal iRating = Math.Round((decimal)driverInfo.IRating / 1000, 1); // iRating

                                //position
                                var carPosition = irsdk.Data.GetInt("CarIdxPosition", carIdx);


                                //get text color using function
                                Color TextColor = DecideTextColor(carIdx, DriverIdx, lapoffset);

                                //Dispatch to seperate thread
                                Application.Current.Dispatcher.Invoke(() =>
                                {

                                    //add driver to UI
                                    _referenceWindow.AddDriver("P" + carPosition.ToString(), driverName, SafetyRating.ToString(), LicenseLevel, iRating.ToString(), delta.ToString(), new SolidColorBrush(TextColor));
                                });

                            }
                        }
                    }
                }
            }
        }

        private Color DecideTextColor(int carIdx, int DriverIdx, int lapOffset)
        {

            //get variables
            //Driver and Car Lap
            var driverlap = irsdk.Data.GetInt("CarIdxLap", DriverIdx);
            var currentlap = irsdk.Data.GetInt("CarIdxLap", carIdx);
            //adjust for lap offset
            currentlap += lapOffset;
            //is car in pits
            var inPits = irsdk.Data.GetBool("CarIdxOnPitRoad", carIdx);


            //iniate default color
            var TextColor = Colors.White;

            //check if its the driver
            if (carIdx == DriverIdx)
            {
                TextColor = Colors.Gold;
            }

            //check if in pits
            else if (inPits)
            {
                TextColor = Colors.DarkGray;
            }

            //red if car is lapping driver
            else if (currentlap > driverlap)
            {
                TextColor = Colors.Red;
            }

            //light blue if driver is lapping car
            else if (currentlap < driverlap)
            {
                TextColor = Colors.SkyBlue;
            }

            return TextColor;
        }

        private List<(int CarIdx, float LapDistPct)> SortDriverList(int totalDrivers, float DriverLapDist)
        {
            //initiate variables
            float lapDistPct = 0;
            int carIdx = 0;
            var driversOrdered = new List<(int CarIdx, float LapDistPct)>();

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

            //return list
            return driversOrdered;
        }

        private void AddPlaceholders(int DriverIdx, List<(int CarIdx, float LapDistPct)> driversOrdered, int playerIndex)
        {

            //how many placeholders should be added
            int range = 3 - playerIndex;

            //if range less than 0 make it 0
            if (range < 0) { range = 0; }

            //Decides if placeholders are needed
            if (range > 0)
            {
                //Dispatch to different thread
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
        }

        private int GetAvgLap(IRacingSdkSessionInfo.DriverInfoModel.DriverModel driverInfo, List<(int CarIdx, float LapDistPct)> driversOrdered)
        {
            //average lap time
            float avglaptime = 0;
            var count = 0;


            //calculate avglaptime
            for (int i = 0; i < driversOrdered.Count; i++)
            {
                //temp variable to get info froom the list
                (var temp1, var temp2) = driversOrdered[i];
                //last lap of the car
                var lastlap = irsdk.Data.GetFloat("CarIdxLastLapTime", temp1);

                //check if in the pits
                var inPits = irsdk.Data.GetBool("CarIdxOnPitRoad", temp1);

                //disregard car if in pits
                if (inPits == false)
                {
                    count = count + 1;

                    //checks if there is a last lap
                    if (lastlap != -1)
                    {
                        avglaptime = avglaptime + lastlap;
                    }
                    //get car class default lap time
                    else
                    {
                        avglaptime = avglaptime + driverInfo.CarClassEstLapTime;
                    }
                }
            }

            //finally calculate the value
            avglaptime = avglaptime / count;

            //return value
            return (int)avglaptime;
        }

        private (double, int) GetDelta(int carIdx, int avglaptime, float DriverLapTime, int i)
        {
            //lap dist pct of current car
            var lapDistPct = irsdk.Data.GetFloat("CarIdxLapDistPct", carIdx);

            //estimated lap time
            var estLapTime = lapDistPct * avglaptime;



            double delta = estLapTime - DriverLapTime;


            //lap offset for lap calculations later
            var lapOffset = 0;

            //fix delta if they are on different laps

            //if delta is negative and driver is on a later lap
            if (i < 3 && delta < 0)
            {
                lapOffset = -1;
                delta = avglaptime - DriverLapTime + estLapTime;
            }

            //if delta is positive and driver is on an earlier lap
            if (i > 3 && delta > 0)
            {
                lapOffset = 1;
                delta = estLapTime - (avglaptime + DriverLapTime);
            }

            //round delta to 1 decimal point
            delta = Math.Round(delta, 1);

            //return values
            return (delta, lapOffset);
        }

        #endregion

        #endregion

    }
}
