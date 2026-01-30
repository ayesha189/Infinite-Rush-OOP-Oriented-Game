using System.Drawing;

namespace GameFrameWork
{
    // Environment object types
    public enum EnvironmentType { Platform, Hazard, Collectible, Decoration }
    
    // Base class for environment objects
    public class EnvironmentObject : GameObject
    {
        public EnvironmentType EnvironmentType { get; set; } = EnvironmentType.Platform;
        public bool IsSolid { get; set; } = true;
        public Color FillColor { get; set; } = Color.Brown;
        public int PointValue { get; set; } = 0;
        public int DamageOnContact { get; set; } = 0;
        
        public override void Draw(Graphics g)
        {
            if (Sprite != null)
                g.DrawImage(Sprite, Bounds);
            else
                g.FillRectangle(new SolidBrush(FillColor), Bounds);
        }
        
        // Factory methods
        public static EnvironmentObject CreatePlatform(PointF pos, SizeF size)
        {
            return new EnvironmentObject { Position = pos, Size = size, IsSolid = true, FillColor = Color.Brown };
        }
        
        public static EnvironmentObject CreateCollectible(PointF pos, int points = 100)
        {
            return new EnvironmentObject { Position = pos, Size = new SizeF(24, 24), 
                EnvironmentType = EnvironmentType.Collectible, PointValue = points, FillColor = Color.Gold, IsSolid = false };
        }
    }
}
