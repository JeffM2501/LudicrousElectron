using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Types.Volumes;
using OpenTK;

namespace LudicrousElectron.Entities.Collisions.Volumes
{
    public class CollisionSphere : Sphere
    {
        public delegate Vector3d GetCenterCB();

        public GetCenterCB GetCenter = null;

        public override Vector3d CenterPoint { get { if (GetCenter != null) return GetCenter(); return Vector3d.Zero; } }

		public CollisionSphere()
		{
			Radius = 100;
		}
	}
}
