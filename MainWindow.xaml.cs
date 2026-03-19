using System.Diagnostics;
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
            GetTime();
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

        /// <summary>
        /// Window Controls Logic.
        /// </summary>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Launching the game.
        /// </summary>
        private async void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchBtn.IsEnabled = false;
                LaunchContent.Text = "Updating";
                LaunchIcon.Text = " ";
                if (!await ClientHandler.CheckForUpdates())
                {
                    FailedLaunch();
                    return;
                }
                LaunchContent.Text = "Starting";
                LaunchIcon.Text = " ";
                if (!await ClientHandler.StartGame())
                {
                    FailedLaunch();
                    return;
                }
                await Task.Delay(2000);
                LaunchContent.Text = "Launch";
                LaunchBtn.IsEnabled = true;
            }
            catch { FailedLaunch(); }
        }
        private async void FailedLaunch()
        {
            LaunchContent.Text = "Failed";
            LaunchIcon.Text = " ";
            await Task.Delay(2000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    LaunchContent.Text = "Launch";
                    LaunchIcon.Text = " ";
                    LaunchBtn.IsEnabled = true;
                });
            });
            LaunchBtn.IsEnabled = true;
        }

        /// <summary>
        /// Sets the greeting text based on the current time of day.
        /// </summary>
        private async void GetTime()
        {
            switch (DateTime.Now.Hour) 
            {
                case >= 5 and < 12:
                    GreetingMain.Text = "Good Morning!";
                    break;
                case >= 12 and < 18:
                    GreetingMain.Text = "Good Afternoon!";
                    break;
                default:
                    GreetingMain.Text = "Good Evening!";
                    break;
            }
        }
    }
}