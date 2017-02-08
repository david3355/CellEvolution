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

namespace Evolution
{
    public class Utility
    {
        /// <summary>
        /// Returns the distance of the ORIGO of two objects
        /// </summary>
        public static double DistanceOrigo(Cell Object1, Cell Object2)
        {
            return Distance(Object1.Origo, Object2.Origo);
        }

        /// <summary>
        /// Returns the distance of the EDGE of two objects
        /// </summary>
        public static double DistanceEdge(Cell Object1, Cell Object2)
        {
            return DistanceOrigo(Object1, Object2) - (Object1.R + Object2.R);
        }

        public static double DistanceEdge(Vector2 Origo1, double Radius1, Vector2 Origo2, double Radius2)
        {
            return Distance(Origo1, Origo2) - (Radius1 + Radius2);
        }

        /// <summary>
        /// Returns the distance of the ORIGO of two objects
        /// </summary>
        public static double Distance(Vector2 Point1, Vector2 Point2)
        {
            return Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
        }

        public static readonly Random Rnd = new Random();

        public static float RandomDouble(double Minimum, double Maximum)
        {
            return (float)(Rnd.NextDouble() * (Maximum - Minimum) + Minimum);
        }

        public static bool CircleContainsPosition(Vector2 CircleTopLeftPosition, double Width, Vector2 Position)
        {
            return CircleTopLeftPosition.X < Position.X && Position.X < CircleTopLeftPosition.X + Width && CircleTopLeftPosition.Y < Position.Y && Position.Y < CircleTopLeftPosition.Y + Width;
        }

        public static Vector2 GetNormalizedDirectionVector(Vector2 VectorOrigo, Vector2 DirectionPoint)
        {
            Vector2 vector = GetDirectionVector(VectorOrigo, DirectionPoint);
            return GetNormalizedVector(vector, (float)Utility.Distance(VectorOrigo, DirectionPoint));
        }

        public static Vector2 GetDirectionVector(Vector2 VectorOrigo, Vector2 DirectionPoint)
        {
            return new Vector2(DirectionPoint.X - VectorOrigo.X, DirectionPoint.Y - VectorOrigo.Y);
        }

        public static Vector2 GetNormalizedVector(Vector2 Vector, float VectorLength)
        {
            return new Vector2(Vector.X / VectorLength, Vector.Y / VectorLength);
        }

        /// <summary>
        /// Rotates a point around a pivot point with an angle
        /// </summary>
        public static Vector2 RotatePoint(Vector2 Pivot, Vector2 Point, float Angle)
        {
            float sin = (float)Math.Sin(MathHelper.ToRadians(Angle));
            float cos = (float)Math.Cos(MathHelper.ToRadians(Angle));
            Point.X -= Pivot.X;
            Point.Y -= Pivot.Y;
            double newx = Point.X * cos - Point.Y * sin;
            double newy = Point.X * sin + Point.Y * cos;
            return new Vector2((float)newx + Pivot.X, (float)newy + Pivot.Y);
        }

        public static float CalculateTextureGap(float Radius)
        {
            if (Radius < 2) return 0;
            return Radius * 0.1185f;
        }


    }
}
