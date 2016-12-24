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
            panel_objectinfo.Visibility = Visibility.Collapsed;
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
            HideObjectInfo();
            Help2.Visibility = Visibility.Collapsed;
            Help1.Visibility = Visibility.Visible;
        }

        private void btn_help2_next_Click(object sender, RoutedEventArgs e)
        {
            HideObjectInfo();
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

        private void panel_objectinfo_Tap(object sender, GestureEventArgs e)
        {
            HideObjectInfo();
        }

        private void HideObjectInfo()
        {
            panel_objectinfo.Visibility = Visibility.Collapsed;
        }

        private void SetObjectInfo(String ResourceName)
        {
            text_info_smallerenemy.Inlines.Clear();
            Span textSpan = (Span)this.Resources[ResourceName];
            Inline newInline;
            foreach (Inline inline in textSpan.Inlines)
            {
                if (inline is Run)
                {
                    newInline = new Run();
                    (newInline as Run).Text = (inline as Run).Text;
                }
                else if (inline is Bold)
                {
                    newInline = new Bold();
                    //newInline.Text = ((inline as Bold).Inlines[0] as Run).Text;
                    String text = ((inline as Bold).Inlines[0] as Run).Text;
                    (newInline as Bold).Inlines.Add(text);
                }
                else newInline = new Run();
                text_info_smallerenemy.Inlines.Add(newInline);
            }
            panel_objectinfo.Visibility = Visibility.Visible;
        }

        private void infostack_Tap(object sender, GestureEventArgs e)
        {
            string name = (sender as StackPanel).Name;
            string resource = name.Replace("stack", "info");
            SetObjectInfo(resource);
        }
    }
}