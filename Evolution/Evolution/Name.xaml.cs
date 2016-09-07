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
    public partial class Name : PhoneApplicationPage
    {
        static Uri Uri = new Uri("/Name.xaml", UriKind.Relative);
        public static Uri GetUri()
        {
            return Uri;
        }

        public Name()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GamePage.playerName = nametext.Text;
            if (nametext.Text != "") NavigationService.Navigate(GamePage.GetUri());
        }
        
    }
}