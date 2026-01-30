using System;

namespace GameFrameWork
{
    // Sound event types
    public enum GameSoundEvent { Collect, PlayerHurt, EnemyHit, EnemyDeath, PlayerDeath, Jump, ButtonClick }
    
    // Sound manager (singleton)
    public class SoundManager
    {
        private static SoundManager instance;
        public static SoundManager Instance => instance ?? (instance = new SoundManager());
        
        public bool SoundEnabled { get; set; } = true;
        public float Volume { get; set; } = 1.0f;
        
        private SoundManager() { }
        
        public void PlayEventSound(GameSoundEvent evt)
        {
            if (!SoundEnabled) return;
            switch (evt) {
                case GameSoundEvent.Collect: AudioManager.PlayCoinSound(); break;
                case GameSoundEvent.PlayerHurt: AudioManager.PlayHurtSound(); break;
                case GameSoundEvent.EnemyDeath: AudioManager.PlayBeep(300, 150); break;
                case GameSoundEvent.ButtonClick: AudioManager.PlayClickSound(); break;
                default: AudioManager.PlayBeep(500, 50); break;
            }
        }
    }
}
