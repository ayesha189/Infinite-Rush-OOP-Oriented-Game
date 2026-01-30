using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameFrameWork
{
    // Collision detection system
    public class CollisionSystem
    {
        public void CheckCollisions(List<GameObject> objects)
        {
            for (int i = 0; i < objects.Count; i++) {
                if (!objects[i].IsActive) continue;
                for (int j = i + 1; j < objects.Count; j++) {
                    if (!objects[j].IsActive) continue;
                    if (objects[i].Bounds.IntersectsWith(objects[j].Bounds)) {
                        objects[i].OnCollision(objects[j]);
                        objects[j].OnCollision(objects[i]);
                    }
                }
            }
        }
        
        public bool CheckCollision(GameObject a, GameObject b)
        {
            return a.IsActive && b.IsActive && a.Bounds.IntersectsWith(b.Bounds);
        }
    }
}
