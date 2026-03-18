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
                case >= 18 and < 22:
                    GreetingMain.Text = "Good Evening!";
                    break;
                case >= 22 or < 5:
                    GreetingMain.Text = "Good Night!";
                    break;
            }
        }
    }
}