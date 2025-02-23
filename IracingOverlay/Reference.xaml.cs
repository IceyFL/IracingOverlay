using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using IracingOverlay.UI;

namespace IracingOverlay
{
    public partial class Reference : Window
    {
        public Reference()
        {
            InitializeComponent();
            var Driver1 = new ReferenceUser("Reference");
            mainGrid.Children.Add(Driver1);
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }
    }
}
