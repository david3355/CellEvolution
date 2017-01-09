using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Evolution
{
    class Player : Cell
    {
        public Player(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) 
        {
            onRage = false;
        }

        private bool onRage;

        public bool OnRage()
        {
            return onRage;
        }

        public void GoOnRage(Texture2D RageTexture)
        {
            onRage = true;
            ChangeTexture(RageTexture);
        }

        public void EndRage(Texture2D OriginalTexture)
        {
            onRage = false;
            ChangeTexture(OriginalTexture);
        }
    }
}
