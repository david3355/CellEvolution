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
    public class PreparedCell : IComparable<PreparedCell>
    {
        public PreparedCell(float Radius, Cell CellType, Texture2D Texture)
        {
            this.Radius = Radius;
            this.CellType = CellType;
            this.Texture = Texture;
        }

        public float Radius { get; set; }
        public Cell CellType { get; set; }
        public Texture2D Texture { get; set; }

        public int CompareTo(PreparedCell other)
        {
            if (this.Radius < other.Radius) return 1;
            else if (this.Radius > other.Radius) return -1;
            return 0;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}", Radius, CellType);
        }
    }
}
