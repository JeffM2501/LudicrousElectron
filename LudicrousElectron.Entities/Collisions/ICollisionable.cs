using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Entities.Collisions.Volumes;
using LudicrousElectron.Types.Volumes;

namespace LudicrousElectron.Entities.Collisions
{
	public interface ICollisionable
	{
        CollisionSphere GetBoundingSphere();

        List<Volume> GetCollisionVolumes();

        void OnCollide(ICollisionable other);
	}
}
