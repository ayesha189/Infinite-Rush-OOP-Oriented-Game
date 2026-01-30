namespace GameFrameWork
{
    // Game time tracking
    public class GameTime
    {
        public float DeltaTime { get; set; } = 0.016f; // ~60 FPS
        public float TotalTime { get; set; } = 0;
        
        public void Update(float delta)
        {
            DeltaTime = delta;
            TotalTime += delta;
        }
    }
}
