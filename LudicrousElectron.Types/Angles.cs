using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Types
{
	public static class Angles
	{
		public static float Difference(float angle_a, float angle_b)
		{
			float ret = (angle_b - angle_a);
			while (ret > 180) ret -= 360;
			while (ret < -180) ret += 360;
			return ret;
		}

		public static bool Near(float inAngle, float compareAngle, float tollerance)
		{
			return Math.Abs(Difference(inAngle, compareAngle)) <= tollerance;
		}
	}
}
