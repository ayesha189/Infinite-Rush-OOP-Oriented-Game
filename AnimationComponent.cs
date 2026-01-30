using System.Drawing;
using System.Collections.Generic;

namespace GameFrameWork
{
    // Component for managing animations
    public class AnimationComponent
    {
        private Dictionary<AnimationState, Animation> animations = new Dictionary<AnimationState, Animation>();
        private AnimationState currentState = AnimationState.Idle;
        
        public bool FlipHorizontal { get; set; } = false;
        public Image CurrentFrame => animations.ContainsKey(currentState) ? animations[currentState].CurrentFrame : null;
        
        public void AddAnimation(AnimationState state, Animation animation)
        {
            animations[state] = animation;
        }
        
        public void SetState(AnimationState state)
        {
            if (currentState != state && animations.ContainsKey(state)) {
                currentState = state;
                animations[state].Reset();
            }
        }
        
        public void Update(float delta = 0.016f)
        {
            if (animations.ContainsKey(currentState))
                animations[currentState].Update(delta);
        }
        
        public void Draw(Graphics g, RectangleF bounds)
        {
            var frame = CurrentFrame;
            if (frame == null) return;
            
            if (FlipHorizontal) {
                var flipped = (Image)frame.Clone();
                ((Bitmap)flipped).RotateFlip(RotateFlipType.RotateNoneFlipX);
                g.DrawImage(flipped, bounds);
            } else {
                g.DrawImage(frame, bounds);
            }
        }
    }
}
