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
using Microsoft.Xna.Framework.Media;

namespace Evolution
{
    public partial class Settings : PhoneApplicationPage
    {
        static Uri Uri = new Uri("/Settings.xaml", UriKind.Relative);
        public static Uri GetUri()
        {
            return Uri;
        }

        public static double slideValMusic = 0.5, slideValEffects = 0.5;
        public static bool loaded = false;
        public static bool stopMusic = false;
        private static bool loadData = false;

        public Settings()
        {
            InitializeComponent();
            loaded = true;
            slideMusic.Value = slideValMusic * 100;
            slideSEffects.Value = slideValEffects * 100;
            cBox_stopPlayer.IsChecked = stopMusic;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            SetGameMode((GameMode)Enum.Parse(typeof(GameMode), ConfigManager.GetInstance.ReadConfig(ConfigKeys.GameMode), true));
        }

        private void SetGameMode(GameMode Mode)
        {
            loadData = true;
            switch (Mode)
            {
                case GameMode.Evolution: radio_gm_evo.IsChecked = true; break;
                case GameMode.Survival: radio_gm_surv.IsChecked = true; break;
            }
            loadData = false;
        }

        private void slideSEffects_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            slideValEffects = (float)slideSEffects.Value / 100;
        }

        private void slideMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = (float)slideMusic.Value / 100;
            slideValMusic = (float)slideMusic.Value / 100;
        }

        private void cBox_stopPlayer_Checked(object sender, RoutedEventArgs e)
        {
            stopMusic = true;
        }

        private void cBox_stopPlayer_Unchecked(object sender, RoutedEventArgs e)
        {
            stopMusic = false;
        }

        private void radio_gm_evo_Checked(object sender, RoutedEventArgs e)
        {
            txt_surv_desc.Visibility = Visibility.Collapsed;
            txt_evo_desc.Visibility = Visibility.Visible;
            if (!loadData) ConfigManager.GetInstance.WriteConfig(ConfigKeys.GameMode, GameMode.Evolution.ToString());
        }

        private void radio_gm_surv_Checked(object sender, RoutedEventArgs e)
        {
            txt_evo_desc.Visibility = Visibility.Collapsed;
            txt_surv_desc.Visibility = Visibility.Visible;
            if (!loadData) ConfigManager.GetInstance.WriteConfig(ConfigKeys.GameMode, GameMode.Survival.ToString());
        }
    }
}