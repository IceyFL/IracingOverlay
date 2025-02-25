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
        public ReferenceUser(string position, string name, string safetyRating, string licenseLevel, string iRating, string delta)
        {
            InitializeComponent();
            Position.Content = position;
            Name.Content = name;
            SafetyRating.Content = safetyRating;
            IratingK.Content = iRating;
            DeltaS.Content = delta;


            //switch case to decide the license color
            switch (licenseLevel.ToLower())
            {
                case "a":
                    LicenseLevel.Background = new SolidColorBrush(Colors.Blue);
                    break;
                case "b":
                    LicenseLevel.Background = new SolidColorBrush(Colors.Green);
                    break;
                case "c":
                    LicenseLevel.Background = new SolidColorBrush(Colors.Yellow);
                    break;
                case "d":
                    LicenseLevel.Background = new SolidColorBrush(Colors.Orange);
                    break;
                case "r":
                    LicenseLevel.Background = new SolidColorBrush(Colors.Red);
                    break;
                case "p":
                    LicenseLevel.Background = new SolidColorBrush(Colors.Black);
                    break;
                default:
                    LicenseLevel.Background = new SolidColorBrush(Colors.Gray); // Default color if license level is not recognized
                    break;
            }


        }
    }
}