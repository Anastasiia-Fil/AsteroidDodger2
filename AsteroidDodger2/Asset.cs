using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsteroidDodger2
{
    public abstract class Asset
    {
        public Point Center { get; set; }

        public Rectangle rect { get; set; }

        public int MoveX { get; set; } = 0;

        public int MoveY { get; set; } = 0;

        public int MoveZ { get; set; } = 0;

        public abstract void Draw(PaintEventArgs e);
    }
}