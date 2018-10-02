using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Types;

namespace LudicrousElectron.Entities.Collisions
{
	public class CollisionManager
	{
        public List<ICollisionable> CollideableElements = new List<ICollisionable>();

        public void CheckCollisions()
        {
            // this is very brute force
            // the right way involves an octree to get what is near what first

            List<Tuple<ICollisionable, ICollisionable>> collidingPairs = new List<Tuple<ICollisionable, ICollisionable>>();

            foreach(ICollisionable obj in CollideableElements)
            {
                foreach (ICollisionable other in CollideableElements)
                {
                    if (obj == other)
                        continue;

                    if (collidingPairs.Find((x) => (x.Item1 == obj && x.Item1 == other) || (x.Item1 == other && x.Item2 == obj)) != null)
                        continue;

                    if (obj.GetBoundingSphere().CheckCollision(other.GetBoundingSphere()).Intersection == Types.Volumes.IntersectionTypes.Intersecting) // fast and cheap (ish)
                    {
                        bool hit = false;
                        foreach (var objVolume in obj.GetCollisionVolumes())
                        {
                            if (hit)
                                break;

                            foreach (var otherVolume in other.GetCollisionVolumes())
                            {
                                if (otherVolume.CheckCollision(objVolume).Intersection == Types.Volumes.IntersectionTypes.Intersecting)
                                {
                                    hit = true;
                                    break;
                                }
                            }

                            if (hit)
                                collidingPairs.Add(new Tuple<ICollisionable, ICollisionable>(obj, other));
                        }
                    }
                }
            }

            foreach (var pair in collidingPairs)
            {
                pair.Item1.OnCollide(pair.Item2);
                pair.Item2.OnCollide(pair.Item1);
            }
        }
	}
}
