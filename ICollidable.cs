using System.Drawing;

namespace GameFrameWork
{
    // Interface for collidable objects
    public interface ICollidable
    {
        RectangleF Bounds { get; }
        bool IsActive { get; }
        void OnCollision(GameObject other);
    }
}
