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
