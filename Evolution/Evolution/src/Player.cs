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
using Microsoft.Xna.Framework.Graphics;

namespace Evolution
{
    class Player : Cell
    {
        public Player(GamePage Game, Texture2D texture, Texture2D RageTexture, Vector2 center, Vector2 velocity, float radius)
            : base(Game, texture, center, velocity, radius)
        {
            onRage = false;
            this.rageTexture = RageTexture;
            rageOpacity = 0.75f;
        }

        private bool onRage;
        private Texture2D rageTexture;
        private readonly float rageOpacity;

        public bool OnRage()
        {
            return onRage;
        }

        public void GoOnRage(Texture2D RageTexture)
        {
            onRage = true;
        }

        public void EndRage(Texture2D OriginalTexture)
        {
            onRage = false;
        }

        public void DrawPlayer(SpriteBatch batch, int level)
        {
            this.Draw(batch, Color.White, 0.9f);
            if(onRage) batch.Draw(rageTexture, topLeft, null, Color.White * rageOpacity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
