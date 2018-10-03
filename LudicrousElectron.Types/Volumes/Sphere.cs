using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace LudicrousElectron.Types.Volumes
{
    public class Sphere : Volume
    {
        public double Radius = 0;
        public virtual Vector3d CenterPoint { get; } = Vector3d.Zero;

        public override ColissionResults CheckCollision(Volume other)
        {
            if (other.GetType() == typeof(Sphere) || other.GetType().IsSubclassOf(typeof(Sphere)))
                return CheckCollision(other as Sphere);
            else if (other.GetType() == typeof(Box) || other.GetType().IsSubclassOf(typeof(Box)))
                return (other as Box).CheckCollision(this); // box knows how to check against spheres so let it do it.

            return ColissionResults.Empty;
        }

        public ColissionResults CheckCollision(Sphere other)
        {
            if (other == null)
                return ColissionResults.Empty;

            double maxDist = Radius + other.Radius;
            double dist = Vector3d.Distance(CenterPoint, other.CenterPoint);
            if (dist > maxDist)
                return ColissionResults.None;

            return new ColissionResults(IntersectionTypes.Intersecting, maxDist - dist);
        }

        public override bool PointIn(Vector3d point)
        {
            double maxDist = Radius;
            double dist = Vector3d.Distance(CenterPoint, point);
            if (dist > maxDist)
                return true;

            return false;
        }
    }
}
