using System;

using OpenTK;

namespace LudicrousElectron.Types
{
	public static class MathUtils
	{
		public static Vector2d ToVector2d(this double angle)
		{
			return new Vector2d(System.Math.Cos(angle), System.Math.Sin(angle));
		}

		public static Vector2d ToVector2d(this double angle, double lenght)
		{
			return new Vector2d(System.Math.Cos(angle) * lenght, System.Math.Sin(angle) * lenght);
		}

		public static double ToAngle(this Vector2d vec)
		{
			double len = vec.Length;

			return System.Math.Atan2(vec.Y / len, vec.X / len);
		}

		public static double ToDeg(this Vector2d vec)
		{
			double len = vec.Length;

			return System.Math.Atan2(vec.Y / len, vec.X / len) * DegCon;
		}

		public static double ToDeg(this double rad)
		{
			return rad * DegCon;
		}

		public static double ToRad(this double deg)
		{
			return deg * RadCon;
		}

		public static double NormalizeDeg(this double deg)
		{
			while (deg < -180)
				deg += 360;
			while (deg > 180)
				deg -= 360;

			return deg;
		}

		public static double NormalizeRad(this double rad)
		{
			while (rad < -System.Math.PI)
				rad += System.Math.PI * 2;
			while (rad > System.Math.PI)
				rad -= System.Math.PI * 2;

			return rad;
		}

		public static float ToDeg(this float rad)
		{
			return rad * (float)DegCon;
		}

		public static float ToRad(this float deg)
		{
			return deg * (float)RadCon;
		}

		public static float NormalizeDeg(this float deg)
		{
			while (deg < -180.0f)
				deg += 360.0f;
			while (deg > 180.0f)
				deg -= 360.0f;

			return deg;
		}

		public static float NormalizeRad(this float rad)
		{
			while (rad < -(float)System.Math.PI)
				rad += (float)System.Math.PI * 2.0f;
			while (rad > (float)System.Math.PI)
				rad -= (float)System.Math.PI * 2.0f;

			return rad;
		}

		public static double RadCon = System.Math.PI / 180.0;
		public static double DegCon = 180.0 / System.Math.PI;

		public static double Interpolate(double x, double x1, double y1, double x2, double y2)
		{
			/* return corresponding y on line x1,y1,x2,y2 for value x
	         *
	         *      (y2 -y1)/(x2 - x1) = (y - y1) / (x - x1)     by similar triangles.
	         *      (x -x1) * (y2 -y1)/(x2 -x1) = y - y1         a little algebra...
	         *      y = (x - x1) * (y2 - y1) / (x2 -x1) + y1;    I think there's one more step
	         *                                                   which would optimize this a bit more.
	         *                                                   but I forget how it goes. 
	         *
	         * Calling this with x2 == x1 is your own damn fault.
	         *
	         */
			return (x - x1) * (y2 - y1) / (x2 - x1) + y1;
		}

		public static double TableInterpolate(double x, double[] xv, double[] yv)
		{
			for (int i = 0; i < xv.Length - 1; i++)
			{
				if (xv[i] <= x && xv[i + 1] > x)
					return Interpolate(x, xv[i], yv[i], xv[i + 1], yv[i + 1]);
			}
			return 0.0;
		}
	}

	public class FloatHelper
	{
		protected static Random rand = new Random();

		public static float DefaultTolerance = 0.0001f;

		public static bool Equals(float f1, float f2)
		{
			return Equals(f1, f2, DefaultTolerance);
		}

		public static bool Equals(float f1, float f2, float tolerance)
		{
			return System.Math.Abs(f1 - f2) <= tolerance;
		}

		public static float Random(float max)
		{
			return (float)(rand.NextDouble() * max);
		}

		public static float Random(float min, float max)
		{
			return Random(max - min) + min;
		}

		public static float CopySign(float val, float sign)
		{
			return (float)(System.Math.Abs(val) * (sign / System.Math.Abs(sign)));
		}
	}

	public class Trig
	{
		public static double DegreeToRadian(double angle)
		{
			return System.Math.PI * angle / 180.0;
		}

		public static double RadianToDegree(double angle)
		{
			return angle * (180.0 / System.Math.PI);
		}

		public static float DegreeToRadian(float angle)
		{
			return (float)System.Math.PI * angle / 180.0f;
		}

		public static float RadianToDegree(float angle)
		{
			return angle * (180.0f / (float)System.Math.PI);
		}

