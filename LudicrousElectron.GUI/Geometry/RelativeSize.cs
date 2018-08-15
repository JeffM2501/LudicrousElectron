using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI.Geometry
{
    public class RelativeSize
    {
        public bool UseWidth = true;
        public float Paramater = 0;

		public bool Raw = false; // param is a raw pixel value already

        public RelativeSize() { }

        public RelativeSize(float param, bool width)
        {
            Paramater = param;
            UseWidth = width;
        }

        public RelativeSize(RelativeSize o, float offset)
        {
            Paramater = o.Paramater - offset;
            UseWidth = o.UseWidth;
        }

        public float ToScreen(int x, int y)
        {
            return Raw ? Paramater : ((UseWidth ? x : y) * Paramater);
        }

        public static RelativeSize FullWidth = new RelativeSize(1, true);
        public static RelativeSize ThreeQuarterWidth = new RelativeSize(0.75f, true);
        public static RelativeSize HalfWidth = new RelativeSize(0.5f, true);
        public static RelativeSize ThirdWidth = new RelativeSize(1.0f / 3.0f, true);
        public static RelativeSize QuarterWidth = new RelativeSize(0.25f, true);
        public static RelativeSize EightWidth = new RelativeSize(0.125f, true);

        public static RelativeSize FullHeight = new RelativeSize(1, false);
        public static RelativeSize TwoThirdHeight = new RelativeSize(2.0f / 3.0f, false);
        public static RelativeSize HalfHeight = new RelativeSize(0.5f, false);
        public static RelativeSize ThirdHeight = new RelativeSize(1.0f / 3.0f, false);
        public static RelativeSize QuarterHeight = new RelativeSize(0.25f, false);
        public static RelativeSize EightHeight = new RelativeSize(0.125f, false);

        public static RelativeSize BorderInsetWidth = new RelativeSize(1 - (RelativeLoc.BorderOffset * 2), true);
        public static RelativeSize BorderInsetHeight = new RelativeSize(1 - (RelativeLoc.BorderOffset * 2), false);

        public static RelativeSize BorderWidth = new RelativeSize(RelativeLoc.BorderOffset, true);
        public static RelativeSize BorderHeight = new RelativeSize(RelativeLoc.BorderOffset, false);

        public static RelativeSize operator +(RelativeSize o, float offset)
        {
            return new RelativeSize(o, offset);
        }

        public static RelativeSize operator -(RelativeSize o, float offset)
        {
            return new RelativeSize(o, -offset);
        }
    }
}
