using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Types
{
    public class Vector2i
    {
		public int x = 0;
		public int y = 0;


		public Vector2i() { }
		public Vector2i(int _x, int _y) { x = _x; y = _y; }

		public Vector2i Clone() { return new Vector2i(x, y); }

		public static readonly Vector2i Zero = new Vector2i();

		public Vector2i Multiply(int scaler)
		{
			return new Vector2i(x * scaler, y * scaler);
		}

		public static Vector2i SubVecD(Vector2i lhs, Vector2i rhs)
		{
			Vector2i r = new Vector2i();
			r.x = lhs.x - rhs.x;
			r.y = lhs.y - rhs.y;

			return r;
		}

		public double Magnitude()
		{
			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		public static double MagVecD(Vector2i vec)
		{
			return Math.Sqrt(Math.Pow(vec.x, 2) + Math.Pow(vec.y, 2));
		}

		public static double Distance(Vector2i lhs, Vector2i rhs)
		{
			return MagVecD(SubVecD(lhs, rhs));
		}

		public static bool TryParse(string text, out Vector2i val)
		{
			val = new Vector2i();
			var nums = text.Split(",".ToCharArray(), 3);
			if (nums.Length > 0)
			{
				if (!int.TryParse(nums[0], out val.x))
					return false;
			}

			if (nums.Length > 1)
			{
				if (!int.TryParse(nums[1], out val.y))
					return false;
			}

			return true;
		}
	}
}
