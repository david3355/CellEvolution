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
    public partial class Help : PhoneApplicationPage
    {
        static Uri Uri = new Uri("/Help.xaml", UriKind.Relative);
        private ConfigManager configmanager;
        public static Uri GetUri()
        {
            return Uri;
        }

        public Help()
        {
            InitializeComponent();
            configmanager = ConfigManager.GetInstance;
            bool showtutorial = true;
            bool.TryParse(configmanager.ReadConfig(ConfigKeys.ShowTutorial), out showtutorial);
            check_showtutorial.Checked -= check_showtutorial_Checked;
            check_showtutorial.Unchecked -= check_showtutorial_Unchecked;
            check_showtutorial.IsChecked = showtutorial;
            check_showtutorial.Checked += check_showtutorial_Checked;
            check_showtutorial.Unchecked += check_showtutorial_Unchecked;
        }        

        private void btn_help1_back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btn_help1_next_Click(object sender, RoutedEventArgs e)
        {
            Help1.Visibility = Visibility.Collapsed;
            Help2.Visibility = Visibility.Visible;
        }

        private void btn_help2_back_Click(object sender, RoutedEventArgs e)
        {
            Help2.Visibility = Visibility.Collapsed;
            Help1.Visibility = Visibility.Visible;
        }

        private void btn_help2_next_Click(object sender, RoutedEventArgs e)
        {
            Help2.Visibility = Visibility.Collapsed;
            Help3.Visibility = Visibility.Visible;
        }

        private void btn_help3_back_Click(object sender, RoutedEventArgs e)
        {
            Help3.Visibility = Visibility.Collapsed;
            Help2.Visibility = Visibility.Visible;
        }

        private void btn_help3_okay_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.startedWithTutorial)
            {
                NavigationService.Navigate(GamePage.GetUri());                
            }
            else NavigationService.GoBack();
        }

        private void check_showtutorial_Checked(object sender, RoutedEventArgs e)
        {
            configmanager.WriteConfig(ConfigKeys.ShowTutorial, "true");
        }

        private void check_showtutorial_Unchecked(object sender, RoutedEventArgs e)
        {
            configmanager.WriteConfig(ConfigKeys.ShowTutorial, "false");
        }
    }
}