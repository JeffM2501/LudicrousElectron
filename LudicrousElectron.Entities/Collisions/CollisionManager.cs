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
		public void Initalize()
		{
		
		}

		public void ProcessCollisions(float timeDelta)
		{
		}

		public List<ICollisionable> QueryArea(Rect2Df area)
		{
			List<ICollisionable> results = new List<ICollisionable>();
			return results;
		}
	}
}
