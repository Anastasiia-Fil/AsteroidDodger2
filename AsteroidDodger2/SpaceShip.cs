using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsteroidDodger2
{
    public class SpaceShip : Asset
    {
        public SpaceShip(Point center)
        {
            Center = center;
            rect = new Rectangle(
                Center.X + 20,
                Center.Y + 8,
                60,
                62
            );
        }

        public override void Draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            GraphicsState state = g.Save();

            g.TranslateTransform(Center.X, Center.Y);

            Point[] shipBody =
            {
                new Point(50, 0),
                new Point(65, 25),
                new Point(80, 50),
                new Point(100, 70),
                new Point(70, 65),
                new Point(50, 80),
                new Point(30, 65),
                new Point(0, 70),
                new Point(20, 50),
                new Point(35, 25)
            };

            using (SolidBrush bodyBrush =
                new SolidBrush(Color.SteelBlue))
            using (Pen outline =
                new Pen(Color.White, 2))
            {
                g.FillPolygon(bodyBrush, shipBody);
                g.DrawPolygon(outline, shipBody);
            }

            using (SolidBrush cockpit =
                new SolidBrush(
                    Color.FromArgb(190, 170, 240, 255)))
            using (Pen cockpitOutline =
                new Pen(Color.White, 2))
            {
                g.FillEllipse(cockpit, 37, 22, 26, 25);
                g.DrawEllipse(cockpitOutline, 37, 22, 26, 25);
            }

            using (Pen detail = new Pen(Color.Cyan, 2))
            {
                g.DrawLine(detail, 22, 48, 38, 58);
                g.DrawLine(detail, 78, 48, 62, 58);
            }

            g.Restore(state);

            rect = new Rectangle(
                Center.X + 20,
                Center.Y + 8,
                60,
                62
            );
        }
    }
}