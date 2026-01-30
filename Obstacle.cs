using System.Drawing;

namespace GameFrameWork
{
    // Obstacle (solid block)
    public class Obstacle : EnvironmentObject
    {
        public Obstacle()
        {
            EnvironmentType = EnvironmentType.Platform;
            IsSolid = true;
            FillColor = Color.SaddleBrown;
        }
        
        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(FillColor), Bounds);
            g.DrawRectangle(Pens.Black, Position.X, Position.Y, Size.Width, Size.Height);
        }
    }
}
