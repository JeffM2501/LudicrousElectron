using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace LudicrousElectron.Types.Volumes
{
    public class Box : Volume
    {
        public virtual Vector3d Center { get; } = Vector3d.Zero;
        public Vector3 Size = Vector3.Zero;
        public virtual Quaterniond Orientation { get; } = Quaterniond.Identity;

        public override ColissionResults CheckCollision(Volume other)
        {
            if (other.GetType() == typeof(Sphere) || other.GetType().IsSubclassOf(typeof(Sphere)))
                return CheckCollision(other as Sphere);
            else if (other.GetType() == typeof(Box) || other.GetType().IsSubclassOf(typeof(Box)))
                return CheckCollision(other as Box);
            return ColissionResults.Empty;
        }

        public ColissionResults CheckCollision(Sphere other)
        {
            if (other == null)
                return ColissionResults.Empty;

            if (other.PointIn(Center) || SphereBoxCollision(other,this))
                return new ColissionResults(IntersectionTypes.Intersecting, 0);

            return ColissionResults.None;
        }

        public override bool PointIn(Vector3d point)
        {
            Vector3d relPoint = Vector3d.Transform(point, Orientation.Inverted()) - Center;

            return Math.Abs(relPoint.X) <= Size.X && Math.Abs(relPoint.Y) <= Size.Y && Math.Abs(relPoint.Z) <= Size.Z;
        }

        public ColissionResults CheckCollision(Box other)
        {
            if (other == null)
                return ColissionResults.Empty;

            // check all this boxes points against me to see if any part of it is inside of me.

            foreach(var pt in other.GetWorldSpacePoints())
            {
                if (PointIn(pt))
                    return new ColissionResults(IntersectionTypes.Intersecting, 0);
            }

            // now check all of MY points inside the other box in case I am entirely inside it
            foreach (var pt in GetWorldSpacePoints())
            {
                if (other.PointIn(pt))
                    return new ColissionResults(IntersectionTypes.Intersecting, 0);
            }

            // TODO, check to see if the box crosses more than one plane?

            return ColissionResults.None;
        }

        public List<Vector3d> GetWorldSpacePoints()
        {
            List<Vector3d> points = new List<Vector3d>();

            points.Add(Vector3d.Transform(new Vector3d(Size.X, Size.Y, Size.Z), Orientation) + Center);
            points.Add(Vector3d.Transform(new Vector3d(Size.X, -Size.Y, Size.Z), Orientation) + Center);
            points.Add(Vector3d.Transform(new Vector3d(Size.X, -Size.Y, -Size.Z), Orientation) + Center);
            points.Add(Vector3d.Transform(new Vector3d(Size.X, Size.Y, -Size.Z), Orientation) + Center);

            points.Add(Vector3d.Transform(new Vector3d(-Size.X, Size.Y, Size.Z), Orientation) + Center);
            points.Add(Vector3d.Transform(new Vector3d(-Size.X, -Size.Y, Size.Z), Orientation) + Center);
            points.Add(Vector3d.Transform(new Vector3d(-Size.X, -Size.Y, -Size.Z), Orientation) + Center);
            points.Add(Vector3d.Transform(new Vector3d(-Size.X, Size.Y, -Size.Z), Orientation) + Center);

            return points;
        }

        public static bool SphereBoxCollision(Sphere sphere, Box rect)
        {
            Vector3d relPoint = Vector3d.Transform(sphere.CenterPoint, rect.Orientation.Inverted());

            double sphereXDistance = Math.Abs(relPoint.X - rect.Center.X);
            double sphereYDistance = Math.Abs(relPoint.Y - rect.Center.Y);
            double sphereZDistance = Math.Abs(relPoint.Z - rect.Center.Z);

            if (sphereXDistance >= (rect.Size.X + sphere.Radius)) { return false; }
            if (sphereYDistance >= (rect.Size.Y + sphere.Radius)) { return false; }
            if (sphereZDistance >= (rect.Size.Z + sphere.Radius)) { return false; }

            if (sphereXDistance < (rect.Size.X)) { return true; }
            if (sphereYDistance < (rect.Size.Y)) { return true; }
            if (sphereZDistance < (rect.Size.Z)) { return true; }

            double cornerDistance_sq = ((sphereXDistance - rect.Size.X) * (sphereXDistance - rect.Size.X)) +
                                  ((sphereYDistance - rect.Size.Y) * (sphereYDistance - rect.Size.Y) +
                                  ((sphereZDistance - rect.Size.Z) * (sphereZDistance - rect.Size.Z)));

            return (cornerDistance_sq < (sphere.Radius * sphere.Radius));
        }
    }
}
