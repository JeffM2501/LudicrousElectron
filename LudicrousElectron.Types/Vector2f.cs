using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Types
{
	public class Vector2f
	{
		public float x = 0;
		public float y = 0;


		public Vector2f() { }
		public Vector2f(float _x, float _y) { x = _x; y = _y; }

		public Vector2f Clone() { return new Vector2f(x, y); }

		public static readonly Vector2f Zero = new Vector2f();

		public Vector2f Multiply(float scaler)
		{
			return new Vector2f(x * scaler, y * scaler);
		}

		public static Vector2f SubVecD(Vector2f lhs, Vector2f rhs)
		{
			Vector2f r = new Vector2f();
			r.x = lhs.x - rhs.x;
			r.y = lhs.y - rhs.y;

			return r;
		}

		public double Magnitude()
		{
			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		public static double MagVecD(Vector2f vec)
		{
			return Math.Sqrt(Math.Pow(vec.x, 2) + Math.Pow(vec.y, 2));
		}

		public static double Distance(Vector2f lhs, Vector2f rhs)
		{
			return MagVecD(SubVecD(lhs, rhs));
		}

		public static bool TryParse(string text, out Vector2f val)
		{
			val = new Vector2f();
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

			return true;
		}
	}
}
