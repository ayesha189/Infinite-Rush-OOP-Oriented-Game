using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace GameFrameWork
{
    // Animated Endless Runner - Car Racing Game
    public class AnimatedEndlessRunnerForm : Form
    {
        // Game constants
        const int ROAD_LEFT = 150;
        const int ROAD_RIGHT = 650;
        const int ROAD_WIDTH = 500;
        const int LANE_WIDTH = 166;
        const int LANE_COUNT = 3;
        
        // Player state
        int playerLane = 1; // 0=left, 1=center, 2=right
        float playerX, playerY;
        float targetPlayerX;
        int lives = 3;
        int score = 0;
        float distance = 0;
        float speed = 8;
        bool isExploding = false;
        int explosionFrame = 0;
        
        // Game state
        bool playing = false;
        bool gameover = false;
        float roadOffset = 0;
        float lightOffset = 0;
        float timeOfDay = 0; // 0-1 for day/night cycle
        
        // Enemy cars
        List<EnemyCar> enemyCars = new List<EnemyCar>();
        
        // Coins
        List<Coin> coins = new List<Coin>();
        

        
        // Particles
        List<Particle> particles = new List<Particle>();
        
        System.Windows.Forms.Timer gameTimer;
        Random rand = new Random();
        
        // Animation frame counter
        int frameCount = 0;
        
        // Difficulty settings
        int difficultyLevel = 1; // 1=Easy, 2=Medium, 3=Hard
        float baseSpeed = 6;
        int enemySpawnRate = 80;
        string difficultyName = "EASY";

        public AnimatedEndlessRunnerForm(int level = 1)
        {
            difficultyLevel = level;
            
            // Set difficulty parameters
            switch (level)
            {
                case 1: // Easy
                    baseSpeed = 6;
                    enemySpawnRate = 90;
                    difficultyName = "EASY";
                    break;
                case 2: // Medium
                    baseSpeed = 10;
                    enemySpawnRate = 60;
                    difficultyName = "MEDIUM";
                    break;
                case 3: // Hard
                    baseSpeed = 14;
                    enemySpawnRate = 40;
                    difficultyName = "HARD";
                    break;
            }
            
            Text = $"🚗 Car Runner - {difficultyName}";
            Size = new Size(850, 900);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            BackColor = Color.FromArgb(30, 30, 40);
            KeyDown += OnKeyDown;
            KeyPreview = true;

            gameTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
            gameTimer.Tick += OnGameUpdate;
            gameTimer.Start();
            
            InitializeGame();
        }

        void InitializeGame()
        {
            playerY = 700;
            playerX = GetLaneX(1);
            targetPlayerX = playerX;
            

        }

        void ResetGame()
        {
            playerLane = 1;
            playerX = GetLaneX(1);
            targetPlayerX = playerX;
            playerY = 700;
            lives = 3;
            score = 0;
            distance = 0;
            speed = baseSpeed;
            isExploding = false;
            explosionFrame = 0;
            roadOffset = 0;
            timeOfDay = 0;
            
            enemyCars.Clear();
            coins.Clear();
            particles.Clear();
            
            InitializeGame();
        }

        float GetLaneX(int lane)
        {
            return ROAD_LEFT + (lane * LANE_WIDTH) + LANE_WIDTH / 2;
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
            
            if (playing && !isExploding)
            {
                if ((e.KeyCode == Keys.A || e.KeyCode == Keys.Left) && playerLane > 0)
                {
                    playerLane--;
                    targetPlayerX = GetLaneX(playerLane);
                }
                if ((e.KeyCode == Keys.D || e.KeyCode == Keys.Right) && playerLane < 2)
                {
                    playerLane++;
                    targetPlayerX = GetLaneX(playerLane);
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ResetGame();
                playing = true;
                gameover = false;
            }
        }

        void OnGameUpdate(object sender, EventArgs e)
        {
            frameCount++;
            
            if (!playing)
            {
                // Animate menu background
                roadOffset += 2;
                if (roadOffset > 100) roadOffset = 0;
                Invalidate();
                return;
            }

            // Smooth player movement
            playerX += (targetPlayerX - playerX) * 0.15f;
            
            // Update road animation
            roadOffset += speed;
            if (roadOffset > 100) roadOffset = 0;
            
            // Update distance and speed
            distance += speed * 0.1f;
            if (frameCount % 300 == 0) speed += 0.3f;
            
            // Day/night cycle
            timeOfDay = (float)((Math.Sin(frameCount * 0.001) + 1) / 2);
            
            // Spawn enemy cars
            if (frameCount % enemySpawnRate == 0)
            {
                int lane = rand.Next(3);
                enemyCars.Add(new EnemyCar
                {
                    X = GetLaneX(lane),
                    Y = -100,
                    Lane = lane,
                    Type = rand.Next(4), // 0=car, 1=truck, 2=taxi, 3=police
                    Speed = 3 + rand.Next(3)
                });
            }
            
            // Spawn coins
            if (frameCount % 90 == 0)
            {
                int lane = rand.Next(3);
                coins.Add(new Coin
                {
                    X = GetLaneX(lane),
                    Y = -50,
                    Rotation = 0
                });
            }
            
            // Update enemy cars
            for (int i = enemyCars.Count - 1; i >= 0; i--)
            {
                enemyCars[i].Y += speed - enemyCars[i].Speed;
                
                // Collision check
                if (!isExploding && Math.Abs(playerX - enemyCars[i].X) < 50 && 
                    Math.Abs(playerY - enemyCars[i].Y) < 70)
                {
                    lives--;
                    isExploding = true;
                    explosionFrame = 0;
                    AudioManager.PlayHurtSound();
                    
                    // Spawn explosion particles
                    for (int p = 0; p < 20; p++)
                    {
                        particles.Add(new Particle
                        {
                            X = playerX,
                            Y = playerY,
                            VX = (float)(rand.NextDouble() * 10 - 5),
                            VY = (float)(rand.NextDouble() * 10 - 5),
                            Life = 30,
                            Color = Color.FromArgb(255, rand.Next(200, 255), rand.Next(100))
                        });
                    }
                    
                    if (lives <= 0)
                    {
                        SaveScore();
                        playing = false;
                        gameover = true;
                        AudioManager.PlayDeathSound();
                    }
                    
                    enemyCars.RemoveAt(i);
                    continue;
                }
                
                // Remove off-screen
                if (enemyCars[i].Y > 950) enemyCars.RemoveAt(i);
            }
            
            // Update coins
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                coins[i].Y += speed;
                coins[i].Rotation += 5;
                
                // Collect coin
                if (Math.Abs(playerX - coins[i].X) < 40 && Math.Abs(playerY - coins[i].Y) < 40)
                {
                    score += 10;
                    AudioManager.PlayCoinSound();
                    
                    // Spawn sparkle particles
                    for (int p = 0; p < 10; p++)
                    {
                        particles.Add(new Particle
                        {
                            X = coins[i].X,
                            Y = coins[i].Y,
                            VX = (float)(rand.NextDouble() * 6 - 3),
                            VY = (float)(rand.NextDouble() * 6 - 3),
                            Life = 20,
                            Color = Color.Gold
                        });
                    }
                    
                    coins.RemoveAt(i);
                    continue;
                }
                
                if (coins[i].Y > 950) coins.RemoveAt(i);
            }
            

            
            // Update particles
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].X += particles[i].VX;
                particles[i].Y += particles[i].VY;
                particles[i].Life--;
                if (particles[i].Life <= 0) particles.RemoveAt(i);
            }
            
            // Handle explosion animation
            if (isExploding)
            {
                explosionFrame++;
                if (explosionFrame > 30)
                {
                    isExploding = false;
                    if (lives > 0)
                    {
                        playerLane = 1;
                        playerX = GetLaneX(1);
                        targetPlayerX = playerX;
                    }
                }
            }
            
            Invalidate();
        }

        void SaveScore()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner.txt");
                File.AppendAllText(path, $"Score:{score} Dist:{(int)distance}m {DateTime.Now}\n");
            }
            catch { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw sky gradient (day/night)
            DrawSky(g);
            
            // Draw grass/environment
            DrawEnvironment(g);
            
            // Draw road
            DrawRoad(g);
            

            
            // Draw coins
            DrawCoins(g);
            
            // Draw enemy cars
            DrawEnemyCars(g);
            
            // Draw player car
            if (!isExploding || explosionFrame % 6 < 3)
                DrawPlayerCar(g);
            
            // Draw particles
            DrawParticles(g);
            
            // Draw speed lines at high speed
            if (speed > 12)
                DrawSpeedLines(g);
            
            // Draw HUD
            DrawHUD(g);
            
            // Draw menu/game over
            if (!playing)
                DrawMenu(g);
        }

        void DrawSky(Graphics g)
        {
            // Clamp helper function
            int Clamp(int val) => Math.Max(0, Math.Min(255, val));
            
            Color skyTop = Color.FromArgb(
                Clamp((int)(135 - timeOfDay * 115)),
                Clamp((int)(206 - timeOfDay * 180)),
                Clamp((int)(235 - timeOfDay * 200)));
            Color skyBottom = Color.FromArgb(
                Clamp((int)(255 - timeOfDay * 200)),
                Clamp((int)(165 - timeOfDay * 140)),
                Clamp((int)(0 + timeOfDay * 40)));
            
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, 850, 200), skyTop, skyBottom, 90))
            {
                g.FillRectangle(brush, 0, 0, 850, 200);
            }
        }

        void DrawEnvironment(Graphics g)
        {
            // Left grass
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, ROAD_LEFT, 900),
                Color.ForestGreen, Color.DarkGreen, LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, 0, 0, ROAD_LEFT - 10, 900);
            }
            
            // Right grass
            using (var brush = new LinearGradientBrush(
                new Rectangle(ROAD_RIGHT, 0, 200, 900),
                Color.DarkGreen, Color.ForestGreen, LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, ROAD_RIGHT + 10, 0, 200, 900);
            }
            
            // Road barriers
            for (int i = 0; i < 20; i++)
            {
                float y = (i * 50 + roadOffset) % 950;
                // Left barrier
                g.FillRectangle(Brushes.Red, ROAD_LEFT - 10, y, 10, 25);
                g.FillRectangle(Brushes.White, ROAD_LEFT - 10, y + 25, 10, 25);
                // Right barrier
                g.FillRectangle(Brushes.Red, ROAD_RIGHT, y, 10, 25);
                g.FillRectangle(Brushes.White, ROAD_RIGHT, y + 25, 10, 25);
            }
        }

        void DrawRoad(Graphics g)
        {
            // Road surface
            using (var brush = new LinearGradientBrush(
                new Rectangle(ROAD_LEFT, 0, ROAD_WIDTH, 900),
                Color.FromArgb(50, 50, 55), Color.FromArgb(40, 40, 45), LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, ROAD_LEFT, 0, ROAD_WIDTH, 900);
            }
            
            // Lane markings (dashed white lines)
            using (var pen = new Pen(Color.White, 4) { DashStyle = DashStyle.Custom, DashPattern = new float[] { 3, 3 } })
            {
                for (int lane = 1; lane < LANE_COUNT; lane++)
                {
                    float x = ROAD_LEFT + lane * LANE_WIDTH;
                    for (int i = 0; i < 15; i++)
                    {
                        float y = (i * 80 + roadOffset) % 1000 - 100;
                        g.FillRectangle(Brushes.White, x - 2, y, 4, 40);
                    }
                }
            }
            
            // Road edge lines (solid yellow)
            g.DrawLine(new Pen(Color.Yellow, 4), ROAD_LEFT + 5, 0, ROAD_LEFT + 5, 900);
            g.DrawLine(new Pen(Color.Yellow, 4), ROAD_RIGHT - 5, 0, ROAD_RIGHT - 5, 900);
        }



        void DrawPlayerCar(Graphics g)
        {
            float x = playerX;
            float y = playerY;
            
            // Car shadow
            g.FillEllipse(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), x - 35, y + 30, 70, 20);
            
            // Car body
            var carColor = Color.FromArgb(220, 30, 30); // Red sports car
            var carDark = Color.FromArgb(180, 20, 20);
            
            // Main body
            using (var brush = new LinearGradientBrush(
                new Rectangle((int)(x - 30), (int)(y - 50), 60, 100),
                carColor, carDark, LinearGradientMode.Vertical))
            {
                // Body shape
                g.FillRectangle(brush, x - 28, y - 40, 56, 85);
            }
            
            // Hood (front)
            g.FillEllipse(new SolidBrush(carColor), x - 25, y - 55, 50, 30);
            
            // Roof
            g.FillRectangle(new SolidBrush(Color.FromArgb(40, 40, 50)), x - 22, y - 20, 44, 35);
            
            // Windshield
            using (var brush = new LinearGradientBrush(
                new Rectangle((int)(x - 20), (int)(y - 35), 40, 20),
                Color.FromArgb(150, 200, 220, 255), Color.FromArgb(100, 150, 180, 220), 90))
            {
                g.FillRectangle(brush, x - 20, y - 35, 40, 18);
            }
            
            // Rear windshield
            using (var brush = new LinearGradientBrush(
                new Rectangle((int)(x - 18), (int)(y + 10), 36, 15),
                Color.FromArgb(100, 150, 180, 220), Color.FromArgb(150, 200, 220, 255), 90))
            {
                g.FillRectangle(brush, x - 18, y + 10, 36, 12);
            }
            
            // Headlights
            g.FillEllipse(Brushes.White, x - 20, y - 50, 12, 8);
            g.FillEllipse(Brushes.White, x + 8, y - 50, 12, 8);
            
            // Headlight glow
            using (var brush = new SolidBrush(Color.FromArgb(30, 255, 255, 200)))
            {
                g.FillEllipse(brush, x - 30, y - 100, 20, 60);
                g.FillEllipse(brush, x + 10, y - 100, 20, 60);
            }
            
            // Tail lights
            g.FillRectangle(Brushes.Red, x - 24, y + 38, 10, 6);
            g.FillRectangle(Brushes.Red, x + 14, y + 38, 10, 6);
            
            // Tail light glow
            using (var brush = new SolidBrush(Color.FromArgb(50, 255, 0, 0)))
            {
                g.FillEllipse(brush, x - 30, y + 35, 25, 30);
                g.FillEllipse(brush, x + 5, y + 35, 25, 30);
            }
            
            // Wheels
            g.FillEllipse(Brushes.Black, x - 32, y - 30, 14, 20);
            g.FillEllipse(Brushes.Black, x + 18, y - 30, 14, 20);
            g.FillEllipse(Brushes.Black, x - 32, y + 15, 14, 20);
            g.FillEllipse(Brushes.Black, x + 18, y + 15, 14, 20);
            
            // Wheel rims
            g.FillEllipse(Brushes.Silver, x - 29, y - 25, 8, 10);
            g.FillEllipse(Brushes.Silver, x + 21, y - 25, 8, 10);
            g.FillEllipse(Brushes.Silver, x - 29, y + 20, 8, 10);
            g.FillEllipse(Brushes.Silver, x + 21, y + 20, 8, 10);
        }

        void DrawEnemyCars(Graphics g)
        {
            foreach (var car in enemyCars)
            {
                float x = car.X;
                float y = car.Y;
                
                Color bodyColor;
                switch (car.Type)
                {
                    case 1: bodyColor = Color.DarkBlue; break; // Truck
                    case 2: bodyColor = Color.Yellow; break; // Taxi
                    case 3: bodyColor = Color.White; break; // Police
                    default: bodyColor = Color.FromArgb(60, 60, 70); break; // Regular car
                }
                
                // Shadow
                g.FillEllipse(new SolidBrush(Color.FromArgb(40, 0, 0, 0)), x - 30, y + 35, 60, 15);
                
                if (car.Type == 1) // Truck
                {
                    // Truck body
                    g.FillRectangle(new SolidBrush(bodyColor), x - 25, y - 60, 50, 110);
                    g.FillRectangle(new SolidBrush(Color.Gray), x - 22, y - 40, 44, 30);
                    // Wheels
                    g.FillEllipse(Brushes.Black, x - 28, y - 50, 12, 18);
                    g.FillEllipse(Brushes.Black, x + 16, y - 50, 12, 18);
                    g.FillEllipse(Brushes.Black, x - 28, y + 25, 12, 18);
                    g.FillEllipse(Brushes.Black, x + 16, y + 25, 12, 18);
                }
                else
                {
                    // Regular car body
                    g.FillRectangle(new SolidBrush(bodyColor), x - 25, y - 35, 50, 75);
                    g.FillEllipse(new SolidBrush(bodyColor), x - 22, y + 25, 44, 25);
                    
                    // Windows
                    g.FillRectangle(new SolidBrush(Color.FromArgb(100, 150, 200, 255)), x - 18, y - 15, 36, 25);
                    
                    // Tail lights (facing player)
                    g.FillRectangle(Brushes.Red, x - 22, y - 38, 8, 5);
                    g.FillRectangle(Brushes.Red, x + 14, y - 38, 8, 5);
                    
                    // Wheels
                    g.FillEllipse(Brushes.Black, x - 28, y - 25, 12, 16);
                    g.FillEllipse(Brushes.Black, x + 16, y - 25, 12, 16);
                    g.FillEllipse(Brushes.Black, x - 28, y + 15, 12, 16);
                    g.FillEllipse(Brushes.Black, x + 16, y + 15, 12, 16);
                    
                    if (car.Type == 3) // Police car extras
                    {
                        // Police lights
                        Color lightColor = frameCount % 10 < 5 ? Color.Red : Color.Blue;
                        g.FillRectangle(new SolidBrush(lightColor), x - 15, y - 20, 10, 6);
                        g.FillRectangle(new SolidBrush(lightColor == Color.Red ? Color.Blue : Color.Red), x + 5, y - 20, 10, 6);
                    }
                    
                    if (car.Type == 2) // Taxi sign
                    {
                        g.FillRectangle(Brushes.Black, x - 10, y - 18, 20, 8);
                        g.DrawString("TAXI", new Font("Arial", 5), Brushes.Yellow, x - 8, y - 17);
                    }
                }
            }
        }

        void DrawCoins(Graphics g)
        {
            foreach (var coin in coins)
            {
                float x = coin.X;
                float y = coin.Y;
                
                // Coin glow
                using (var brush = new SolidBrush(Color.FromArgb(50, 255, 215, 0)))
                {
                    g.FillEllipse(brush, x - 25, y - 25, 50, 50);
                }
                
                // Coin body (animated rotation effect)
                float width = (float)(20 * Math.Abs(Math.Cos(coin.Rotation * Math.PI / 180)));
                width = Math.Max(width, 4);
                
                using (var brush = new LinearGradientBrush(
                    new Rectangle((int)(x - width), (int)(y - 15), (int)(width * 2), 30),
                    Color.Gold, Color.DarkGoldenrod, LinearGradientMode.Horizontal))
                {
                    g.FillEllipse(brush, x - width, y - 15, width * 2, 30);
                }
                
                // Dollar sign
                if (width > 10)
                {
                    g.DrawString("$", new Font("Arial", 12, FontStyle.Bold), Brushes.DarkGoldenrod, x - 7, y - 10);
                }
            }
        }

        void DrawParticles(Graphics g)
        {
            foreach (var p in particles)
            {
                int alpha = (int)(255 * ((float)p.Life / 30));
                using (var brush = new SolidBrush(Color.FromArgb(alpha, p.Color)))
                {
                    g.FillEllipse(brush, p.X - 4, p.Y - 4, 8, 8);
                }
            }
        }

        void DrawSpeedLines(Graphics g)
        {
            using (var pen = new Pen(Color.FromArgb(100, 255, 255, 255), 2))
            {
                for (int i = 0; i < 10; i++)
                {
                    float x = ROAD_LEFT + rand.Next(ROAD_WIDTH);
                    float y = rand.Next(900);
                    float len = 20 + speed * 2;
                    g.DrawLine(pen, x, y, x, y + len);
                }
            }
        }

        void DrawHUD(Graphics g)
        {
            // HUD background
            g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 0, 0, 850, 60);
            
            var font = new Font("Arial", 16, FontStyle.Bold);
            var smallFont = new Font("Arial", 12);
            
            // Score
            g.DrawImage(DrawCoinIcon(), 20, 15, 30, 30);
            g.DrawString($"{score}", font, Brushes.Gold, 55, 18);
            
            // Distance
            g.DrawString($"📏 {(int)distance}m", font, Brushes.White, 180, 18);
            
            // Speed
            float speedPercent = Math.Max(0, Math.Min((speed - baseSpeed) / 12, 1));
            g.FillRectangle(Brushes.DarkGray, 350, 20, 100, 20);
            int redVal = Math.Max(0, Math.Min(255, (int)(255 * speedPercent)));
            int greenVal = Math.Max(0, Math.Min(255, (int)(255 * (1 - speedPercent))));
            g.FillRectangle(new SolidBrush(Color.FromArgb(redVal, greenVal, 0)), 
                350, 20, Math.Max(1, speedPercent * 100), 20);
            g.DrawString($"⚡ {(int)(speed * 10)} km/h", smallFont, Brushes.White, 460, 20);
            
            // Lives
            for (int i = 0; i < lives; i++)
            {
                DrawMiniCar(g, 700 + i * 40, 25);
            }
        }

        Bitmap DrawCoinIcon()
        {
            var bmp = new Bitmap(30, 30);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(Brushes.Gold, 2, 2, 26, 26);
                g.DrawString("$", new Font("Arial", 14, FontStyle.Bold), Brushes.DarkGoldenrod, 6, 3);
            }
            return bmp;
        }

        void DrawMiniCar(Graphics g, float x, float y)
        {
            g.FillRectangle(Brushes.Red, x - 10, y - 15, 20, 30);
            g.FillRectangle(Brushes.DarkRed, x - 8, y - 5, 16, 10);
        }

        void DrawMenu(Graphics g)
        {
            // Overlay
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), 0, 0, 850, 900);
            
            // Difficulty badge color
            Color diffColor = difficultyLevel == 1 ? Color.LimeGreen : 
                              difficultyLevel == 2 ? Color.Orange : Color.Red;
            
            if (gameover)
            {
                DrawCenteredText(g, "💥 GAME OVER 💥", 280, Color.Red, 42);
                DrawCenteredText(g, $"Mode: {difficultyName}", 340, diffColor, 20);
                DrawCenteredText(g, $"Score: {score}", 380, Color.Gold, 28);
                DrawCenteredText(g, $"Distance: {(int)distance}m", 430, Color.White, 22);
            }
            else
            {
                DrawCenteredText(g, "🚗 CAR RUNNER 🚗", 250, Color.Gold, 48);
                DrawCenteredText(g, $"Mode: {difficultyName}", 320, diffColor, 22);
                DrawCenteredText(g, "Dodge traffic and collect coins!", 370, Color.LightGray, 18);
                DrawCenteredText(g, "← → or A/D to switch lanes", 410, Color.LightGray, 16);
            }
            
            // Animated press enter text
            int alpha = (int)(Math.Abs(Math.Sin(frameCount * 0.05)) * 255);
            DrawCenteredText(g, "Press ENTER to Start", 520, Color.FromArgb(alpha, Color.LimeGreen), 24);
        }

        void DrawCenteredText(Graphics g, string text, int y, Color color, int size)
        {
            var font = new Font("Arial", size, FontStyle.Bold);
            var sz = g.MeasureString(text, font);
            g.DrawString(text, font, new SolidBrush(color), (850 - sz.Width) / 2, y);
        }

        // Helper classes
        class EnemyCar
        {
            public float X, Y;
            public int Lane, Type, Speed;
        }

        class Coin
        {
            public float X, Y, Rotation;
        }



        class Particle
        {
            public float X, Y, VX, VY;
            public int Life;
            public Color Color;
        }
    }
}
