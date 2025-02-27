using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IracingOverlay.UI
{
    /// <summary>
    /// Interaction logic for ReferenceUser.xaml
    /// </summary>
    public partial class ReferenceUser : UserControl
    {
        public ReferenceUser(string position, string name, string safetyRating, string licenseLevel, string iRating, string delta, SolidColorBrush TextColor, Color BackgroundColor)
        {
            InitializeComponent();
            Position.Content = position;
            Name.Content = name;
            SafetyRating.Content = safetyRating;
            IratingK.Content = iRating;
            DeltaS.Content = delta;


            //Change text color
            Position.Foreground = TextColor;
            Name.Foreground = TextColor;
            DeltaS.Foreground = TextColor;

            // totally optimal way to get the color to work probably

            // Remove "0x"
            string hexColor = licenseLevel.Replace("0x", "");

            // Convert hex to a Color
            Color licenseLevelColor = (Color)ColorConverter.ConvertFromString("#" + hexColor);

            // Set the background color
            LicenseLevel.Background = new SolidColorBrush(licenseLevelColor);

            //set background color of entire element
            backgroundThing.Background = new SolidColorBrush(BackgroundColor);


        }
    }
}