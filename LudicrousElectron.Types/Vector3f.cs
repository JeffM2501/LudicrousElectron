using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Types
{
    public enum Axes
    {
        X,
        Y,
        Z,
    }

    public class Vector3f
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;

        public Vector3f() { }
        public Vector3f(float _x, float _y, float _z) { x = _x; y = _y; z = _z; }

        public Vector3f Clone() { return new Vector3f(x, y, z); }

        public Vector3f(Axes axis, float value) { Set(axis, value); }

        public void Set(Axes axis, float value)
        {
            switch (axis)
            {
                case Axes.X:
                    x = value;
                    break;
                case Axes.Y:
                    y = value;
                    break;
                case Axes.Z:
                    z = value;
                    break;
            }
        }

        public static readonly Vector3f Zero = new Vector3f();

        public static bool TryParse(string text, out Vector3f val)
        {
            val = new Vector3f();
            var nums = text.Split(",".ToCharArray(), 3);
            if (nums.Length > 0)
            {
                if (!float.TryParse(nums[0], out val.x))
                    return false;
            }

            if (nums.Length > 1)
            {
                if (!float.TryParse(nums[1], out val.y))
                    return false;
            }

            if (nums.Length > 2)
            {
                if (!float.TryParse(nums[2], out val.z))
                    return false;
            }

            return true;
        }


        public double Magnitude()
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }
    }
}
