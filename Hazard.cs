using System.Drawing;

namespace GameFrameWork
{
    // Hazard object that damages player
    public class Hazard : EnvironmentObject
    {
        public Hazard()
        {
            EnvironmentType = EnvironmentType.Hazard;
            IsSolid = false;
            FillColor = Color.DarkRed;
            DamageOnContact = 10;
        }
        
        public override void OnCollision(GameObject other)
        {
            if (other is Player player) {
                player.Health -= DamageOnContact;
                AudioManager.PlayHurtSound();
            }
        }
        
        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(FillColor), Bounds);
            // Draw spikes
            using (var pen = new Pen(Color.Gray, 2))
                g.DrawRectangle(pen, Position.X, Position.Y, Size.Width, Size.Height);
        }
    }
}
