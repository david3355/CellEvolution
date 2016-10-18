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

        /// <summary>
        /// Returns the distance of the ORIGO of two objects
        /// </summary>
        public static double Distance(Vector2 Point1, Vector2 Point2)
        {
            return Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
        }

        public static readonly Random Rnd = new Random();
    }
}
