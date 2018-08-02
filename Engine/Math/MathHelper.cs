using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Engine.Math
{
    public static class MathHelper
    {
        internal static readonly float MachineEpsilonFloat = GetMachineEpsilonFloat();

        public static float ToDegrees (float rad)
        {
            return rad * (float)(180 / System.Math.PI);
        }

        public static float ToRadians(float rad)
        {
            return rad * (float)(System.Math.PI/180);
        }

        public static OpenTK.Vector2 Vector2FromAngle(float angle)
        {
            angle = ToRadians(angle);
            return new OpenTK.Vector2((float)System.Math.Cos(angle), (float)System.Math.Sign(angle));
        }

        public static OpenTK.Vector3 Vector3FromAngle(float angle, bool zUp = false)
        {
            OpenTK.Vector2 vec = Vector2FromAngle(angle);
            if (zUp)
                return new OpenTK.Vector3(vec.X, vec.Y, 0);
            else
                return new OpenTK.Vector3(vec.X, 0, vec.Y);
        }

        internal static bool WithinEpsilon(float floatA, float floatB)
        {
            return System.Math.Abs(floatA - floatB) < MachineEpsilonFloat;
        }

        private static float GetMachineEpsilonFloat()
        {
            float machineEpsilon = 1.0f;
            float comparison;

            /* Keep halving the working value of machineEpsilon until we get a number that
			 * when added to 1.0f will still evaluate as equal to 1.0f.
			 */
            do
            {
                machineEpsilon *= 0.5f;
                comparison = 1.0f + machineEpsilon;
            }
            while (comparison > 1.0f);

            return machineEpsilon;
        }
    }
}
