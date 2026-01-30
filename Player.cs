using System.Drawing;

namespace GameFrameWork
{
    // Player base class
    public class Player : GameObject
    {
        public int Health { get; set; } = 100;
        public int Score { get; set; } = 0;
        
        public Player()
        {
            Size = new SizeF(40, 60);
        }
        
        public override void Draw(Graphics g)
        {
            if (Sprite != null)
                g.DrawImage(Sprite, Bounds);
            else
                g.FillRectangle(Brushes.Blue, Bounds);
        }
    }
}
