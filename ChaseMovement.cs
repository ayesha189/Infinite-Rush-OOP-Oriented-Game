using System;
using System.Drawing;

namespace GameFrameWork
{
    // Chase movement - follows a target
    public class ChaseMovement : IMovement
    {
        public GameObject Target { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; }
        
        public ChaseMovement(GameObject target, float speed = 2f, float range = 200f)
        {
            Target = target; Speed = speed; Range = range;
        }
        
        public void Move(GameObject obj, GameTime gameTime)
        {
            if (Target == null || !Target.IsActive) return;
            
            float dx = Target.Position.X - obj.Position.X;
            float dy = Target.Position.Y - obj.Position.Y;
            float dist = (float)Math.Sqrt(dx*dx + dy*dy);
            
            if (dist < Range && dist > 5) {
                obj.Position = new PointF(
                    obj.Position.X + (dx/dist) * Speed,
                    obj.Position.Y + (dy/dist) * Speed);
            }
        }
    }
}
