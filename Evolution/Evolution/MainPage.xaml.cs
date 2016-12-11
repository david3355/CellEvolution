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
        public static bool startedWithGameModeChoose = false;
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            startedWithTutorial = false;
            startedWithGameModeChoose = false;
            panel_restart_confirm.Visibility = Visibility.Collapsed;
            if (ConfigManager.GetInstance.ReadConfig(ConfigKeys.GameMode) == GameMode.Evolution.ToString() && ConfigManager.GetInstance.ReadConfig(ConfigKeys.LastLevel) != "1")
            {
                btn_restart.Visibility = Visibility.Visible;
            }
            else
            {
                btn_restart.Visibility = Visibility.Collapsed;
            }
        }


        private void StartGame()
        {
            String firstStart = configmanager.ReadConfig(ConfigKeys.FirstStart);
            if (firstStart == "true")
            {
                startedWithGameModeChoose = true;
                NavigationService.Navigate(ChooseGameMode.GetUri());
            }
            else
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
        }

        // Simple button Click event handler to take us to the second page
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            StartGame();
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

        private void btn_restart_Click(object sender, RoutedEventArgs e)
        {
            btn_restart.Visibility = Visibility.Collapsed;
            panel_restart_confirm.Visibility = Visibility.Visible;
        }

        private void restart_yes_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.GetInstance.WriteConfig(ConfigKeys.LastLevel, "1");
            StartGame();
        }

        private void restart_no_Click(object sender, RoutedEventArgs e)
        {
            btn_restart.Visibility = Visibility.Visible;
            panel_restart_confirm.Visibility = Visibility.Collapsed;
        }

    }
}