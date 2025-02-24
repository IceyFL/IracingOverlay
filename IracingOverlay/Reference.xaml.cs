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
            //temp variables
            string position = "P1";
            string drivername = "John Doe";
            string safetyrating = "4.99";
            string licenselevel = "A";
            string irating = "10.9k";
            string delta = "+100.9s";

            //create a new ReferenceUser object
            var Driver1 = new ReferenceUser(position, drivername, safetyrating, licenselevel, irating, delta);
            //add reference user object to the UI
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
