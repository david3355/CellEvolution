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
using System.Collections.Generic;


namespace Evolution
{
    public class Cell
    {
        protected GamePage game;
        protected Texture2D texture;
        protected Vector2 topLeft;
        public Vector2 velocity;
        private String id;

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
                ScaleAndBounce();
            }
        }
        protected float scale;

        public Cell()
        {
            id = Guid.NewGuid().ToString();
        }

        public Cell(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius)
            : this()
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
        }

        void ScaleAndBounce()
        {
            CalculateScale();
            BounceBall();
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, topLeft, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public virtual void Update()
        {
            BounceBall();
            topLeft += velocity;
        }

        public void BounceBall()
        {
            Vector2 newTopLeft = topLeft + velocity;
            float left, right, top, bottom;
            left = newTopLeft.X;
            right = newTopLeft.X + (radius * 2);
            top = newTopLeft.Y;
            bottom = newTopLeft.Y + (radius * 2);

            if (left < 0) topLeft.X = 0;
            if (right > game.ActualWidth) topLeft.X = (float)game.ActualWidth - radius * 2;
            if (left < 0 || right > game.ActualWidth)
            {
                velocity.X *= -1;
            }

            if (top < 0) topLeft.Y = 0;
            if(bottom > game.ActualHeight) topLeft.Y = (float)game.ActualHeight - radius * 2;
            if (top < 0 || bottom > game.ActualHeight)
            {
                velocity.Y *= -1;
            }
        }

        public void ChangeTexture(Texture2D Texture)
        {
            texture = Texture;
        }

        public override bool Equals(object obj)
        {
            if (obj is Cell)
            {
                if ((obj as Cell).id.Equals(this.id)) return true;
            }
            return false;
        }

    }
}
