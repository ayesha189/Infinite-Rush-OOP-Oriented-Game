using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace GameFrameWork
{
    // Main Menu - Start, Settings, Exit
    public class MainMenuForm : Form
    {
        Button btnSettings, btnExit;

        public MainMenuForm()
        {
            Text = "Endless Runner";
            Size = new Size(800, 700);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(20, 20, 40);
            DoubleBuffered = true;
            Paint += OnPaint;

            // Title
            Controls.Add(new Label {
                Text = "ENDLESS RUNNER", Font = new Font("Arial", 42, FontStyle.Bold),
                ForeColor = Color.Gold, AutoSize = true, Location = new Point(180, 80),
                BackColor = Color.Transparent
            });

            // Instructions
            Controls.Add(new Label {
                Text = "Collect GOLD coins | Avoid BROWN obstacles\nUse WASD or Arrow Keys to move",
                Font = new Font("Arial", 12), ForeColor = Color.LightGray,
                AutoSize = true, Location = new Point(210, 180), BackColor = Color.Transparent
            });

            // High Score
            int highScore = LoadHighScore();
            Controls.Add(new Label {
                Text = $"High Score: {highScore}", Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Gold, AutoSize = true, Location = new Point(320, 240),
                BackColor = Color.Transparent
            });
            // Difficulty label
            Controls.Add(new Label {
                Text = "SELECT DIFFICULTY", Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White, AutoSize = true, Location = new Point(290, 280),
                BackColor = Color.Transparent
            });

            // Easy button (Green)
            var btnEasy = CreateLevelButton("🟢 EASY", 320, Color.FromArgb(40, 120, 40));
            btnEasy.Click += (s, e) => {
                AudioManager.PlayClickSound();
                Hide(); new AnimatedEndlessRunnerForm(1).ShowDialog(); Show();
            };
            Controls.Add(btnEasy);

            // Medium button (Orange)
            var btnMedium = CreateLevelButton("🟡 MEDIUM", 380, Color.FromArgb(180, 120, 20));
            btnMedium.Click += (s, e) => {
                AudioManager.PlayClickSound();
                Hide(); new AnimatedEndlessRunnerForm(2).ShowDialog(); Show();
            };
            Controls.Add(btnMedium);

            // Hard button (Red)
            var btnHard = CreateLevelButton("🔴 HARD", 440, Color.FromArgb(160, 40, 40));
            btnHard.Click += (s, e) => {
                AudioManager.PlayClickSound();
                Hide(); new AnimatedEndlessRunnerForm(3).ShowDialog(); Show();
            };
            Controls.Add(btnHard);

            // Settings button
            btnSettings = CreateButton("SETTINGS", 510);
            btnSettings.Click += (s, e) => {
                AudioManager.PlayClickSound();
                new SettingsForm().ShowDialog();
            };
            Controls.Add(btnSettings);

            // Exit button  
            btnExit = CreateButton("EXIT", 580);
            btnExit.Click += (s, e) => {
                AudioManager.PlayClickSound();
                if (MessageBox.Show("Exit?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Application.Exit();
            };
            Controls.Add(btnExit);
        }

        int LoadHighScore()
        {
            try {
                string path = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner.txt");
                if (File.Exists(path)) {
                    int max = 0;
                    foreach (var line in File.ReadAllLines(path)) {
                        var parts = line.Split(' ');
                        if (parts.Length > 0 && parts[0].StartsWith("Score:")) {
                            int.TryParse(parts[0].Replace("Score:", ""), out int s);
                            if (s > max) max = s;
                        }
                    }
                    return max;
                }
            } catch { }
            return 0;
        }

        Button CreateButton(string text, int y) => new Button {
            Text = text, Size = new Size(220, 50), Location = new Point(290, y),
            Font = new Font("Arial", 14, FontStyle.Bold),
            ForeColor = Color.White, BackColor = Color.FromArgb(60, 60, 100),
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            FlatAppearance = { BorderColor = Color.Gold, BorderSize = 2,
                MouseOverBackColor = Color.FromArgb(80, 80, 140) }
        };

        Button CreateLevelButton(string text, int y, Color bgColor) => new Button {
            Text = text, Size = new Size(220, 45), Location = new Point(290, y),
            Font = new Font("Arial", 13, FontStyle.Bold),
            ForeColor = Color.White, BackColor = bgColor,
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            FlatAppearance = { BorderColor = Color.White, BorderSize = 2,
                MouseOverBackColor = Color.FromArgb(
                    Math.Min(bgColor.R + 30, 255), 
                    Math.Min(bgColor.G + 30, 255), 
                    Math.Min(bgColor.B + 30, 255)) }
        };

        void OnPaint(object sender, PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(ClientRectangle,
                Color.FromArgb(20, 20, 60), Color.FromArgb(10, 10, 30),
                LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, ClientRectangle);
            using (var pen = new Pen(Color.FromArgb(100, 255, 215, 0), 2))
                e.Graphics.DrawRectangle(pen, 20, 20, 755, 635);
        }
    }

    // Settings Form with Audio Controls
    public class SettingsForm : Form
    {
        TrackBar trackMusic, trackSFX;
        Label lblMusic, lblSFX;
        CheckBox chkMute;

        public SettingsForm()
        {
            Text = "Settings";
            Size = new Size(420, 320);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            BackColor = Color.FromArgb(30, 30, 50);

            // Title
            Controls.Add(new Label {
                Text = "SETTINGS", Font = new Font("Arial", 20, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.White, Location = new Point(140, 15), AutoSize = true
            });

            // Music Volume label
            Controls.Add(new Label {
                Text = "Music Volume:", Font = new Font("Arial", 11),
                ForeColor = Color.White, Location = new Point(30, 60), AutoSize = true
            });

            // Music Volume slider
            trackMusic = new TrackBar {
                Location = new Point(30, 85), Size = new Size(280, 45),
                Minimum = 0, Maximum = 100, Value = AudioManager.MusicVolume,
                TickStyle = TickStyle.None, BackColor = Color.FromArgb(30, 30, 50)
            };
            trackMusic.ValueChanged += (s, e) => {
                AudioManager.MusicVolume = trackMusic.Value;
                lblMusic.Text = $"{trackMusic.Value}%";
            };
            Controls.Add(trackMusic);

            // Music percentage
            lblMusic = new Label {
                Text = $"{AudioManager.MusicVolume}%", Font = new Font("Arial", 11),
                ForeColor = Color.DeepSkyBlue, Location = new Point(320, 90), AutoSize = true
            };
            Controls.Add(lblMusic);

            // Sound Effects label
            Controls.Add(new Label {
                Text = "Sound Effects Volume:", Font = new Font("Arial", 11),
                ForeColor = Color.White, Location = new Point(30, 130), AutoSize = true
            });

            // SFX slider
            trackSFX = new TrackBar {
                Location = new Point(30, 155), Size = new Size(280, 45),
                Minimum = 0, Maximum = 100, Value = AudioManager.SFXVolume,
                TickStyle = TickStyle.None, BackColor = Color.FromArgb(30, 30, 50)
            };
            trackSFX.ValueChanged += (s, e) => {
                AudioManager.SFXVolume = trackSFX.Value;
                lblSFX.Text = $"{trackSFX.Value}%";
            };
            Controls.Add(trackSFX);

            // SFX percentage
            lblSFX = new Label {
                Text = $"{AudioManager.SFXVolume}%", Font = new Font("Arial", 11),
                ForeColor = Color.DeepSkyBlue, Location = new Point(320, 160), AutoSize = true
            };
            Controls.Add(lblSFX);

            // Mute checkbox
            chkMute = new CheckBox {
                Text = "Mute All Audio", Font = new Font("Arial", 11),
                ForeColor = Color.White, Location = new Point(30, 200),
                AutoSize = true, Checked = !AudioManager.SoundEnabled
            };
            chkMute.CheckedChanged += (s, e) => {
                AudioManager.SoundEnabled = !chkMute.Checked;
            };
            Controls.Add(chkMute);

            // SAVE button
            var btnSave = new Button {
                Text = "SAVE", Size = new Size(100, 35), Location = new Point(100, 240),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White, BackColor = Color.FromArgb(40, 120, 40),
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => { AudioManager.PlayClickSound(); Close(); };
            Controls.Add(btnSave);

            // CANCEL button
            var btnCancel = new Button {
                Text = "CANCEL", Size = new Size(100, 35), Location = new Point(210, 240),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White, BackColor = Color.FromArgb(120, 40, 40),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => Close();
            Controls.Add(btnCancel);
        }
    }
}
