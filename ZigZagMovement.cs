using System.Drawing;

namespace GameFrameWork
{
    // ZigZag movement behavior
    public class ZigZagMovement : IMovement
    {
        public float Speed { get; set; }
        public float LeftBound { get; set; }
        public float RightBound { get; set; }
        private bool movingRight = true;
        
        public ZigZagMovement(float left, float right, float speed = 3f)
        {
            LeftBound = left; RightBound = right; Speed = speed;
        }
        
        public void Move(GameObject obj, GameTime gameTime)
        {
            // Move down
            obj.Position = new PointF(obj.Position.X, obj.Position.Y + Speed);
            
            // Move horizontally
            if (movingRight) {
                obj.Position = new PointF(obj.Position.X + Speed * 0.5f, obj.Position.Y);
                if (obj.Position.X >= RightBound) movingRight = false;
            } else {
                obj.Position = new PointF(obj.Position.X - Speed * 0.5f, obj.Position.Y);
                if (obj.Position.X <= LeftBound) movingRight = true;
            }
        }
    }
}
