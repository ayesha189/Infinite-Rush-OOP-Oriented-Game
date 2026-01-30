using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace GameFrameWork
{
    // Endless Runner Game - Simple OOP Implementation
    public class EndlessRunnerGameForm : Form
    {
        // Game state
        int px = 425, py = 100;              // Player position
        int ex = 425, ey = 1000;             // Enemy position
        int[] ox = new int[5], oy = new int[5], ot = new int[5]; // Obstacles
        int score = 0, lives = 3, dist = 0;
        float speed = 5;
        bool playing = false, gameover = false;
        
        System.Windows.Forms.Timer gameTimer;
        Random rand = new Random();

        public EndlessRunnerGameForm()
        {
            // Form setup
            Text = "Endless Runner";
            Size = new Size(850, 850);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            BackColor = Color.FromArgb(51, 51, 51);
            KeyDown += OnKeyDown;
            KeyPreview = true;

            // Game timer (60 FPS)
            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += OnGameUpdate;
            gameTimer.Start();
            
            ResetGame();
        }

        // Reset game state
        void ResetGame()
        {
            px = 425; py = 100;
            score = 0; lives = 3; speed = 5; ey = 1000; dist = 0;
            for (int i = 0; i < 5; i++)
            {
                ox[i] = rand.Next(25, 825);
                oy[i] = 850 + (i * 200);
                ot[i] = rand.Next(2);
            }
        }

        // Save score to file
        void SaveScore()
        {
            try {
                string path = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner.txt");
                File.AppendAllText(path, $"Score:{score} Dist:{dist}m {DateTime.Now}\n");
            } catch { }
        }

        // Keyboard input
        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
            
            if (playing)
            {
                if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) px -= 20;
                if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) px += 20;
                if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) py += 20;
                if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) py -= 20;
                px = Math.Max(35, Math.Min(815, px));
                py = Math.Max(50, Math.Min(750, py));
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ResetGame();
                playing = true;
                gameover = false;
            }
        }

        // Game update loop
        void OnGameUpdate(object sender, EventArgs e)
        {
            if (!playing) { Invalidate(); return; }
            
            dist++;

            // Update obstacles
            for (int i = 0; i < 5; i++)
            {
                oy[i] -= (int)speed;

                // Collision check
                if (Math.Abs(px - ox[i]) < 40 && Math.Abs(py - oy[i]) < 40)
                {
                    if (ot[i] == 0) { lives--; AudioManager.PlayHurtSound(); }
                    else { score += 10; AudioManager.PlayCoinSound(); }
                    oy[i] = -100;
                    
                    if (lives == 0) { SaveScore(); playing = false; gameover = true; }
                }

                // Respawn
                if (oy[i] < -50)
                {
                    oy[i] = 900 + rand.Next(200);
                    ox[i] = rand.Next(25, 825);
                    ot[i] = rand.Next(2);
                    score++;
                }
            }

            // Enemy (after score 100)
            if (score > 100)
            {
                ey -= (int)(speed + 1);
                if (ex < px) ex += 2; else if (ex > px) ex -= 2;
                if (ey < -100) { ey = 1000; ex = rand.Next(850); }
                
                if (Math.Abs(px - ex) < 40 && Math.Abs(py - ey) < 40)
                {
                    lives--; ey = 1000; AudioManager.PlayHurtSound();
                    if (lives == 0) { SaveScore(); playing = false; gameover = true; AudioManager.PlayDeathSound(); }
                }
            }

            if (score % 100 == 0 && score > 0) speed += 0.01f;
            Invalidate();
        }

        // Drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (!playing)
            {
                // Menu screen
                DrawText(g, gameover ? "GAME OVER" : "ENDLESS RUNNER", 400, Color.Red, 36);
                if (gameover) {
                    DrawText(g, $"Score: {score}", 450, Color.White, 20);
                    DrawText(g, $"Distance: {dist}m", 490, Color.Gold, 18);
                }
                DrawText(g, "Press ENTER", 550, Color.LimeGreen, 16);
                return;
            }

            // Draw walls
            g.DrawLine(new Pen(Color.White, 5), 20, 0, 20, 850);
            g.DrawLine(new Pen(Color.White, 5), 830, 0, 830, 850);

            // Draw player
            g.FillRectangle(Brushes.DodgerBlue, px - 15, 850 - py - 20, 30, 30);
            g.FillEllipse(Brushes.MistyRose, px - 15, 850 - py - 50, 30, 30);
            g.FillRectangle(Brushes.Black, px - 10, 850 - py - 45, 8, 8);
            g.FillRectangle(Brushes.Black, px + 2, 850 - py - 45, 8, 8);

            // Draw enemy
            if (score > 100)
            {
                g.FillRectangle(Brushes.Red, ex - 15, 850 - ey - 20, 30, 30);
                g.FillEllipse(Brushes.Black, ex - 15, 850 - ey - 50, 30, 30);
            }

            // Draw obstacles/coins
            for (int i = 0; i < 5; i++)
            {
                int y = 850 - oy[i];
                if (ot[i] == 0) g.FillRectangle(Brushes.SaddleBrown, ox[i] - 20, y - 20, 40, 40);
                else g.FillEllipse(Brushes.Gold, ox[i] - 15, y - 15, 30, 30);
            }

            // HUD
            var font = new Font("Arial", 16, FontStyle.Bold);
            g.DrawString($"Score: {score}", font, Brushes.White, 50, 20);
            g.DrawString($"Dist: {dist}m", font, Brushes.Gold, 350, 20);
            g.DrawString($"Lives: {lives}", font, Brushes.Red, 700, 20);
        }

        void DrawText(Graphics g, string text, int y, Color c, int size)
        {
            var font = new Font("Arial", size, FontStyle.Bold);
            var sz = g.MeasureString(text, font);
            g.DrawString(text, font, new SolidBrush(c), (850 - sz.Width) / 2, y);
        }
    }
}
