using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Flarial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Window Dragbar logic
        /// </summary>
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
            Console.WriteLine("Dragging window...");
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchBtn.IsEnabled = false;
                LaunchContent.Text = "Updating";
                LaunchIcon.Text = " ";
                await ClientHandler.CheckForUpdates();
                LaunchContent.Text = "Starting";
                LaunchIcon.Text = " ";
                await ClientHandler.StartGame();
                LaunchContent.Text = "Launch";
                LaunchBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}