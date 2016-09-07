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
    public partial class HighScores : PhoneApplicationPage
    {
        static Uri Uri = new Uri("/HighScores.xaml", UriKind.Relative);
        public static Uri GetUri()
        {
            return Uri;
        }

        public HighScores()
        {
            InitializeComponent();
            txt_hl.Text = hLevel.ToString();
            txt_hs.Text = hScore.ToString();
        }

        public static int hScore = 0;
        public static int hLevel = 0;
    }
}