		public static double Hypot(double x, double y)
		{
			return System.Math.Sqrt(x * x + y * y);
		}

		public static float Hypot(float x, float y)
		{
			return (float)System.Math.Sqrt(x * x + y * y);
		}

		public static float ToAngle(float x, float y)
		{
			return (float)System.Math.Atan2(y, x);
		}
	}

   

    public class FloatRand
	{
        private static Random RNG = new Random();

        public static float RandInRange(float min, float max)
		{
			return (float)(RNG.NextDouble() * (max - min) + min);
		}

		public static float RandPlusMinus()
		{
			return RandInRange(-1, 1);
		}
	}

	public class MatrixHelper4
	{
		public static Matrix4 FromQuaternion(Quaternion quat)
		{
			quat.Normalize();
			Matrix4 mat = new Matrix4();
			mat.M11 = 1 - 2 * (quat.Y * quat.Y) - 2 * (quat.Z * quat.Z);
			mat.M12 = 2 * (quat.X * quat.Y) - 2 * (quat.Z * quat.W);
			mat.M13 = 2 * (quat.X * quat.Z) + 2 * (quat.Y * quat.W);

			mat.M21 = 2 * (quat.X * quat.Y) + 2 * (quat.Z * quat.W);
			mat.M22 = 1 - 2 * (quat.X * quat.X) - 2 * (quat.Z * quat.Z);
			mat.M23 = 2 * (quat.Y * quat.Z) + 2 * (quat.X * quat.W);

			mat.M31 = 2 * (quat.X * quat.Z) + 2 * (quat.Y * quat.W);
			mat.M32 = 2 * (quat.Y * quat.Z) + 2 * (quat.X * quat.W);
			mat.M32 = 1 - 2 * (quat.X * quat.X) - 2 * (quat.Y * quat.Y);

			mat.Transpose();
			return mat;
		}

		// matrix grid methods
		public static float M11(Matrix4 m) { return m.Row0.X; }
		public static void M11(ref Matrix4 m, float value) { m.Row0.X = value; }

		public static float M12(Matrix4 m) { return m.Row0.Y; }
		public static void M12(ref Matrix4 m, float value) { m.Row0.Y = value; }

		public static float M13(Matrix4 m) { return m.Row0.Z; }
		public static void M13(ref Matrix4 m, float value) { m.Row0.Z = value; }

		public static float M14(Matrix4 m) { return m.Row0.W; }
		public static void M14(ref Matrix4 m, float value) { m.Row0.W = value; }

		public static float M21(Matrix4 m) { return m.Row1.X; }
		public static void M21(ref Matrix4 m, float value) { m.Row1.X = value; }

		public static float M22(Matrix4 m) { return m.Row1.Y; }
		public static void M22(ref Matrix4 m, float value) { m.Row1.Y = value; }

		public static float M23(Matrix4 m) { return m.Row1.Z; }
		public static void M23(ref Matrix4 m, float value) { m.Row1.Z = value; }

		public static float M24(Matrix4 m) { return m.Row1.W; }
		public static void M24(ref Matrix4 m, float value) { m.Row1.W = value; }

		public static float M31(Matrix4 m) { return m.Row2.X; }
		public static void M31(ref Matrix4 m, float value) { m.Row2.X = value; }

		public static float M32(Matrix4 m) { return m.Row2.Y; }
		public static void M32(ref Matrix4 m, float value) { m.Row2.Y = value; }

		public static float M33(Matrix4 m) { return m.Row2.Z; }
		public static void M33(ref Matrix4 m, float value) { m.Row2.Z = value; }

		public static float M34(Matrix4 m) { return m.Row2.W; }
		public static void M34(ref Matrix4 m, float value) { m.Row2.W = value; }

		public static float M41(Matrix4 m) { return m.Row3.X; }
		public static void M41(ref Matrix4 m, float value) { m.Row3.X = value; }

		public static float M42(Matrix4 m) { return m.Row3.Y; }
		public static void M42(ref Matrix4 m, float value) { m.Row3.Y = value; }

		public static float M43(Matrix4 m) { return m.Row3.Z; }
		public static void M43(ref Matrix4 m, float value) { m.Row3.Z = value; }

		public static float M44(Matrix4 m) { return m.Row3.W; }
		public static void M44(ref Matrix4 m, float value) { m.Row3.W = value; }

