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
using Microsoft.Phone.Tasks;

namespace Evolution
{
    public partial class BuyApp : PhoneApplicationPage
    {
        static Uri Uri = new Uri("/BuyApp.xaml", UriKind.Relative);
        public static bool navigatedFromGame = false;
        private static Uri cellPredatorHD_URI = new Uri("https://www.microsoft.com/store/apps/9pk64j11k62m");

        public static Uri GetUri()
        {
            return Uri;
        }

        public BuyApp()
        {
            InitializeComponent();
            cell_predator_hd_hyperlink.NavigateUri = cellPredatorHD_URI;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void ClearNavigationBackstack()
        {
            if(navigatedFromGame) NavigationService.RemoveBackEntry();
            // while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ClearNavigationBackstack();
            base.OnBackKeyPress(e);
        }

        private void GoToCellPredatorHD()
        {
            WebBrowserTask task = new WebBrowserTask() { Uri = cellPredatorHD_URI };
            task.Show();
        }

        private void Image_Tap(object sender, GestureEventArgs e)
        {
            GoToCellPredatorHD();
        }

        private void cell_predator_hd_hyperlink_Tap(object sender, GestureEventArgs e)
        {
            e.Handled = true;
            GoToCellPredatorHD();
        }
    }
}