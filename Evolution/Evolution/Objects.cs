using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;



namespace Evolution
{
    public class Objects
    {
        protected GamePage game;
        protected Texture2D texture;
        protected Vector2 topLeft;
        public Vector2 velocity;

        public Vector2 Origo
        {
            get
            {
                return new Vector2(topLeft.X + radius, topLeft.Y + radius);
            }
        }

        private float radius;

        public float R
        {
            get { return radius; }
            set
            {
                radius = value;
                CalculateScale();
            }
        }
        protected float scale;

        public Objects()
        {
        }

        public Objects(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius)
        {
            SetAttributes(Game, texture, center, velocity, radius);
        }

        public void SetAttributes(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius)
        {
            this.game = Game;
            this.texture = texture;
            this.topLeft = new Vector2(center.X - radius, center.Y - radius);
            this.velocity = velocity;
            this.radius = radius;
            CalculateScale();
        }

        void CalculateScale()
        {
            float width = (float)texture.Bounds.Width;
            this.scale = (this.radius * 2) / width;
            //topLeft.X = Origo.X - radius;
            //topLeft.Y = Origo.Y - radius;
            BounceBall();
        }

        public void Draw(SpriteBatch batch)
        {
            //batch.Begin();
            batch.Draw(texture, topLeft, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //batch.End();
        }

        public void Update()
        {
            topLeft += velocity;
            BounceBall();
        }

        public void BounceBall()
        {
            Vector2 newTopLeft = topLeft + velocity;
            float left, right, top, bottom;
            left = newTopLeft.X;
            right = newTopLeft.X + (radius * 2);
            top = newTopLeft.Y;
            bottom = newTopLeft.Y + (radius * 2);

            if (top < 0 || bottom > game.ActualHeight)
            {
                velocity.Y *= -1;                
            }

            if (left < 0 || right > game.ActualWidth)
            {
                velocity.X *= -1;                
            }
        }

        public void ChangeTexture(Texture2D Texture)
        {
            texture = Texture;
        }

    }

    class Player : Objects
    {
        public Player(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        
    }
    class Enemy : Objects
    {
        public Enemy(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public Enemy() { }
    }
    class AntiMatter : Objects
    {
        public AntiMatter(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public AntiMatter() { }
    }
    class Infection : Objects
    {
        public Infection(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public Infection() { }
    }
    class SizeDecrease : Infection
    {
        public SizeDecrease(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public SizeDecrease() { }
    }
    class InverseMoving : Infection
    {
        public InverseMoving(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public InverseMoving() { }
    }
    class Rage : Infection
    {
        public Rage(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public Rage() { }
    }
}