		// matrix index methods
		//         public static float m0(Matrix4 m) { return m.Row0.X; }
		//         public static void m0(ref Matrix4 m, float value) { m.Row0.X = value; }
		// 
		//         public static float m1(Matrix4 m) { return m.Row0.Y; }
		//         public static void m1(ref Matrix4 m, float value) { m.Row0.Y = value; }
		// 
		//         public static float m2(Matrix4 m) { return m.Row0.Z; }
		//         public static void m2(ref Matrix4 m, float value) { m.Row0.Z = value; }
		//      
		//         public static float m3(Matrix4 m) { return m.Row0.W; }
		//         public static void m3(ref Matrix4 m, float value) { m.Row0.W = value; }
		// 
		//         public static float m4(Matrix4 m) { return m.Row1.X; }
		//         public static void m4(ref Matrix4 m, float value) { m.Row1.X = value; }
		// 
		//         public static float m5(Matrix4 m) { return m.Row1.Y; }
		//         public static void m5(ref Matrix4 m, float value) { m.Row1.Y = value; }
		// 
		//         public static float m6(Matrix4 m) { return m.Row1.Z; }
		//         public static void m6(ref Matrix4 m, float value) { m.Row1.Z = value; }
		// 
		//         public static float m7(Matrix4 m) { return m.Row1.W; }
		//         public static void m7(ref Matrix4 m, float value) { m.Row1.W = value; }
		// 
		//         public static float m8(Matrix4 m) { return m.Row2.X; }
		//         public static void m8(ref Matrix4 m, float value) { m.Row2.X = value; }
		// 
		//         public static float m9(Matrix4 m) { return m.Row2.Y; }
		//         public static void m9(ref Matrix4 m, float value) { m.Row1.Y = value; }
		// 
		//         public static float m10(Matrix4 m) { return m.Row2.Z; }
		//         public static void m10(ref Matrix4 m, float value) { m.Row2.Z = value; }
		// 
		//         public static float m11(Matrix4 m) { return m.Row2.W; }
		//         public static void m11(ref Matrix4 m, float value) { m.Row2.W = value; }
		// 
		//         public static float m12(Matrix4 m) { return m.Row3.X; }
		//         public static void m12(ref Matrix4 m, float value) { m.Row3.X = value; }
		// 
		//         public static float m13(Matrix4 m) { return m.Row3.Y; }
		//         public static void m13(ref Matrix4 m, float value) { m.Row3.Y = value; }
		// 
		//         public static float m14(Matrix4 m) { return m.Row3.Z; }
		//         public static void m14(ref Matrix4 m, float value) { m.Row3.Z = value; }
		//      
		//         public static float m15(Matrix4 m) { return m.Row3.W; }
		//         public static void m15(ref Matrix4 m, float value) { m.Row3.W = value; }

		// col major
		public static float m0(Matrix4 m) { return m.Row0.X; }
		public static void m0(ref Matrix4 m, float value) { m.Row0.X = value; }

		public static float m1(Matrix4 m) { return m.Row1.X; }
		public static void m1(ref Matrix4 m, float value) { m.Row1.X = value; }

		public static float m2(Matrix4 m) { return m.Row2.X; }
		public static void m2(ref Matrix4 m, float value) { m.Row2.X = value; }

		public static float m3(Matrix4 m) { return m.Row3.X; }
		public static void m3(ref Matrix4 m, float value) { m.Row3.X = value; }

		public static float m4(Matrix4 m) { return m.Row0.Y; }
		public static void m4(ref Matrix4 m, float value) { m.Row0.Y = value; }

		public static float m5(Matrix4 m) { return m.Row1.Y; }
		public static void m5(ref Matrix4 m, float value) { m.Row1.Y = value; }

		public static float m6(Matrix4 m) { return m.Row2.Y; }
		public static void m6(ref Matrix4 m, float value) { m.Row2.Y = value; }

		public static float m7(Matrix4 m) { return m.Row3.Y; }
		public static void m7(ref Matrix4 m, float value) { m.Row2.Y = value; }

		public static float m8(Matrix4 m) { return m.Row0.Z; }
		public static void m8(ref Matrix4 m, float value) { m.Row0.Z = value; }

		public static float m9(Matrix4 m) { return m.Row1.Z; }
		public static void m9(ref Matrix4 m, float value) { m.Row1.Z = value; }

		public static float m10(Matrix4 m) { return m.Row2.Z; }
		public static void m10(ref Matrix4 m, float value) { m.Row2.Z = value; }

