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
        public static Uri GetUri()
        {
            return Uri;
        }

        public Help()
        {
            InitializeComponent();
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
            // TODO: check if game is started, if so, go to gamepage
            NavigationService.GoBack();
        }
    }
}