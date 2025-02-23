using System.Windows;
using System.Windows.Controls;

namespace IracingOverlay.UI
{
    /// <summary>
    /// Interaction logic for ReferenceUser.xaml
    /// </summary>
    public partial class ReferenceUser : UserControl
    {
        public ReferenceUser(string Text)
        {
            InitializeComponent();
            Name.Content = Text;
        }
    }
}