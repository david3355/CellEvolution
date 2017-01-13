using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace Evolution
{
    public partial class GamePage : PhoneApplicationPage
    {
        public static Uri GetUri()
        {
            return Uri;
        }

        #region Fields

        static Uri Uri = new Uri("/GamePage.xaml", UriKind.Relative);
        public static readonly string[] backgrNames = { "bg1", "bg2", "bg3", "bg4", "bg5", "bg6", "bg7", "bg8", "bg9", "bg10", "bg11", "bg12", "bg13", "bg14", "bg15", "bg16", "bg17", "bg18" };

        const string TEXT_EXTINCT = "Cell destroyed";
        const string TEXT_CONGRATULATIONS = "Congratulations!";
        const string TEXT_LEVELCOMPLETED = "Congratulations, level {0} completed!";
        const string TEXT_LASTLEVELCOMPLETED = "You have completed the last level!";
        const string TEXT_DOUBLETAP = "Tap the screen on two points for new level";
        const string TEXT_DOUBLETAP_RESTART = "Tap the screen on two points to restart level";
        const string TEXT_PRESSBACK = "Press Back key again to exit";
        const string TEXT_PLAYFURTHER = "You can play further if you like...";
        const string TEXT_LEVEL = "Level {0}";
        const string TEXT_SCORE = "Score: {0}";

        float width_tx_extinct, width_tx_doubletap, width_tx_pressback, width_tx_doubletap_restart, width_tx_lastlevelcomp, width_tx_congrat, width_tx_playfurther, width_tx_level;

        ContentManager contentManager;  // A tartalmak betöltéséhez szükséges
        GameTimer timer;
        SpriteBatch spriteBatch; // A rajzolásért/megjelenítésért felelős grafikai objektum        

        Player player;
        Texture2D tx_player;
        Texture2D tx_enemy_smaller;
        Texture2D tx_enemy_bigger;
        Texture2D tx_intellienemy_smaller;
        Texture2D tx_intellienemy_bigger;
        Texture2D tx_antimatter;
        Texture2D tx_sdinf;
        Texture2D tx_iminf;
        Texture2D tx_bG;
        Texture2D tx_rage;
        Texture2D tx_doubletap;
        Texture2D tx_doubletap_red;
        Texture2D tx_blackboard;
        List<Texture2D> backgrounds;

        Song s_music;
        SoundEffect se_collosion, se_move, se_infection, se_extinct, se_rage, se_levelCompleted, se_gameCompleted;

        bool levelStarted;
        bool backKeyPressed;
        int actualBackgroundIndex;
        int n_enemy, n_antim, n_inf, n_intellienemy;
        int rageCycle;
        int rageDuration;
        float speed;
        int initialPlayerSize;
        public static float musicVolume = 1f, effectsVolume = 0.4f;
        public static string playerName;
        double smallObjectInfectTreshold;
        bool levelCompletedSoundPlayed;

        List<Cell> objects;
        int level, score;

        Random rnd = new Random();
        SpriteFont sf, sf_mgs, sf_levelcomp_msg, sf_congrat_msg;
        GameTimer gt_sdi, gt_imi, gtse, gt_game, gt_rageOn; // Az infection objektumok időzítői
        GameTimer gt_startlevel;


        int t_imi, t_sdi; // time of inverse moving infection / time of size decreasing infection
        int t_se, t_game, t_rageOn;

        bool touching, twoTouches, levelEnd, canTwoTouch, infected, rageObject, rageOn;
        int terminated;
        private String text_levelcompleted = "Congratulations, level completed!";
        private String text_score = "Score:";
        private String text_level = "Level";
        private Color levelEndColor = Color.Green;
        private Color lastLevelCompletedColor = Color.Blue;

        #endregion Fields

        void NewLevel()
        {
            level += 1;
            SetLevel(level);
        }

        void RestartLevel()
        {
            SetLevel(level);
        }

        void SetLevel(int Level)
        {
            SetBackground();
            initialPlayerSize = 10 + level / 2;
            speed = 0.1f + level * 0.09f;
            int levelchange = level <= 15 ? level : 15;
            n_enemy = 4 + levelchange / 2;
            n_intellienemy = 6 + levelchange / 2;
            n_antim = 10 + levelchange / 3;
            n_inf = 2 + level / 6;
            t_game = 0;
            rageCycle = 60 - level * 2;
            rageDuration = 6;
            Initialize();
        }

        private void SetBackground()
        {
            actualBackgroundIndex = level - 1;
            if (actualBackgroundIndex >= backgrounds.Count - 1) actualBackgroundIndex = backgrounds.Count - 1;
            tx_bG = backgrounds[actualBackgroundIndex];
        }

        void Initialize()
        {
            text_level = String.Format(TEXT_LEVEL, level);
            levelStarted = false;
            gt_startlevel = new GameTimer();
            gt_startlevel.UpdateInterval = TimeSpan.FromMilliseconds(1500);
            gt_startlevel.Update += gt_startlevel_Update;
            gt_startlevel.Start();
            levelEnd = false;
            canTwoTouch = false;
            twoTouches = false;
            terminated = 0;
            levelCompletedSoundPlayed = false;

            effectsVolume = (float)Settings.slideValEffects;

            //player:
            Vector2 velocity = Vector2.Zero;
            Vector2 center = new Vector2(400, 240);
            int playerStartRadius = initialPlayerSize;
            double bigenemyMaxSize = 13 + level;
            int animatterMaxSize = 10 + level;
            player = new Player(this, tx_player, center, velocity, playerStartRadius);
            //all other objects:
            if (objects != null && objects.Count > 0) objects.Clear();
            objects = new List<Cell>();
            AddObjects(new Enemy(), tx_enemy_smaller, n_enemy, playerStartRadius, GetRanVelocity(speed));
            AddObjects(new Enemy(), tx_enemy_bigger, n_enemy, -bigenemyMaxSize, GetRanVelocity(speed));
            AddObjects(new IntelligentEnemy(), tx_intellienemy_smaller, n_intellienemy, playerStartRadius, GetRanVelocity(speed));
            AddObjects(new IntelligentEnemy(), tx_intellienemy_bigger, n_intellienemy, -bigenemyMaxSize, GetRanVelocity(speed));
            AddObjects(new AntiMatter(), tx_antimatter, n_antim, animatterMaxSize, GetRanVelocity(speed));
            AddObjects(new SizeDecrease(), tx_sdinf, n_inf, 0, Vector2.Zero);
            AddObjects(new InverseMoving(), tx_iminf, n_inf, 0, Vector2.Zero);

            gt_sdi.Stop(); t_sdi = 0; // az új pálya kezdésekor nem lehet infection
            gt_game.Start();
        }

        // E metódus segítségébel az objektumokat szignatúra alapján, univerzálisan tudjuk hozzáadni a listához
        void AddObjects(Cell obj, Texture2D texture, int number, double maxRad, Vector2 velocity)
        {
            float radius;
            Vector2 origoPosition;
            for (int i = 0; i < number; i++)
            {
                if (maxRad == 0) radius = 12; // az infection-ök mérete rögzített
                else if (maxRad < 0) radius = Utility.RandomDouble(player.R + 0.1, Math.Abs(maxRad)); // ha nagyobb ellenséget adunk hozzá, tudnunk kell róla, így csak a player sugara és maxRad között generálhatunk számokat
                else radius = Utility.RandomDouble(5, maxRad);
                origoPosition = GetRandomPositionAroundPlayer(radius);
                if (obj is IntelligentEnemy) obj = new IntelligentEnemy();  // TODO: ezen szépíteni kéne
                else if (obj is Enemy) obj = new Enemy();
                else if (obj is AntiMatter) obj = new AntiMatter();
                else if (obj is SizeDecrease) obj = new SizeDecrease();
                else if (obj is InverseMoving) obj = new InverseMoving();
                obj.SetAttributes(this, texture, origoPosition, velocity, radius);
                objects.Add(obj);
            }
        }

        void gt_startlevel_Update(object sender, GameTimerEventArgs e)
        {
            gt_startlevel.Stop();
            gt_startlevel = null;
            levelStarted = true;
        }

        void AddSmallerEnemyToBalance(GameObjectCount Count)
        {
            Cell enemy;
            Texture2D texture;
            if (rnd.Next(0, 3) < 1)
            {
                enemy = new Enemy();
                texture = tx_enemy_smaller;
            }
            else
            {
                enemy = new IntelligentEnemy();
                texture = tx_intellienemy_smaller;
            }
            float radius;
            if (player.R <= 0.2) return;
            if (player.R <= 2) radius = player.R - 0.1f;
            else radius = Utility.RandomDouble(player.R / 2, player.R - 0.1);
            Vector2 position = GetRandomPositionAroundPlayer(radius);
            enemy.SetAttributes(this, texture, position, GetRanVelocity(speed), radius);
            objects.Add(enemy);
        }

        Vector2 GetRandomPositionAroundPlayer(float Radius)
        {
            double screenWidth, screenHeight;
            screenWidth = this.ActualWidth > this.ActualHeight ? this.ActualWidth : this.ActualHeight;
            screenHeight = this.ActualWidth > this.ActualHeight ? this.ActualHeight : this.ActualWidth;
            float dist = 100;
            float X, Y;
            float margin = 5;
            int NumberOfTrials = 0;
            do
            {
                X = Utility.RandomDouble(Radius + margin, screenWidth - Radius * 2 - margin);
                Y = Utility.RandomDouble(Radius + margin, screenHeight - Radius * 2 - margin);
                NumberOfTrials++;
            }
            while (X < player.Origo.X + dist && X > player.Origo.X - dist && Y < player.Origo.Y + dist && Y > player.Origo.Y - dist || CollideWithOtherObjects(X, Y, Radius, NumberOfTrials)); // távolságot kell, hogy hagyjunk a player és az új objektumok között
            return new Vector2(X, Y);
        }

        private bool CollideWithOtherObjects(float OrigoX, float OrigoY, float Radius, int NumberOfTrials)
        {
            const int MAX_TRIAL_NUMBER = 1000;
            if (NumberOfTrials > MAX_TRIAL_NUMBER) return false;
            const float DISTANCE = 0;
            foreach (Cell obj in objects)
            {
                if (obj is Enemy && Utility.DistanceEdge(obj.Origo, obj.R, new Vector2(OrigoX, OrigoY), Radius) < DISTANCE) return true;
            }
            return false;
        }

        Vector2 GetRanVelocity(float speed)
        {
            return new Vector2(rnd.Next(2) < 1 ? speed : -speed, rnd.Next(2) < 1 ? speed : -speed); // TODO: minden irányba random
        }

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;
        }

        private void LoadBackGrounds()
        {
            foreach (String bgrname in backgrNames)
            {
                backgrounds.Add(contentManager.Load<Texture2D>("backgrounds/" + bgrname));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ConfigManager.GetInstance.ReadConfig(ConfigKeys.FirstStart) == "true") ConfigManager.GetInstance.WriteConfig(ConfigKeys.FirstStart, "false");

            if (MainPage.startedWithTutorial) NavigationService.RemoveBackEntry();
            if (MainPage.startedWithGameModeChoose) NavigationService.RemoveBackEntry();
            TouchPanel.EnabledGestures = GestureType.Flick /*| GestureType.Hold | GestureType.Tap | GestureType.DoubleTap*/;

            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            //contentManager.RootDirectory = "Content";
            // TODO: use this.content to load your game content here

            backKeyPressed = false;
            backgrounds = new List<Texture2D>();
            LoadBackGrounds();
            actualBackgroundIndex = -1;
            tx_player = contentManager.Load<Texture2D>("player");
            tx_enemy_smaller = contentManager.Load<Texture2D>("enemy_smaller");
            tx_enemy_bigger = contentManager.Load<Texture2D>("enemy_bigger");
            tx_intellienemy_smaller = contentManager.Load<Texture2D>("intellienemy_smaller");
            tx_intellienemy_bigger = contentManager.Load<Texture2D>("intellienemy_bigger");
            tx_antimatter = contentManager.Load<Texture2D>("antimatter");
            tx_sdinf = contentManager.Load<Texture2D>("sizedecinf");
            tx_iminf = contentManager.Load<Texture2D>("inverseinf");
            tx_rage = contentManager.Load<Texture2D>("rage");
            tx_doubletap = contentManager.Load<Texture2D>("doubletap");
            tx_doubletap_red = contentManager.Load<Texture2D>("doubletap_red");
            tx_blackboard = contentManager.Load<Texture2D>("blackboard");
            sf = contentManager.Load<SpriteFont>("myFont");
            sf_mgs = contentManager.Load<SpriteFont>("SFmgs");
            sf_levelcomp_msg = contentManager.Load<SpriteFont>("levelcomp_msg");
            sf_congrat_msg = contentManager.Load<SpriteFont>("congrat_msg");
            s_music = contentManager.Load<Song>("BGMusic");
            se_collosion = contentManager.Load<SoundEffect>("collosion");
            se_extinct = contentManager.Load<SoundEffect>("death");
            se_move = contentManager.Load<SoundEffect>("move");
            se_infection = contentManager.Load<SoundEffect>("infection");
            se_rage = contentManager.Load<SoundEffect>("flame");
            se_levelCompleted = contentManager.Load<SoundEffect>("glassbell");
            se_gameCompleted = contentManager.Load<SoundEffect>("glassbell_gamecompleted");

            width_tx_extinct = sf_levelcomp_msg.MeasureString(TEXT_EXTINCT).X;
            width_tx_lastlevelcomp = sf_levelcomp_msg.MeasureString(TEXT_LASTLEVELCOMPLETED).X;
            width_tx_congrat = sf_congrat_msg.MeasureString(TEXT_CONGRATULATIONS).X;
            width_tx_doubletap = sf_mgs.MeasureString(TEXT_DOUBLETAP).X;
            width_tx_pressback = sf_mgs.MeasureString(TEXT_PRESSBACK).X;
            width_tx_doubletap_restart = sf_mgs.MeasureString(TEXT_DOUBLETAP_RESTART).X;
            width_tx_playfurther = sf_mgs.MeasureString(TEXT_PLAYFURTHER).X;
            width_tx_level = sf_congrat_msg.MeasureString(TEXT_LEVEL).X;

            level = 0;
            score = 0;
            smallObjectInfectTreshold = 2.5;
            gt_sdi = new GameTimer();
            gt_sdi.UpdateInterval = TimeSpan.FromSeconds(1);
            gt_sdi.Update += gt_imi_Update;
            gt_imi = new GameTimer();
            gt_imi.UpdateInterval = TimeSpan.FromSeconds(1);
            gt_imi.Update += gt_sdi_Update;
            gtse = new GameTimer();
            gtse.UpdateInterval = TimeSpan.FromMilliseconds(1);
            gtse.Update += gtse_Update;
            gt_game = new GameTimer();
            gt_game.UpdateInterval = TimeSpan.FromSeconds(1);
            gt_game.Update += gt_game_Update;
            gt_rageOn = new GameTimer();
            gt_rageOn.UpdateInterval = TimeSpan.FromSeconds(1);
            gt_rageOn.Update += gt_rageOn_Update;
            t_se = 0;
            t_game = 0;

            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.White);
            int lastLevel = int.Parse(ConfigManager.GetInstance.ReadConfig(ConfigKeys.LastLevel));
            level = lastLevel;
            SetLevel(lastLevel);

            if (Settings.stopMusic)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = musicVolume;
                MediaPlayer.Play(s_music);
            }
            else if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = musicVolume;
                MediaPlayer.Play(s_music);
            }

            timer.Start();

            base.OnNavigatedTo(e);
        }

        void gt_imi_Update(object sender, GameTimerEventArgs e)
        {
            t_imi -= 1;
        }

        void gt_sdi_Update(object sender, GameTimerEventArgs e)
        {
            t_sdi -= 1;
        }

        void gtse_Update(object sender, GameTimerEventArgs e)
        {
            t_se += 1;
        }

        void gt_game_Update(object sender, GameTimerEventArgs e)
        {
            t_game += 1;
            BalanceEnemies();
            SlowlyKillTinyObjects();
        }

        void gt_rageOn_Update(object sender, GameTimerEventArgs e)
        {
            t_rageOn += 1;
        }

        private void StopTimers()
        {
            timer.Stop();
            gt_game.Stop();
        }

        /// <summary>
        /// This method is invoked when the GamePage is not longer visible, when a user press Back, Home buttons, etc.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopTimers();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);
            //NavigationService.Navigate(MainPage.GetUri());
            //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            //base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            if (!levelStarted) return;
            HandleTouches();
            UpdateObjects();
            IsLevelEnd();
            if (player.velocity.X > 0) player.velocity.X -= 0.1f;
            if (player.velocity.X < 0) player.velocity.X += 0.1f;
            if (player.velocity.Y > 0) player.velocity.Y -= 0.1f;
            if (player.velocity.Y < 0) player.velocity.Y += 0.1f;
            if (Math.Abs(player.velocity.Y) < 0.11) player.velocity.Y = 0;
            if (Math.Abs(player.velocity.X) < 0.11) player.velocity.X = 0;
            if (terminated == 1)
            {
                PlayerDies();
            }
            if ((levelEnd || IsPlayerTerminated()) && !canTwoTouch) canTwoTouch = true;
            if (levelEnd)
            {
                HighScores.SetMaxLevel(level);
                HighScores.SetLastLevel(level + 1);
            }

            if (levelEnd && twoTouches) NewLevel();
            if (IsPlayerTerminated() && twoTouches) RestartLevel();
            if (t_game > 0 && t_game % rageCycle == 0 && !rageObject)
            {
                rageObject = true;
                AddObjects(new Rage(), tx_rage, 1, 0, Vector2.Zero);
                se_rage.Play(effectsVolume, 0, 0);
            }
            if (t_game > 0 && t_game % rageCycle == rageDuration && rageObject)
            {
                rageObject = false;
                foreach (Cell obj in objects) if (obj is Rage) { objects.Remove(obj); break; }
            }
        }

        private bool IsLastLevel()
        {
            return level == backgrounds.Count;
        }

        private void PlayerDies()
        {
            se_extinct.Play(effectsVolume, 0, 0);
            objects.Clear();
            gt_game.Stop();
            terminated = -1;
            if (ConfigManager.GetInstance.ReadConfig(ConfigKeys.GameMode) == GameMode.Survival.ToString())
            {
                level = 1;
                HighScores.ResetLastLevel();
            }
        }

        private void BalanceEnemies()
        {
            int ratio = 3;
            int maxSmallerEnemies = 30;
            GameObjectCount count = CountObjects();
            if (terminated == 0 && !levelEnd && count.SmallerEnemies < count.GreaterEnemies * ratio && count.SmallerEnemies < maxSmallerEnemies)
                AddSmallerEnemyToBalance(count);
        }

        private void SlowlyKillTinyObjects()
        {
            float sizeDecrease = 0.3f;
            Cell obj;
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                obj = objects[i];
                if (obj is Enemy)
                {
                    if (obj.R < smallObjectInfectTreshold) obj.R -= sizeDecrease;
                    if (obj.R <= 0) objects.RemoveAt(i);
                }
            }
            if (player.R < smallObjectInfectTreshold) player.R -= sizeDecrease;
        }

        class GameObjectCount
        {
            public GameObjectCount()
            {
                GreaterEnemies = 0;
                SmallerEnemies = 0;
                AllEnemies = 0;
            }

            public int GreaterEnemies { get; set; }
            public int SmallerEnemies { get; set; }
            public int AllEnemies { get; set; }
        }

        private GameObjectCount CountObjects()
        {
            GameObjectCount count = new GameObjectCount();
            foreach (Cell gameobject in objects)
            {
                if (gameobject is Enemy)
                {
                    count.AllEnemies++;
                    if (gameobject.R < player.R) count.SmallerEnemies++;
                    else count.GreaterEnemies++;
                }
            }
            return count;
        }

        private bool IsPlayerTerminated()
        {
            return !levelEnd && player.R <= 0;
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(tx_bG, Vector2.Zero, Color.White);
            DrawBlackBoard(0.1f);


            if (!twoTouches && terminated == 0) // Amíg nincs double tap és a játékos él, addig kirajzoljuk az objektumokat és az adatokat.
            {
                if (player.R > 0) player.Draw(spriteBatch);
                foreach (Cell en in objects) en.Draw(spriteBatch);
                if (player.R > 0 && player.OnRage()) player.Draw(spriteBatch);
            }
            else if (IsPlayerTerminated())
            {
                DrawBlackBoard(0.7f);
                spriteBatch.DrawString(sf_levelcomp_msg, TEXT_EXTINCT, new Vector2((float)this.ActualWidth / 2 - width_tx_extinct / 2, (float)this.ActualHeight / 2 - 30), Color.Red);
                spriteBatch.DrawString(sf_mgs, TEXT_DOUBLETAP_RESTART, new Vector2((float)this.ActualWidth / 2 - width_tx_doubletap_restart / 2, (float)this.ActualHeight / 2 + 50), Color.Red);
                int tap_img_width = 100;
                spriteBatch.Draw(tx_doubletap_red, new Microsoft.Xna.Framework.Rectangle((int)this.ActualWidth / 2 - tap_img_width / 2, (int)this.ActualHeight / 2 + 100, tap_img_width, tap_img_width), Color.White);
            }
            if (levelEnd)
            {
                DrawBlackBoard(0.7f);
                if (IsLastLevel())
                {
                    spriteBatch.DrawString(sf_congrat_msg, TEXT_CONGRATULATIONS, new Vector2((float)this.ActualWidth / 2 - width_tx_congrat / 2, (float)this.ActualHeight / 2 - 110), lastLevelCompletedColor);
                    spriteBatch.DrawString(sf_levelcomp_msg, TEXT_LASTLEVELCOMPLETED, new Vector2((float)this.ActualWidth / 2 - width_tx_lastlevelcomp / 2, (float)this.ActualHeight / 2 + 30), lastLevelCompletedColor);
                    spriteBatch.DrawString(sf_mgs, TEXT_PLAYFURTHER, new Vector2((float)this.ActualWidth / 2 - width_tx_playfurther / 2, (float)this.ActualHeight / 2 + 100), lastLevelCompletedColor);
                }
                else
                {
                    spriteBatch.DrawString(sf_levelcomp_msg, text_levelcompleted, new Vector2((float)this.ActualWidth / 2 - sf_levelcomp_msg.MeasureString(text_levelcompleted).X / 2, (float)this.ActualHeight / 2 - 100), levelEndColor);
                    spriteBatch.DrawString(sf_mgs, TEXT_DOUBLETAP, new Vector2((float)this.ActualWidth / 2 - width_tx_doubletap / 2, (float)this.ActualHeight / 2), levelEndColor);
                    int tap_img_width = 100;
                    spriteBatch.Draw(tx_doubletap, new Microsoft.Xna.Framework.Rectangle((int)this.ActualWidth / 2 - tap_img_width / 2, (int)this.ActualHeight / 2 + 50, tap_img_width, tap_img_width), Color.White);
                }
                spriteBatch.DrawString(sf_levelcomp_msg, text_score, new Vector2((float)this.ActualWidth / 2 - sf_levelcomp_msg.MeasureString(text_score).X / 2, 30), Color.White);
            }

            if (!levelStarted)
            {
                DrawBlackBoard(0.7f);
                spriteBatch.DrawString(sf_levelcomp_msg, text_level, new Vector2((float)this.ActualWidth / 2 - sf_levelcomp_msg.MeasureString(text_level).X / 2, (float)this.ActualHeight / 2 - 30), Color.AntiqueWhite);
            }

            if (backKeyPressed)
            {
                DrawBlackBoard(0.5f);
                spriteBatch.DrawString(sf_mgs, TEXT_PRESSBACK, new Vector2((float)this.ActualWidth / 2 - width_tx_pressback / 2, (float)this.ActualHeight / 2 + 170), Color.Red);
            }
            spriteBatch.End();
        }

        private void DrawBlackBoard(float Opacity)
        {
            spriteBatch.Draw(tx_blackboard, new Microsoft.Xna.Framework.Rectangle(0, 0, (int)this.ActualWidth, (int)this.ActualHeight), Color.White * Opacity);
        }

        void HandleTouches()
        {
            TouchCollection touches = TouchPanel.GetState();

            if (!touching && touches.Count > 0)
            {
                if (terminated == 0)
                {
                    backKeyPressed = false;
                    if (player.R > 0 && touches.Count < 2) se_move.Play(effectsVolume, 0, 0);
                    SetNewVelocity(touches);
                }
                touching = true;
                if (canTwoTouch && touches.Count == 2) twoTouches = true;
                else twoTouches = false;
            }
            else if (touches.Count == 0)
            {
                touching = false;
                twoTouches = false;
            }
            // if (levelEnd || IsPlayerTerminated()) twoTouches = true;    // TODO: only for debug; simulate two touches to enable next level
        }

        void SetNewVelocity(TouchCollection touch)
        {
            // itt állítjuk be, hogy kattintás (tap) hatására milyen irányba és sebességgel mozduljon el a player
            float speed = 1.5f; // basic speed
            speed += GetBoostByGesture();
            Vector2 K = player.Origo;
            Vector2 T = touch[0].Position;
            Vector2 V = new Vector2();
            V.X = K.X - T.X;
            V.Y = K.Y - T.Y;
            if (Math.Abs(V.X) > Math.Abs(V.Y))
            {
                V.Y /= Math.Abs(V.X);
                V.X /= Math.Abs(V.X);
            }
            else
            {
                V.X /= Math.Abs(V.Y);
                V.Y /= Math.Abs(V.Y);
            }
            V.X *= speed;
            V.Y *= speed;
            player.velocity.X += V.X;
            player.velocity.Y += V.Y;
            // inverse moving infection esetén:
            if (!infected && t_imi > 0)
            {
                player.velocity.X *= -1;
                player.velocity.Y *= -1;
                infected = true;
            }

            if (player.velocity.X > 10) player.velocity.X = 10;
            if (player.velocity.Y > 10) player.velocity.Y = 10;
        }

        float GetBoostByGesture()
        {
            if (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                const float MAX_BOOST = 2.5f;
                switch (gesture.GestureType)
                {
                    case GestureType.Flick:
                    return MAX_BOOST;
                }
            }
            return 0.0f;
        }

        void UpdateObjects()
        {
            if (t_imi <= 0)
            {
                gt_sdi.Stop();
                infected = false;
            }
            if (t_sdi <= 0) gt_imi.Stop();
            if (t_sdi > 0) player.R -= 0.03f;  // ha épp elkapott egy size decreasing infectiont, amíg tart a fertőzés, csökken a mérete
            if (t_rageOn >= 5)
            {
                t_rageOn = 0;
                gt_rageOn.Stop();
                rageOn = false;
                player.EndRage(tx_player);
            }

            foreach (Cell e in objects)
            {
                if (e is Enemy && e.R < player.R) e.ChangeTexture(e is IntelligentEnemy ? tx_intellienemy_smaller : tx_enemy_smaller);
                else if (e is Enemy && e.R >= player.R) e.ChangeTexture(e is IntelligentEnemy ? tx_intellienemy_bigger : tx_enemy_bigger);
            }
            player.Update();
            foreach (Cell e in objects)
            {
                if (e is IntelligentEnemy) (e as IntelligentEnemy).ChangeVelocity(objects, player, speed);
                e.Update();
            }
            CollosionObjects();
            if (player.R <= 0 && terminated == 0 && !levelEnd) terminated = 1;
        }

        void CollosionTest(Cell b1, Cell b2)
        {
            float x, k;
            double d; // Distance
            d = Utility.DistanceOrigo(b1, b2);
            k = (float)((b1.R + b2.R) - d) / 2;
            x = 1f;
            if (b1 is Player && b2 is Infection && d <= b1.R + b2.R)
            {
                if (b2 is Rage) se_rage.Play(effectsVolume, 0, 0);
                else se_infection.Play(effectsVolume, 0, 0);
                if (b2 is InverseMoving)
                {
                    t_imi = 5;
                    gt_sdi.Start();
                }
                else if (b2 is SizeDecrease)
                {
                    t_sdi = 5;
                    gt_imi.Start();
                }
                else if (b2 is Rage)
                {
                    player.GoOnRage(tx_rage);
                    rageOn = true;
                    gt_rageOn.Start();
                }
                objects.Remove(b2);
            }
            else if (b2 is AntiMatter && d < b1.R + b2.R)
            {
                b1.R -= k;
                b2.R -= k;
                if (b1 is Player && terminated == 0) CollosionSound();

            }
            else if (d <= b1.R + b2.R)
            {
                if (b1 is Player && rageOn)
                {
                    b1.R += x * 0.2f;
                    b2.R -= x * 0.5f;
                }
                else if (b1.R > b2.R)
                {
                    b1.R += x * 0.2f;
                    b2.R -= x;
                }
                else if (b1.R < b2.R)
                {
                    b2.R += x * 0.2f;
                    b1.R -= x;
                }
                else
                {
                    b1.velocity *= -1;
                    b2.velocity *= -1;
                }
                if (b1 is Player) CollosionSound();
            }
            if (!(b1 is Player) && b1.R <= 0) objects.Remove(b1);
            if (b2.R <= 0) objects.Remove(b2);
            if (b1 is Player && !(b2 is AntiMatter) && b2.R <= 0)
            {
                score += 10;
                HighScores.SetHighScore(score);
                text_score = String.Format(TEXT_SCORE, score);
            }
        }

        void CollosionSound()
        {
            if (t_se == 0)
            {
                se_collosion.Play(effectsVolume, 0, 0);
                gtse.Start();
            }
            else if (t_se >= se_collosion.Duration.TotalMilliseconds)
            {
                t_se = 0;
                gtse.Stop();
            }
        }

        void CollosionObjects()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    if (objects[i] is Infection || objects[j] is Infection) continue;  // az infection-ök csak a playerre vonatkoznak
                    CollosionTest(objects[i], objects[j]);
                }
            }
            for (int i = 0; i < objects.Count; i++)
            {
                CollosionTest(player, objects[i]);
            }
        }

        void IsLevelEnd()
        {
            // ha van olyan ellenség, mely nagyobb a playernél, még nincs vége
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is Enemy && objects[i].R > player.R) return;
            }
            if (terminated == 0)
            {
                levelEnd = true;
                if (!levelCompletedSoundPlayed)
                {
                    if (IsLastLevel()) se_gameCompleted.Play(effectsVolume, 0, 0);
                    else se_levelCompleted.Play(effectsVolume, 0, 0);
                    levelCompletedSoundPlayed = true;
                    text_levelcompleted = String.Format(TEXT_LEVELCOMPLETED, level);
                }
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!backKeyPressed && terminated == 0)
            {
                backKeyPressed = true;
                e.Cancel = true;
            }
        }

    }


}