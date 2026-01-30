using System.Drawing;

namespace GameFrameWork
{
    // Horizontal patrol movement behavior
    public class HorizontalPatrolMovement : IMovement
    {
        public float LeftBound { get; set; }
        public float RightBound { get; set; }
        public float Speed { get; set; }
        private bool movingRight = true;
        
        public HorizontalPatrolMovement(float left, float right, float speed = 2f)
        {
            LeftBound = left; RightBound = right; Speed = speed;
        }
        
        public void Move(GameObject obj, GameTime gameTime)
        {
            if (movingRight) {
                obj.Position = new PointF(obj.Position.X + Speed, obj.Position.Y);
                if (obj.Position.X >= RightBound) movingRight = false;
            } else {
                obj.Position = new PointF(obj.Position.X - Speed, obj.Position.Y);
                if (obj.Position.X <= LeftBound) movingRight = true;
            }
        }
    }
}
