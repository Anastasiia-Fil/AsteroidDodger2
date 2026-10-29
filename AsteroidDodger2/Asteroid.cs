
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsteroidDodger2
{
    public class Asteroid : Asset
    {
        private static readonly Random random = new Random();

        public int Radius { get; set; }

        public int CraterSeed { get; private set; }

        public Asteroid(Point center, int radius)
        {
            Center = center;
            Radius = radius;
            CraterSeed = random.Next(1000);
        }

        public override void Draw(PaintEventArgs e)
        {
            rect = new Rectangle(
                Center.X,
                Center.Y,
                Radius,
                Radius
            );

            Graphics g = e.Graphics;

            using (SolidBrush body =
                new SolidBrush(Color.FromArgb(255, 105, 105, 115))) 
            using (Pen outline =
    new Pen(Color.FromArgb(220, 150, 150, 150)))
            {
                g.FillEllipse(body, rect);
                g.DrawEllipse(outline, rect);
            }

            Random craterRandom = new Random(CraterSeed);

            for (int i = 0; i < 3; i++)
            {
                int craterSize = Math.Max(
                    5,
                    Radius / craterRandom.Next(4, 7)
                );

                int maxOffset = Math.Max(
                    7,
                    Radius - craterSize - 4
                );

                int cx = Center.X +
                    craterRandom.Next(6, maxOffset);

                int cy = Center.Y +
                    craterRandom.Next(6, maxOffset);

                using (SolidBrush crater =
                    new SolidBrush(Color.FromArgb(150, 45, 45, 55)))
                {
                    g.FillEllipse(
                        crater,
                        cx,
                        cy,
                        craterSize,
                        craterSize
                    );
                }
            }
        }
    }
}
