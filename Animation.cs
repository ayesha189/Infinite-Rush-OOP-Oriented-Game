using System.Drawing;
using System.Collections.Generic;

namespace GameFrameWork
{
    // Animation frame sequence
    public class Animation
    {
        public List<Image> Frames { get; } = new List<Image>();
        public float FrameTime { get; set; } = 0.1f;
        public bool Loop { get; set; } = true;
        
        private int currentFrame = 0;
        private float elapsed = 0;
        
        public Image CurrentFrame => Frames.Count > 0 ? Frames[currentFrame] : null;
        
        public void Update(float delta)
        {
            if (Frames.Count == 0) return;
            elapsed += delta;
            if (elapsed >= FrameTime) {
                elapsed = 0;
                currentFrame++;
                if (currentFrame >= Frames.Count)
                    currentFrame = Loop ? 0 : Frames.Count - 1;
            }
        }
        
        public void Reset() { currentFrame = 0; elapsed = 0; }
    }
}
