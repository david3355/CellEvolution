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
    public class Objects
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
                CalculateScale();
            }
        }
        protected float scale;

        public Objects()
        {
            id = Guid.NewGuid().ToString();
        }

        public Objects(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius)
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

        public virtual void Update()
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

        public override bool Equals(object obj)
        {
            if (obj is Objects)
            {
                if ((obj as Objects).id.Equals(this.id)) return true;
            }
            return false;
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


    class IntelligentEnemy : Enemy
    {
        public IntelligentEnemy(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public IntelligentEnemy() { }

        private const float ANTIMATTER_DANGER_DIST = 10;

        public void ChangeVelocity(List<Objects> Objects, Player Player, float speed)
        {
            Objects closestenemy = GetClosestObject(Objects, Player);
            Objects closestAntimaterial = GetClosestAntimatter(Objects);

            Vector2 velocity;

            if (closestAntimaterial != null && Utility.DistanceEdge(this, closestAntimaterial) <= ANTIMATTER_DANGER_DIST) velocity = GetFleeVector(closestAntimaterial.Origo, this.Origo);
            else if (closestenemy.R >= this.R) velocity = GetFleeVector(closestenemy.Origo, this.Origo);
            else velocity = GetFollowVector(this.Origo, closestenemy.Origo);
            velocity.X *= speed;
            velocity.Y *= speed;
            this.velocity = velocity;
        }

        private Vector2 GetFleeVector(Vector2 EnemyOrigo, Vector2 SelfOrigo)
        {
            return GetRandomFleeDirection(EnemyOrigo, SelfOrigo, 60);
        }

        private Vector2 GetFollowVector(Vector2 SelfOrigo, Vector2 PreyOrigo)
        {
            return GetNormalizedDirectionVector(SelfOrigo, PreyOrigo);
        }

        private Objects GetClosestObject(List<Objects> Objects, Player Player)
        {
            double mindistance = Utility.DistanceEdge(this, Player);
            Objects minobject = Player;
            double distance;

            foreach (Objects obj in Objects)
            {
                if (obj is Enemy)
                {
                    if (!obj.Equals(this))
                    {
                        distance = Utility.DistanceEdge(this, obj);
                        if (distance < mindistance)
                        {
                            mindistance = distance;
                            minobject = obj;
                        }
                    }
                }
            }
            return minobject;
        }

        private AntiMatter GetClosestAntimatter(List<Objects> Objects)
        {
            AntiMatter minantimatter = null;
            double mindistance = -100;
            double distance;

            foreach (Objects obj in Objects)
            {
                if (obj is AntiMatter)
                {
                    distance = Utility.DistanceEdge(this, obj);
                    if (distance < mindistance)
                    {
                        mindistance = distance;
                        minantimatter = (AntiMatter)obj;
                    }
                }
            }
            return minantimatter;
        }

        private Vector2 GetNormalizedDirectionVector(Vector2 VectorOrigo, Vector2 DirectionPoint)
        {
            Vector2 vector = GetDirectionVector(VectorOrigo, DirectionPoint);
            return GetNormalizedVector(vector, (float)Utility.Distance(VectorOrigo, DirectionPoint));
        }

        private Vector2 GetDirectionVector(Vector2 VectorOrigo, Vector2 DirectionPoint)
        {
            return new Vector2(DirectionPoint.X - VectorOrigo.X, DirectionPoint.Y - VectorOrigo.Y);
        }

        private Vector2 GetNormalizedVector(Vector2 Vector, float VectorLength)
        {
            return new Vector2(Vector.X / VectorLength, Vector.Y / VectorLength);
        }

        private Vector2 GetRandomFleeDirection(Vector2 VectorOrigo, Vector2 DirectionPoint, int VariationDegree)
        {
            float randomangle = Utility.Rnd.Next(-VariationDegree, VariationDegree + 1);
            Vector2 directionVector = GetDirectionVector(VectorOrigo, DirectionPoint);
            Vector2 rotatedDirectionPoint = RotatePoint(new Vector2(0, 0), directionVector, randomangle);
            return GetNormalizedVector(rotatedDirectionPoint, (float)Utility.Distance(VectorOrigo, DirectionPoint));
        }

        /// <summary>
        /// Rotates a point around a pivot point with an angle
        /// </summary>
        static Vector2 RotatePoint(Vector2 Pivot, Vector2 Point, float Angle)
        {
            float sin = (float)Math.Sin(MathHelper.ToRadians(Angle));
            float cos = (float)Math.Cos(MathHelper.ToRadians(Angle));
            Point.X -= Pivot.X;
            Point.Y -= Pivot.Y;
            double newx = Point.X * cos - Point.Y * sin;
            double newy = Point.X * sin + Point.Y * cos;
            return new Vector2((float)newx + Pivot.X, (float)newy + Pivot.Y);
        }

        public override void Update()
        {
            if (topLeft.X + velocity.X <= 0) topLeft.X = 0;
            else if (topLeft.X + R * 2 + velocity.X >= game.ActualWidth) topLeft.X = (float)game.ActualWidth - R * 2;
            else topLeft.X += velocity.X;

            if (topLeft.Y + velocity.Y <= 0) topLeft.Y = 0;
            else if (topLeft.Y + R * 2 + velocity.X >= game.ActualHeight) topLeft.Y = (float)game.ActualHeight - R * 2;
            else topLeft.Y += velocity.Y;
        }
    }
}
