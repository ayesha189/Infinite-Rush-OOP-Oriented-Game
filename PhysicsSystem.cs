using System.Collections.Generic;
using System.Drawing;

namespace GameFrameWork
{
    // Physics system for gravity and movement
    public class PhysicsSystem
    {
        public float Gravity { get; set; } = 0.5f;
        public float GroundLevel { get; set; } = 500;
        
        public void ApplyPhysics(List<GameObject> objects)
        {
            foreach (var obj in objects) {
                if (!obj.IsActive || !obj.IsRigidBody) continue;
                
                // Apply gravity
                obj.Velocity = new PointF(obj.Velocity.X, obj.Velocity.Y + Gravity);
                
                // Ground collision
                if (obj.Position.Y + obj.Size.Height >= GroundLevel) {
                    obj.Position = new PointF(obj.Position.X, GroundLevel - obj.Size.Height);
                    obj.Velocity = new PointF(obj.Velocity.X, 0);
                }
            }
        }
    }
}
