using System.Drawing;
using EZInput;

namespace GameFrameWork
{
    // Keyboard-based movement
    public class KeyboardMovement : IMovement
    {
        public float Speed { get; set; } = 5f;
        
        public void Move(GameObject obj, GameTime gameTime)
        {
            float dx = 0, dy = 0;
            if (Keyboard.IsKeyPressed(Key.A) || Keyboard.IsKeyPressed(Key.LeftArrow)) dx = -Speed;
            if (Keyboard.IsKeyPressed(Key.D) || Keyboard.IsKeyPressed(Key.RightArrow)) dx = Speed;
            if (Keyboard.IsKeyPressed(Key.W) || Keyboard.IsKeyPressed(Key.UpArrow)) dy = -Speed;
            if (Keyboard.IsKeyPressed(Key.S) || Keyboard.IsKeyPressed(Key.DownArrow)) dy = Speed;
            obj.Position = new PointF(obj.Position.X + dx, obj.Position.Y + dy);
        }
    }
}
