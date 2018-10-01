using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace LudicrousElectron.Types.Volumes
{
    public class Sphere
    {
        public double Radius = 0;
        public Vector3 CenterPoint = Vector3.Zero;


        public class ColissionResults
        {
            public IntersectionTypes Intersection = IntersectionTypes.Disjoint;
            public double Overlap = double.MinValue;
        }
    }
}
