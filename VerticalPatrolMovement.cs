using System.Drawing;

namespace GameFrameWork
{
    // Vertical patrol movement behavior
    public class VerticalPatrolMovement : IMovement
    {
        public float TopBound { get; set; }
        public float BottomBound { get; set; }
        public float Speed { get; set; }
        private bool movingDown = true;
        
        public VerticalPatrolMovement(float top, float bottom, float speed = 2f)
        {
            TopBound = top; BottomBound = bottom; Speed = speed;
        }
        
        public void Move(GameObject obj, GameTime gameTime)
        {
            if (movingDown) {
                obj.Position = new PointF(obj.Position.X, obj.Position.Y + Speed);
                if (obj.Position.Y >= BottomBound) movingDown = false;
            } else {
                obj.Position = new PointF(obj.Position.X, obj.Position.Y - Speed);
                if (obj.Position.Y <= TopBound) movingDown = true;
            }
        }
    }
}
