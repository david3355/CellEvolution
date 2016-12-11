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

namespace Evolution
{
    public partial class ChooseGameMode : PhoneApplicationPage
    {
        static Uri Uri = new Uri("/ChooseGameMode.xaml", UriKind.Relative);
        static bool gameModeChosen = false;
        static bool loadData = false;

        public static Uri GetUri()
        {
            return Uri;
        }
        public ChooseGameMode()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (gameModeChosen)
            {
                txt_choose.Visibility = Visibility.Collapsed;
                GameMode gamemode = (GameMode)Enum.Parse(typeof(GameMode), ConfigManager.GetInstance.ReadConfig(ConfigKeys.GameMode), true);
                loadData = true;
                switch (gamemode)
                {
                    case GameMode.Evolution: radio_chosegm_evo.IsChecked = true; break;
                    case GameMode.Survival: radio_chosegm_surv.IsChecked = true; break;
                }
            }
            loadData = false;
        }

        private void radio_chosegm_evo_Checked(object sender, RoutedEventArgs e)
        {
            gameModeChosen = true;
            btn_gamemode_next.IsEnabled = true;
            txt_choose.Visibility = Visibility.Collapsed;
            txt_surv_tutorial.Visibility = Visibility.Collapsed;
            txt_evo_tutorial.Visibility = Visibility.Visible;
            if(!loadData) ConfigManager.GetInstance.WriteConfig(ConfigKeys.GameMode, GameMode.Evolution.ToString());
        }

        private void radio_chosegm_surv_Checked(object sender, RoutedEventArgs e)
        {
            gameModeChosen = true;
            btn_gamemode_next.IsEnabled = true;
            txt_choose.Visibility = Visibility.Collapsed;
            txt_evo_tutorial.Visibility = Visibility.Collapsed;
            txt_surv_tutorial.Visibility = Visibility.Visible;
            if (!loadData) ConfigManager.GetInstance.WriteConfig(ConfigKeys.GameMode, GameMode.Survival.ToString());
        }

        private void btn_gamemode_next_Click(object sender, RoutedEventArgs e)
        {
            String showTutorial = ConfigManager.GetInstance.ReadConfig(ConfigKeys.ShowTutorial);
            if (showTutorial == "true")
            {
                MainPage.startedWithTutorial = true;
                NavigationService.Navigate(Help.GetUri());
            }
            else
            {
                MainPage.startedWithTutorial = false;
                NavigationService.Navigate(GamePage.GetUri());
            }
        }
    }
}