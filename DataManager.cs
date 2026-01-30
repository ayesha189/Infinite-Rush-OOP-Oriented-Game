using System;
using System.IO;
using System.Text.Json;

namespace GameFrameWork
{
    /// <summary>
    /// Game save data structure.
    /// </summary>
    public class GameSaveData
    {
        public int HighScore { get; set; } = 0;
        public int CurrentLevel { get; set; } = 1;
        public int MaxLevelUnlocked { get; set; } = 1;
        public float MusicVolume { get; set; } = 1.0f;
        public float SoundVolume { get; set; } = 1.0f;
        public bool IsMuted { get; set; } = false;
        public string PlayerName { get; set; } = "Player";
        public DateTime LastPlayed { get; set; } = DateTime.Now;
        public int TotalPlayTimeSeconds { get; set; } = 0;
        public int TotalEnemiesDefeated { get; set; } = 0;
        public int TotalCoinsCollected { get; set; } = 0;
    }

    /// <summary>
    /// Manages persistent game data using file storage.
    /// </summary>
    public class DataManager
    {
        private static DataManager instance;
        private string saveFilePath;
        private GameSaveData currentData;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the current save data.
        /// </summary>
        public GameSaveData Data => currentData;

        private DataManager()
        {
            // Save to AppData folder
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gameFolderPath = Path.Combine(appDataPath, "ShadowKnightChronicles");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(gameFolderPath))
            {
                Directory.CreateDirectory(gameFolderPath);
            }

            saveFilePath = Path.Combine(gameFolderPath, "savegame.json");
            currentData = new GameSaveData();
            Load();
        }

        /// <summary>
        /// Saves the current game data to file.
        /// </summary>
        public void Save()
        {
            try
            {
                currentData.LastPlayed = DateTime.Now;
                string json = JsonSerializer.Serialize(currentData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(saveFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving game: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads game data from file.
        /// </summary>
        public void Load()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    currentData = JsonSerializer.Deserialize<GameSaveData>(json) ?? new GameSaveData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading game: {ex.Message}");
                currentData = new GameSaveData();
            }
        }

        /// <summary>
        /// Updates high score if current is higher.
        /// </summary>
        public void UpdateHighScore(int score)
        {
            if (score > currentData.HighScore)
            {
                currentData.HighScore = score;
                Save();
            }
        }

        /// <summary>
        /// Unlocks a level.
        /// </summary>
        public void UnlockLevel(int level)
        {
            if (level > currentData.MaxLevelUnlocked)
            {
                currentData.MaxLevelUnlocked = level;
                Save();
            }
        }

        /// <summary>
        /// Sets the current level.
        /// </summary>
        public void SetCurrentLevel(int level)
        {
            currentData.CurrentLevel = level;
            Save();
        }

        /// <summary>
        /// Updates audio settings.
        /// </summary>
        public void UpdateAudioSettings(float musicVolume, float soundVolume, bool isMuted)
        {
            currentData.MusicVolume = musicVolume;
            currentData.SoundVolume = soundVolume;
            currentData.IsMuted = isMuted;
            Save();
        }

        /// <summary>
        /// Updates player stats.
        /// </summary>
        public void UpdateStats(int enemiesDefeated, int coinsCollected)
        {
            currentData.TotalEnemiesDefeated += enemiesDefeated;
            currentData.TotalCoinsCollected += coinsCollected;
            Save();
        }

        /// <summary>
        /// Resets all save data.
        /// </summary>
        public void ResetData()
        {
            currentData = new GameSaveData();
            Save();
        }

        /// <summary>
        /// Checks if a level is unlocked.
        /// </summary>
        public bool IsLevelUnlocked(int level)
        {
            return level <= currentData.MaxLevelUnlocked;
        }
    }
}
