using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace IracingOverlay
{
    public partial class Reference : Window
    {
        public Reference()
        {
            InitializeComponent();
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
