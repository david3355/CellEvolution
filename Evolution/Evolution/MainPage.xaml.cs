using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Threading;

namespace Evolution
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static bool startedWithTutorial = false;
        static Uri Uri = new Uri("/MainPage.xaml", UriKind.Relative);
        ConfigManager configmanager;
        public static Uri GetUri()
        {
            return Uri;
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            configmanager = ConfigManager.GetInstance;
        }

        // Simple button Click event handler to take us to the second page
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            String showTutorial = configmanager.ReadConfig(ConfigKeys.ShowTutorial);
            if (showTutorial == "true")
            {
                startedWithTutorial = true;
                NavigationService.Navigate(Help.GetUri());
            }
            else
            {
                startedWithTutorial = false;
                NavigationService.Navigate(GamePage.GetUri());
            }
        }

        private void Settings_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(Settings.GetUri());
        }

        private void Highscores_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(HighScores.GetUri());
        }

        private void Help_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(Help.GetUri());
        }

    }
}