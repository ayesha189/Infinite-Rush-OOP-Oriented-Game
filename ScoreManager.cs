using System;

namespace GameFrameWork
{
    // Score manager (singleton)
    public class ScoreManager
    {
        private static ScoreManager instance;
        public static ScoreManager Instance => instance ?? (instance = new ScoreManager());
        
        public int CurrentScore { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;
        
        public event Action<int> OnScoreChanged;
        
        private ScoreManager() { LoadHighScore(); }
        
        public void AddScore(int points)
        {
            CurrentScore += points;
            if (CurrentScore > HighScore) HighScore = CurrentScore;
            OnScoreChanged?.Invoke(CurrentScore);
        }
        
        public void Reset() { CurrentScore = 0; }
        
        private void LoadHighScore()
        {
            try {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner_highscore.txt");
                if (System.IO.File.Exists(path))
                    int.TryParse(System.IO.File.ReadAllText(path), out int hs);
            } catch { }
        }
        
        public void SaveHighScore()
        {
            try {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EndlessRunner_highscore.txt");
                System.IO.File.WriteAllText(path, HighScore.ToString());
            } catch { }
        }
    }
}
