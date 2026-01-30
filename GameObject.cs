using System.Drawing;

namespace GameFrameWork
{
    // Base class for all game objects
    public class GameObject : IDrawable, IUpdatable, ICollidable
    {
        public PointF Position { get; set; }
        public SizeF Size { get; set; }
        public PointF Velocity { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsRigidBody { get; set; } = true;
        public Image Sprite { get; set; }
        public IMovement Movement { get; set; }
        
        public RectangleF Bounds => new RectangleF(Position, Size);
        
        public virtual void Update(GameTime gameTime)
        {
            Movement?.Move(this, gameTime);
            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);
        }
        
        public virtual void Draw(Graphics g)
        {
            if (Sprite != null)
                g.DrawImage(Sprite, Bounds);
            else
                g.FillRectangle(Brushes.Gray, Bounds);
        }
        
        public virtual void OnCollision(GameObject other) { }
    }
}
