using BronzeFactoryApplication.ApplicationServices;
using BronzeFactoryApplication.Helpers.StartupHelpers;
using BronzeFactoryApplication.ViewModels;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BronzeFactoryApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IDisposable
    {
        public static string ApplicationVersion { get => ((App)App.Current).ApplicationVersion; }

        private readonly LoggingConsole loggingConsole;

        public MainWindow(MainViewModel vm, 
                          LoggingConsole loggingConsole)
        {
            InitializeComponent();
            DataContext = vm;
            this.loggingConsole = loggingConsole;
        }

        /// <summary>
        /// A Timer to Handle the Opacity Change of the Starting Logo
        /// </summary>
        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(40) };
        private void MainStartingWindow_Initialized(object sender, EventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        /// <summary>
        /// Handles the Initial Screen Opacity Play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (StartingLogoContainer.Opacity > 0)
            {
                StartingLogoContainer.Opacity -= 0.05;
            }
            //else if(MainWindowCentralGrid.Opacity < 1)
            //{
            //    MainWindowCentralGrid.Opacity += 0.05;
            //}
            else
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// Properly Dispose the Window
        /// </summary>
        public void Dispose()
        {
            timer.Tick -= Timer_Tick;
        }

        /// <summary>
        /// Closes the Whole Application when the Main Window Closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainStartingWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainStartingWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MessageService.Questions.ApplicationClose() is MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        [RelayCommand]
        private void OpenConsole()
        {
            //The Logging Console is a SingleTonWindow as the Main Window . So its always considered Open
            //So we must check if its visible
            if (loggingConsole.IsVisible)
            {
                loggingConsole.Hide();
            }
            else
            {
                loggingConsole.Show();
            }
        }
    }
}
