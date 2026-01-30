using System.Drawing;

namespace GameFrameWork
{
    // Collectible item
    public class Collectible : EnvironmentObject
    {
        public Collectible()
        {
            EnvironmentType = EnvironmentType.Collectible;
            IsSolid = false;
            FillColor = Color.Gold;
            PointValue = 100;
            Size = new SizeF(24, 24);
        }
        
        public override void OnCollision(GameObject other)
        {
            if (other is Player player) {
                player.Score += PointValue;
                ScoreManager.Instance.AddScore(PointValue);
                AudioManager.PlayCoinSound();
                IsActive = false;
            }
        }
        
        public override void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(FillColor), Bounds);
        }
    }
}
