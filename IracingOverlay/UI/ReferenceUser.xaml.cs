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
            LicenseLevel.Background = new SolidColorBrush(Colors.Green);


        }
    }
}