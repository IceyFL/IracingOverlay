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


            // totally optimal way to decide the license color
            if (licenseLevel.ToLower().Contains("a"))
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Blue);
            }
            else if (licenseLevel.ToLower().Contains("b"))
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Green);
            }
            else if (licenseLevel.ToLower().Contains("c"))
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Yellow);
            }
            else if (licenseLevel.ToLower().Contains("d"))
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (licenseLevel.ToLower().Contains("r"))
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Red);
            }
            else if (licenseLevel.ToLower().Contains("p"))
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                LicenseLevel.Background = new SolidColorBrush(Colors.Gray); // Default color if license level is not recognized
            }


        }
    }
}