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
        }

        public void AddDriver(string position, string drivername, string safetyrating, string licenselevel, string irating, string delta) {

            //create a reference user object
            var Driver = new ReferenceUser(position, drivername, safetyrating, licenselevel, irating, delta);

            //add it to the UI
            mainGrid.Children.Add(Driver);
        }

        public void Placeholder() {
            
            //create a placeholder object
            var empty = new Placeholder();

            //add to the UI
            mainGrid.Children.Add(empty);

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
