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
using System.Collections.Generic;

namespace Evolution
{
    class IntelligentEnemy : Enemy
    {
        public IntelligentEnemy(GamePage Game, Texture2D texture, Vector2 center, Vector2 velocity, float radius) : base(Game, texture, center, velocity, radius) { }
        public IntelligentEnemy() { }

        private const float ANTIMATTER_DANGER_DIST = 10;

        public void ChangeVelocity(List<Cell> Objects, Player Player, float speed)
        {
            Cell closestenemy = GetClosestObject(Objects, Player);
            Cell closestAntimaterial = GetClosestAntimatter(Objects);

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

        private Cell GetClosestObject(List<Cell> Objects, Player Player)
        {
            double mindistance = Utility.DistanceEdge(this, Player);
            Cell minobject = Player;
            double distance;

            foreach (Cell obj in Objects)
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

        private AntiMatter GetClosestAntimatter(List<Cell> Objects)
        {
            AntiMatter minantimatter = null;
            double mindistance = -100;
            double distance;

            foreach (Cell obj in Objects)
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