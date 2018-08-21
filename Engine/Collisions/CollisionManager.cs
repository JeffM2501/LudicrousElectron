using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Box2DNet.Dynamics;

using LudicrousElectron.Types;

namespace LudicrousElectron.Engine.Collisions
{
	public static class CollisionManager
	{
		public static void Initalize()
		{
			CollisonWorld = new World(new Box2DNet.Collision.AABB(), new Box2DNet.Common.Vec2(), true);
		}

		public static void ProcessCollisions(float timeDelta)
		{
            if (CollisonWorld != null)
            {
                CollisonWorld.Step(timeDelta, 1, 1);
            }
		}

		public static List<ICollisionable> QueryArea(Rect2Df area)
		{
			List<ICollisionable> results = new List<ICollisionable>();
			return results;
		}

		private static World CollisonWorld = null;
	}
}
