using System;
using System.IO;

namespace GameFrameWork
{
    // Audio Manager with Music and SFX volume controls
    public static class AudioManager
    {
        private static bool soundEnabled = true;
        private static int musicVolume = 65;
        private static int sfxVolume = 100;

        public static bool SoundEnabled
        {
            get => soundEnabled;
            set { soundEnabled = value; SaveSettings(); }
        }

        public static int MusicVolume
        {
            get => musicVolume;
            set { musicVolume = Math.Max(0, Math.Min(100, value)); SaveSettings(); }
        }

        public static int SFXVolume
        {
            get => sfxVolume;
            set { sfxVolume = Math.Max(0, Math.Min(100, value)); SaveSettings(); }
        }

        // For backward compatibility
        public static int Volume
        {
            get => sfxVolume;
            set => SFXVolume = value;
        }

        static AudioManager()
        {
            LoadSettings();
        }

        // Play beep sound
        public static void PlayBeep(int freq, int duration)
        {
            if (!soundEnabled || sfxVolume == 0) return;
            try { Console.Beep(freq, duration); } catch { }
        }

        public static void PlayCoinSound() => PlayBeep(800, 50);
        public static void PlayHurtSound() => PlayBeep(200, 100);
        public static void PlayDeathSound() => PlayBeep(150, 300);
        public static void PlayClickSound() => PlayBeep(600, 50);

        static void SaveSettings()
        {
            try {
                string path = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner_settings.txt");
                File.WriteAllText(path, $"{soundEnabled},{musicVolume},{sfxVolume}");
            } catch { }
        }

        static void LoadSettings()
        {
            try {
                string path = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner_settings.txt");
                if (File.Exists(path)) {
                    var parts = File.ReadAllText(path).Split(',');
                    if (parts.Length >= 3) {
                        soundEnabled = parts[0] == "True";
                        int.TryParse(parts[1], out musicVolume);
                        int.TryParse(parts[2], out sfxVolume);
                    }
                }
            } catch { }
        }
    }
}
