using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using IracingOverlay.UI;

namespace IracingOverlay
{
    public partial class Reference : Window
    {
        //list to store all the UI elements
        private List<UIElement> elements = new List<UIElement>();

        public Reference()
        {
            InitializeComponent();
        }

        public void AddDriver(string position, string drivername, string safetyrating, string licenselevel, string irating, string delta) {

            //create a reference user object
            var Driver = new ReferenceUser(position, drivername, safetyrating, licenselevel, irating, delta);

            //add it to the UI
            elements.Add(Driver);
        }

        public void Placeholder() {
            
            //create a placeholder object
            var empty = new Placeholder();

            //add to the UI
            elements.Add(empty);

        }

        public void ClearGrid()
        {
            // Clear all children from the mainGrid
            mainGrid.Children.Clear();

            //add all the elements from the list to the mainGrid
            foreach (var element in elements)
            {
                mainGrid.Children.Add(element);
            }

            elements.Clear(); // reset the list
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
