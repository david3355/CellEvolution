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

        public override Cell CreateInstance()
        {
            return new IntelligentEnemy();
        }

        public virtual void ChangeVelocity(List<Cell> Objects, Player Player, float speed)
        {
            Cell closestenemy = GetClosestObject(Objects, Player);
            if (closestenemy == null) return;
            Cell closestAntimaterial = GetClosestAntimatter(Objects);

            Vector2 velocity;
            float antimatter_danger_dist = -100;
            if (closestAntimaterial != null)
            {
                if (closestAntimaterial.R >= this.R) antimatter_danger_dist = 6;
                else if (this.R * 0.6 <= closestAntimaterial.R) antimatter_danger_dist = 3;
            }
            if (closestAntimaterial != null && Utility.DistanceEdge(this, closestAntimaterial) <= antimatter_danger_dist) velocity = GetFleeVector(closestAntimaterial.Origo, this.Origo);
            else if (closestenemy.R >= this.R) velocity = GetFleeVector(closestenemy.Origo, this.Origo);
            else
            {
                List<Cell> exceptions = new List<Cell>();
                while (!CanTouchObject(closestenemy))
                {
                    if (!exceptions.Contains(closestenemy)) exceptions.Add(closestenemy);
                    closestenemy = GetClosestObjectExcept(Objects, Player, exceptions);
                    if (closestenemy == null) return;
                }
                exceptions.Clear();
                velocity = GetFollowVector(this.Origo, closestenemy.Origo);
            }
            velocity.X *= speed;
            velocity.Y *= speed;
            this.velocity = velocity;
        }

        /// <summary>
        /// Check if the actual object can touch the object given as parameter (depending whether the object is in corner or not)
        /// </summary>
        private bool CanTouchObject(Cell Object)
        {
            Corner corner = Object.IsCornered();
            if (corner == Corner.NoCorner) return true;
            float changeX, changeY;
            switch (corner)
            {
                case Corner.TopLeft:
                changeX = -1; changeY = -1; break;
                case Corner.TopRight:
                changeX = 1; changeY = -1; break;
                case Corner.BottomLeft:
                changeX = -1; changeY = 1; break;
                case Corner.BottomRight:
                changeX = 1; changeY = 1; break;
                default: return true;
            }
            Vector2 cornerPoint = new Vector2(Object.Origo.X + Object.R * changeX, Object.Origo.Y + Object.R * changeY);
            Vector2 chaserTestOrigo = new Vector2(cornerPoint.X + this.R * -changeX, cornerPoint.Y + this.R * -changeY);
            return Utility.Distance(Object.Origo, chaserTestOrigo) < Object.R - Utility.CalculateTextureGap(Object.R) + this.R - Utility.CalculateTextureGap(this.R);
        }

        private Vector2 GetFleeVector(Vector2 EnemyOrigo, Vector2 SelfOrigo)
        {
            return GetRandomFleeDirection(EnemyOrigo, SelfOrigo, 60);
        }

        private Vector2 GetFollowVector(Vector2 SelfOrigo, Vector2 PreyOrigo)
        {
            return Utility.GetNormalizedDirectionVector(SelfOrigo, PreyOrigo);
        }

        private Cell GetClosestObject(List<Cell> Objects, Player Player)
        {
            return GetClosestObjectExcept(Objects, Player, new List<Cell>());
        }

        private Cell GetClosestObjectExcept(List<Cell> Objects, Player Player, List<Cell> Exceptions)
        {
            double mindistance = 10000;
            Cell minobject = null;
            double distance = Utility.DistanceEdge(this, Player);
            if (distance < mindistance && !Exceptions.Contains(Player))
            {
                mindistance = distance;
                minobject = Player;
            }

            foreach (Cell obj in Objects)
            {
                if (obj is Enemy)
                {
                    if (!obj.Equals(this))
                    {
                        distance = Utility.DistanceEdge(this, obj);
                        if (distance < mindistance && !Exceptions.Contains(obj))
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
            double mindistance = 10000;
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

        private Vector2 GetRandomFleeDirection(Vector2 VectorOrigo, Vector2 DirectionPoint, int VariationDegree)
        {
            float randomangle = Utility.Rnd.Next(-VariationDegree, VariationDegree + 1);
            Vector2 directionVector = Utility.GetDirectionVector(VectorOrigo, DirectionPoint);
            Vector2 rotatedDirectionPoint = Utility.RotatePoint(new Vector2(0, 0), directionVector, randomangle);
            return Utility.GetNormalizedVector(rotatedDirectionPoint, (float)Utility.Distance(VectorOrigo, DirectionPoint));
        }


        public override void Update()
        {
            float textureGap = Utility.CalculateTextureGap(R);
            float left_new, right_new, top_new, bottom_new;
            left_new = topLeft.X + velocity.X;
            right_new = topLeft.X + (R * 2) + velocity.X;
            top_new = topLeft.Y + velocity.Y;
            bottom_new = topLeft.Y + (R * 2) + velocity.Y;
            if (left_new + textureGap <= 0) topLeft.X = 0 - textureGap;
            else if (right_new - textureGap >= game.ActualWidth) topLeft.X = (float)game.ActualWidth - R * 2 + textureGap;
            else topLeft.X += velocity.X;

            if (top_new + textureGap <= 0) topLeft.Y = 0 - textureGap;
            else if (bottom_new - textureGap >= game.ActualHeight) topLeft.Y = (float)game.ActualHeight - R * 2 + textureGap;
            else topLeft.Y += velocity.Y;
        }
    }
}