		public static float m11(Matrix4 m) { return m.Row3.Z; }
		public static void m11(ref Matrix4 m, float value) { m.Row3.Z = value; }

		public static float m12(Matrix4 m) { return m.Row0.W; }
		public static void m12(ref Matrix4 m, float value) { m.Row0.W = value; }

		public static float m13(Matrix4 m) { return m.Row1.W; }
		public static void m13(ref Matrix4 m, float value) { m.Row1.W = value; }

		public static float m14(Matrix4 m) { return m.Row2.W; }
		public static void m14(ref Matrix4 m, float value) { m.Row2.W = value; }

		public static float m15(Matrix4 m) { return m.Row3.W; }
		public static void m15(ref Matrix4 m, float value) { m.Row3.W = value; }


		public static void SetRotationRadians(ref Matrix4 mat, Vector3 angles)
		{
			double cr = System.Math.Cos(angles.X);
			double sr = System.Math.Sin(angles.X);
			double cp = System.Math.Cos(angles.Y);
			double sp = System.Math.Sin(angles.Y);
			double cy = System.Math.Cos(angles.Z);
			double sy = System.Math.Sin(angles.Z);

			mat.M11 = (float)(cp * cy);
			mat.M12 = (float)(cp * sy);
			mat.M13 = (float)(-sp);
			mat.M14 = (float)(0.0f);

			double srsp = sr * sp;
			double crsp = cr * sp;

			mat.M21 = (float)(srsp * cy - cr * sy);
			mat.M22 = (float)(srsp * sy + cr * cy);
			mat.M23 = (float)(sr * cp);

			mat.M31 = (float)(crsp * cy + sr * sy);
			mat.M32 = (float)(crsp * sy - sr * cy);
			mat.M34 = (float)(cr * cp);
		}

		public static void SetTranslation(ref Matrix4 mat, Vector3 translation)
		{
			mat.M41 = translation.X;
			mat.M42 = translation.Y;
			mat.M43 = translation.Z;
		}
	}
	public class QuaternionHelper
	{
		public static Quaternion FromEuler(Vector3 angles)
		{
			double x, y, z, w;

			double c1 = System.Math.Cos(angles.Y * 0.5f);
			double c2 = System.Math.Cos(angles.Z * 0.5f);
			double c3 = System.Math.Cos(angles.X * 0.5f);

			double s1 = System.Math.Sin(angles.Y * 0.5f);
			double s2 = System.Math.Sin(angles.Z * 0.5f);
			double s3 = System.Math.Sin(angles.X * 0.5f);

			w = c1 * c2 * c3 - s1 * s2 * s3;
			x = s1 * s2 * c3 + c1 * c2 * s3;
			y = s1 * c2 * c3 + c1 * s2 * s3;
			z = c1 * s2 * c3 - s1 * c2 * s3;

			return new Quaternion((float)x, (float)y, (float)z, (float)w);
		}

		public static Quaternion FromMatrix(Matrix4 m1)
		{
			//             if (false)
			//             {
			//                 float w = (float)Math.Sqrt(1.0 + m1.M11 + m1.M22 + m1.M33) / 2.0f;
			//                 float w4 = (4.0f * w);
			//                 float x = (m1.M32 - m1.M23) / w4;
			//                 float y = (m1.M13 - m1.M31) / w4;
			//                 float z = (m1.M21 - m1.M12) / w4;
			// 
			//                 return new Quaternion(x, y, z, w);
			//             }

			float w = (float)System.Math.Sqrt(System.Math.Max(0, 1 + m1.M11 + m1.M22 + m1.M33)) / 2f;
			float x = (float)System.Math.Sqrt(System.Math.Max(0, 1 + m1.M11 - m1.M22 - m1.M33)) / 2f;
			float y = (float)System.Math.Sqrt(System.Math.Max(0, 1 - m1.M11 + m1.M22 - m1.M33)) / 2f;
			float z = (float)System.Math.Sqrt(System.Math.Max(0, 1 - m1.M11 - m1.M22 + m1.M33)) / 2f;
			x = FloatHelper.CopySign(x, m1.M32 - m1.M23);
			y = FloatHelper.CopySign(y, m1.M12 - m1.M31);
			z = FloatHelper.CopySign(z, m1.M21 - m1.M12);
			return new Quaternion(x, y, z, w);
		}
	}

}
