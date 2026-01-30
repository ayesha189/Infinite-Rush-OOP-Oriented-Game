using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameFrameWork
{
    // Main game container
    public class Game
    {
        public List<GameObject> Objects { get; } = new List<GameObject>();
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public Color BackgroundColor { get; set; } = Color.CornflowerBlue;
        
        public void AddObject(GameObject obj) => Objects.Add(obj);
        public void RemoveObject(GameObject obj) => Objects.Remove(obj);
        public void Clear() => Objects.Clear();
        
        public void Update(GameTime gameTime)
        {
            foreach (var obj in Objects)
                if (obj.IsActive) obj.Update(gameTime);
        }
        
        public void Draw(Graphics g)
        {
            foreach (var obj in Objects)
                if (obj.IsActive) obj.Draw(g);
        }
    }
}
