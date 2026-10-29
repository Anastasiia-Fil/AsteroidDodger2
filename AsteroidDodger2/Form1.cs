
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AsteroidDodger2
{
    public partial class Form1 : Form
    {
        private readonly Random rand = new Random();
        private List<Asteroid> asteroids;
        private List<Point> stars;
        private int score;
        private int dodged;
        private int hit;
        private int frameCounter;
        private bool gameOver;
        private SpaceShip ship;
        private int elapsedSeconds = 0;



        private readonly Rectangle playArea = new Rectangle(80, 90, 1040, 690);

        private readonly Rectangle scoreboardArea =
    new Rectangle(80, 20, 1040, 55);

        private readonly Rectangle gameOverArea =
    new Rectangle(400, 260, 400, 300);

        private readonly Rectangle restartArea =
    new Rectangle(510, 500, 180, 50);

        public Form1()
        {
            InitializeComponent();

            this.Paint += Paint_object;
            timerAbstroid.Tick += timerAbstroid_Tick;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(1200, 850);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Text = "AsteroidDodger - GDI+ Graphics Demo";
            BackColor = Color.FromArgb(5, 8, 4);
            KeyPreview = true;
            DoubleBuffered = true;

            timerAbstroid.Interval = 16;

            InitializeObject();

            timerAbstroid.Start();
        }

        private void InitializeObject()
       
        {
            score = 0;
            dodged = 0;
            hit = 0;
            frameCounter = 0;
            gameOver = false;

            ship = new SpaceShip(new Point(550, 685));

            asteroids = new List<Asteroid>();
            stars = new List<Point>();
            elapsedSeconds = 0;

            // Generate random stars in the play area
            for (int i = 0; i < 100; i++)
            {
                int x = rand.Next(playArea.Left, playArea.Right);
                int y = rand.Next(playArea.Top, playArea.Bottom);

                stars.Add(new Point(x, y));
            }
        }

        private void AddAstroid(int y)
        {
            int size = rand.Next(28, 72);

            int x = rand.Next(
                playArea.Left + 5,
                playArea.Right - size - 5
            );

            Asteroid asteroid = new Asteroid(new Point(x, y), size);

            asteroid.MoveY = rand.Next(3, 8);

            asteroids.Add(asteroid);
        }


        private void CheckCollisions()
        {
            if (ship == null || asteroids == null)
                return;

            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                Asteroid asteroid = asteroids[i];

                // Check collision between spaceship and asteroid
                if (ship.rect.IntersectsWith(asteroid.rect))
                {
                    hit++;

                    // Remove asteroid after collision
                    asteroids.RemoveAt(i);

                    if (hit >= 3)
                    {
                        gameOver = true;
                        timerAbstroid.Stop();
                        ship.MoveX = 0;
                        break;
                    }
                }
                // Asteroid successfully passed the spaceship
                else if (asteroid.rect.Top > playArea.Bottom)
                {
                    dodged++;
                    score += 10;

                    asteroids.RemoveAt(i);
                }
            }
        }


        private void Paint_object(object sender, PaintEventArgs e)
        {
            // 1. Draw play area border
            using (Pen playPen = new Pen(Color.Cyan, 2))
            {
                e.Graphics.DrawRectangle(playPen, playArea);
            }

            // 2. Draw scoreboard border
            using (Pen scorePen = new Pen(Color.White, 2))
            {
                e.Graphics.DrawRectangle(scorePen, scoreboardArea);
            }

            // 3. Save graphics state for scoreboard
            var scoreState = e.Graphics.Save();

            // Keep scoreboard text inside its rectangle
            e.Graphics.SetClip(scoreboardArea);

            using (Font font = new Font("Arial", 14, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString(
    "Time: " + elapsedSeconds + " sec",
    font, brush, 100, 35);

                e.Graphics.DrawString(
                    "Dodged: " + dodged,
                    font, brush, 350, 35);

                e.Graphics.DrawString(
                    "Hits: " + hit,
                    font, brush, 550, 35);
                e.Graphics.DrawString(
    "Score: " + score,
    font, brush, 800, 35);
            }

            // Restore graphics after scoreboard
            e.Graphics.Restore(scoreState);


            // 4. CLIPPING FOR PLAY AREA STARTS HERE
            var playState = e.Graphics.Save();

            // Restrict drawing to playArea rectangle
            e.Graphics.SetClip(playArea);

            // Draw stars in the background
            if (stars != null)
            {
                using (SolidBrush starBrush = new SolidBrush(Color.White))
                {
                    foreach (Point star in stars)
                    {
                        e.Graphics.FillEllipse(
                            starBrush,
                            star.X,
                            star.Y,
                            2,
                            2
                        );
                    }
                }
            }

            // 5. Draw spaceship
            if (ship != null)
            {
                ship.Draw(e);
            }

            // 6. Draw asteroids
            if (asteroids != null)
            {
                foreach (Asteroid asteroid in asteroids)
                {
                    asteroid.Draw(e);
                }
            }

            // 7. END OF PLAY AREA CLIPPING
            e.Graphics.Restore(playState);

            // Draw Game Over screen on top of the game
            if (gameOver)
            {
                DrawGameOver(e);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Game objects will be drawn here.
        }


        private void timerAbstroid_Tick(object sender, EventArgs e)
        {
            if (gameOver)
                return;

            frameCounter++;

            // Calculate elapsed time using timer ticks
            elapsedSeconds =
                (frameCounter * timerAbstroid.Interval) / 1000;

            // Create asteroids
            if (frameCounter % 40 == 0)
            {
                AddAstroid(playArea.Top);
            }

            // Move asteroids
            foreach (Asteroid asteroid in asteroids)
            {
                asteroid.Center = new Point(
                    asteroid.Center.X,
                    asteroid.Center.Y + asteroid.MoveY
                );

                // Update collision rectangle
                asteroid.rect = new Rectangle(
                    asteroid.Center.X,
                    asteroid.Center.Y,
                    asteroid.rect.Width,
                    asteroid.rect.Height
                );
            }

            // Move spaceship
            if (ship != null)
            {
                int newX = ship.Center.X + ship.MoveX;

                newX = Math.Max(
                    playArea.Left,
                    Math.Min(newX, playArea.Right - 100)
                );

                ship.Center = new Point(newX, ship.Center.Y);

                // Update collision rectangle
                ship.rect = new Rectangle(
                    newX + 20,
                    ship.Center.Y + 8,
                    60,
                    62
                );
            }

            // Check collisions and update score
            CheckCollisions();

            // Redraw
            Invalidate();
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver) return; 
            
            if (ship == null) return;

            if (e.KeyCode == Keys.Left)
            {
                ship.MoveX = -7;
            }
            else if (e.KeyCode == Keys.Right)
            {
                ship.MoveX = 7;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (ship == null) return;

            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                ship.MoveX = 0;
            }
        }

        private void DrawGameOver(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw Game Over panel
            using (SolidBrush background =
                new SolidBrush(Color.FromArgb(20, 30, 45)))
            using (Pen border = new Pen(Color.Cyan, 2))
            {
                g.FillRectangle(background, gameOverArea);
                g.DrawRectangle(border, gameOverArea);
            }

            using (Font titleFont =
                new Font("Arial", 24, FontStyle.Bold))
            using (Font textFont =
                new Font("Arial", 16))
            using (Brush white = new SolidBrush(Color.White))
            using (Brush red = new SolidBrush(Color.LightCoral))
            {
                g.DrawString("GAME OVER", titleFont, red, 485, 290);

                g.DrawString("Time: " + elapsedSeconds + " sec",
                    textFont, white, 450, 350);

                g.DrawString("Dodged: " + dodged,
                    textFont, white, 450, 385);

                g.DrawString("Hits: " + hit + "/3",
                    textFont, white, 450, 420);

                g.DrawString("Score: " + score,
                    textFont, white, 450, 455);
            }

            // Draw Restart button
            using (SolidBrush buttonBrush =
                new SolidBrush(Color.SteelBlue))
            using (Font buttonFont =
                new Font("Arial", 13, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(buttonBrush, restartArea);

                g.DrawString("RESTART GAME",
     buttonFont, textBrush, 520, 514);
            }
        }
        private void RestartGame()
        {
            timerAbstroid.Stop();

            InitializeObject();

            timerAbstroid.Start();

            Invalidate();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameOver && restartArea.Contains(e.Location))
            {
                RestartGame();
            }
        }
    }
}
