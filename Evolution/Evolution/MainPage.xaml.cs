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
using Microsoft.Xna.Framework;

namespace Evolution
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static bool startedWithTutorial = false;
        public static bool startedWithGameModeChoose = false;
        private static bool logoShowed = false;
        private static SolidColorBrush red_brush = new SolidColorBrush(Colors.Red);
        static Uri Uri = new Uri("/MainPage.xaml", UriKind.Relative);
        ConfigManager configmanager;

        public static Uri GetUri()
        {
            return Uri;
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            configmanager = ConfigManager.GetInstance;
            int highscore, maxlevel, lastlevel;
            int.TryParse(ConfigManager.GetInstance.ReadConfig(ConfigKeys.HighScore), out highscore);
            int.TryParse(ConfigManager.GetInstance.ReadConfig(ConfigKeys.MaxLevel), out maxlevel);
            int.TryParse(ConfigManager.GetInstance.ReadConfig(ConfigKeys.LastLevel), out lastlevel);
            HighScores.HighScore = highscore;
            HighScores.MaxLevel = maxlevel;
            HighScores.LastLevel = lastlevel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //HighScores.SetMaxLevel(18);        // DEBUG: for all levels

            startedWithTutorial = false;
            startedWithGameModeChoose = false;
            panel_choose_level.Visibility = Visibility.Collapsed;
            panel_choose_evomode.Visibility = Visibility.Collapsed;
            panel_loading.Visibility = Visibility.Collapsed;
            if (logoShowed) panel_mainmenu.Visibility = Visibility.Visible;
            if (ConfigManager.GetInstance.ReadConfig(ConfigKeys.GameMode) == GameMode.Evolution.ToString())
            {
                img_selectlevel.Visibility = Visibility.Visible;
            }
            else
            {
                img_selectlevel.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenMain()
        {
            ShowGamePremier();
        }

        private void ShowGamePremier()
        {
            FadeAnimation fadeAnim = new FadeAnimation(panel_premier, ShowMenu);
            fadeAnim.Animate();
        }

        private void ShowMenu()
        {
            panel_premier.Visibility = Visibility.Collapsed;
            panel_mainmenu.Visibility = Visibility.Visible;
            logoShowed = true;
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
                    if (j == COLS - 1)
                    {
                        button.Foreground = red_brush;
                        button.FontWeight = FontWeights.Bold;
                        button.BorderBrush = red_brush;
                        button.BorderThickness = new Thickness(5);
                    }
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
            panel_mainmenu.Visibility = Visibility.Collapsed;
            panel_loading.Visibility = Visibility.Visible;
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
        private void Start_Click(object sender, GestureEventArgs e)
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

        private void Select_level_Click(object sender, GestureEventArgs e)
        {
            SetLevelBoard();
            img_selectlevel.Visibility = Visibility.Collapsed;
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
                img_selectlevel.Visibility = Visibility.Visible;
                panel_choose_level.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            if (panel_choose_evomode.Visibility == Visibility.Visible)
            {
                panel_choose_evomode.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            else base.OnBackKeyPress(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            OpenMain();
        }

        private void img_selectlevelgrey_Tap(object sender, GestureEventArgs e)
        {
            panel_choose_evomode.Visibility = Visibility.Visible;
        }

        private void panel_choose_evomode_Tap(object sender, GestureEventArgs e)
        {
            panel_choose_evomode.Visibility = Visibility.Collapsed;
        }
    }

    delegate void AnimationEnds();
    class FadeAnimation
    {
        public FadeAnimation(UIElement Element, AnimationEnds AnimationEndsCallback)
        {
            element = Element;
            animationEndsCallback = AnimationEndsCallback;
        }

        private UIElement element;
        private GameTimer timer_fadeIn, timer_wait, timer_fadeOut;
        private AnimationEnds animationEndsCallback;
        private double fadeChange;
        private double updateInterval;

        public void Animate()
        {
            element.Opacity = 0.0;
            timer_wait = new GameTimer();
            timer_wait.Update += timer_waitbefore_Update;
            timer_wait.UpdateInterval = TimeSpan.FromMilliseconds(500);
            timer_wait.Start();
        }

        void timer_waitbefore_Update(object sender, GameTimerEventArgs e)
        {
            timer_wait.Stop();
            timer_wait = null;
            const double animationTimeSec = 1;
            fadeChange = 0.01;
            updateInterval = (animationTimeSec * 100) * fadeChange * 10;
            timer_fadeIn = new GameTimer();
            timer_fadeIn.Update += timer_fadeIn_Update;
            timer_fadeIn.UpdateInterval = TimeSpan.FromMilliseconds(updateInterval);
            timer_fadeIn.Start();
        }

        void timer_fadeIn_Update(object sender, GameTimerEventArgs e)
        {
            if (element.Opacity > 0.94)
            {
                element.Opacity = 1;
                timer_fadeIn.Stop();
                timer_fadeIn = null;
                timer_wait = new GameTimer();
                timer_wait.Update += timer_wait_Update;
                timer_wait.UpdateInterval = TimeSpan.FromSeconds(1);
                timer_wait.Start();
            }
            else element.Opacity += fadeChange;
        }

        void timer_wait_Update(object sender, GameTimerEventArgs e)
        {
            timer_wait.Stop();
            timer_wait = null;
            timer_fadeOut = new GameTimer();
            timer_fadeOut.Update += timer_fadeOut_Update;
            timer_fadeOut.UpdateInterval = TimeSpan.FromMilliseconds(updateInterval);
            timer_fadeOut.Start();
        }

        void timer_fadeOut_Update(object sender, GameTimerEventArgs e)
        {
            if (element.Opacity < 0.06)
            {
                element.Opacity = 0;
                timer_fadeOut.Stop();
                timer_fadeOut = null;
                animationEndsCallback();
            }
            else element.Opacity -= fadeChange;
        }

    }
}