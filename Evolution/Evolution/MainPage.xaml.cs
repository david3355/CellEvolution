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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Windows.Media.Imaging;

namespace Evolution
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static bool startedWithTutorial = false;
        public static bool startedWithGameModeChoose = false;
        static Uri Uri = new Uri("/MainPage.xaml", UriKind.Relative);
        ConfigManager configmanager;
        ContentManager contentManager;  // A tartalmak betöltéséhez szükséges

        public static Uri GetUri()
        {
            return Uri;
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            configmanager = ConfigManager.GetInstance;
            contentManager = (Application.Current as App).Content;                      
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            startedWithTutorial = false;
            startedWithGameModeChoose = false;
            panel_choose_level.Visibility = Visibility.Collapsed;
            if (ConfigManager.GetInstance.ReadConfig(ConfigKeys.GameMode) == GameMode.Evolution.ToString() && int.Parse(ConfigManager.GetInstance.ReadConfig(ConfigKeys.MaxLevel)) >= 1)
            {
                btn_select_level.Visibility = Visibility.Visible;
            }
            else
            {
                btn_select_level.Visibility = Visibility.Collapsed;
            }
        }

        private void SetLevelBoard()
        {
            grid_levelboard.Children.Clear();
            grid_levelboard.ColumnDefinitions.Clear();
            grid_levelboard.RowDefinitions.Clear();
            int highestCompletedLevel = int.Parse(ConfigManager.GetInstance.ReadConfig(ConfigKeys.MaxLevel));
            const int ROWS = 3;
            const int COLS = 6;
            for (int i = 0; i < ROWS; i++)
            {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = new GridLength(1, GridUnitType.Star);
                grid_levelboard.RowDefinitions.Add(rowdef);
            }
            for (int i = 0; i < COLS; i++)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                coldef.Width = new GridLength(1, GridUnitType.Star);
                grid_levelboard.ColumnDefinitions.Add(coldef);
            }
            int level = 1;
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    Button button = new Button();
                    button.Width = 120;
                    button.Height = 120;
                    button.Content = level;
                    ImageBrush buttonBackgr = new ImageBrush();
                    if (level <= highestCompletedLevel + 1)
                    {
                        buttonBackgr.ImageSource = new BitmapImage(new Uri(String.Format("Images/thumbnails/bg{0}.jpg", level), UriKind.Relative));
                        button.Tap += level_selected;
                    }
                    else
                    {
                        button.IsEnabled = false;
                    }
                    button.Background = buttonBackgr;
                    level++;
                    grid_levelboard.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                }
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

        private void btn_select_level_Click(object sender, RoutedEventArgs e)
        {
            SetLevelBoard();
            btn_select_level.Visibility = Visibility.Collapsed;
            panel_choose_level.Visibility = Visibility.Visible;
        }

        private void level_selected(object sender, RoutedEventArgs e)
        {
            panel_choose_level.Visibility = Visibility.Collapsed;
            int level = int.Parse((sender as Button).Content.ToString());
            HighScores.ResetLastLevel();
            HighScores.SetLastLevel(level);
            StartGame();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (panel_choose_level.Visibility == Visibility.Visible)
            {
                btn_select_level.Visibility = Visibility.Visible;
                panel_choose_level.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            else base.OnBackKeyPress(e);
        }
    }
}