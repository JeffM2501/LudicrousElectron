using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace LudicrousElectron.Types.Volumes
{
    public abstract class Volume
    {
        public class ColissionResults
        {
            public IntersectionTypes Intersection = IntersectionTypes.Unknown;
            public double Overlap = double.MinValue;

            public ColissionResults() { /* default constructor */ }

            public ColissionResults(IntersectionTypes i, double o)
            {
                Intersection = i;
                Overlap = o;
            }

            public static readonly ColissionResults Empty = new ColissionResults();
            public static readonly ColissionResults None = new ColissionResults(IntersectionTypes.Disjoint, double.MaxValue);
        }

        public abstract ColissionResults CheckCollision(Volume other);
        public abstract bool PointIn(Vector3d point);
    }
}
