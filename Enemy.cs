using System.Drawing;

namespace GameFrameWork
{
    // Enemy base class
    public class Enemy : GameObject
    {
        public int Damage { get; set; } = 10;
        
        public Enemy()
        {
            Size = new SizeF(40, 40);
        }
        
        public override void Draw(Graphics g)
        {
            if (Sprite != null)
                g.DrawImage(Sprite, Bounds);
            else
                g.FillRectangle(Brushes.Red, Bounds);
        }
    }
}
