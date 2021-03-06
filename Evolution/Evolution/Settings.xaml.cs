﻿using System;
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

        public static double slideValMusic = 1, slideValEffects = 0.3;
        public static bool loaded = false;
        public static bool stopMusic = false;
        public Settings()
        {
            InitializeComponent();
            loaded = true;
            slideMusic.Value = slideValMusic;
            slideSEffects.Value = slideValEffects;
            cBox_stopPlayer.IsChecked = stopMusic;
        }

        private void slideSEffects_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            slideValEffects = (float)slideSEffects.Value;
        }

        private void slideMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = (float)slideMusic.Value;
            slideValMusic = (float)slideMusic.Value;
        }

        private void cBox_stopPlayer_Checked(object sender, RoutedEventArgs e)
        {
            stopMusic = true;
        }

        private void cBox_stopPlayer_Unchecked(object sender, RoutedEventArgs e)
        {
            stopMusic = false;
        }
    }
}