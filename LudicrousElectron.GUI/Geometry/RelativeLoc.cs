using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI.Geometry
{
    public class RelativeLoc
    {
        public enum Edge
        {
            Maximal,
            Minimal,
            Middle,
        }

        public Edge RelativeTo = Edge.Minimal;
        public float Paramater = 0;

        public RelativeLoc() { }

        public RelativeLoc(float param, Edge relative)
        {
            RelativeTo = relative;
            Paramater = param;
        }

        public RelativeLoc(RelativeLoc o, float offset)
        {
            RelativeTo = o.RelativeTo;
            Paramater = o.Paramater + offset;
        }

        public float ToScreen(int pixel)
        {
            float pixelOffset = pixel * Paramater;
            switch (RelativeTo)
            {
                default:
                case Edge.Minimal:
                    return pixelOffset;

                case Edge.Maximal:
                    return pixel - pixelOffset;

                case Edge.Middle:
                    return pixel * 0.5f + pixelOffset;
            }
        }

        public static RelativeLoc XLeft = new RelativeLoc(0, Edge.Minimal);
        public static RelativeLoc XRight = new RelativeLoc(0, Edge.Maximal);
        public static RelativeLoc XCenter = new RelativeLoc(0, Edge.Middle);
        public static RelativeLoc XFirstThird = new RelativeLoc(1f / 3f, Edge.Minimal);
        public static RelativeLoc XSecondThird = new RelativeLoc(1f / 3f, Edge.Maximal);

        public static RelativeLoc YLower = new RelativeLoc(0, Edge.Minimal);
        public static RelativeLoc YUpper = new RelativeLoc(0, Edge.Maximal);
        public static RelativeLoc YCenter = new RelativeLoc(0, Edge.Middle);
        public static RelativeLoc YFirstThird = new RelativeLoc(1f / 3f, Edge.Minimal);
        public static RelativeLoc YSecondThird = new RelativeLoc(1f / 3f, Edge.Maximal);
		public static RelativeLoc YUpperQuarter = new RelativeLoc(1f / 4f, Edge.Maximal);

		public static float BorderOffset = 0.025f;

        public static RelativeLoc XLeftBorder = new RelativeLoc(BorderOffset, Edge.Minimal);
        public static RelativeLoc XRightBorder = new RelativeLoc(BorderOffset, Edge.Maximal);
        public static RelativeLoc YLowerBorder = new RelativeLoc(BorderOffset, Edge.Minimal);
        public static RelativeLoc YUpperBorder = new RelativeLoc(BorderOffset, Edge.Maximal);

        public static RelativeLoc operator +(RelativeLoc o, float offset)
        {
            return new RelativeLoc(o, offset);
        }

        public static RelativeLoc operator -(RelativeLoc o, float offset)
        {
            return new RelativeLoc(o, -offset);
        }
    }

	public class RelativePoint
	{
		public RelativeLoc X = new RelativeLoc();
		public RelativeLoc Y = new RelativeLoc();

		public RelativePoint() { }
		public RelativePoint(RelativeLoc _x, RelativeLoc _y)
		{
			X = _x;
			Y = _y;
		}

		public static RelativePoint operator + (RelativePoint p, Vector2 shift)
		{
			return new RelativePoint(p.X + shift.X, p.Y + shift.Y);
		}

		public static RelativePoint Center = new RelativePoint(RelativeLoc.XCenter, RelativeLoc.YCenter);
        public static RelativePoint LowerLeft = new RelativePoint(RelativeLoc.XLeft, RelativeLoc.YLower);
		public static RelativePoint MiddleLeft = new RelativePoint(RelativeLoc.XLeft, RelativeLoc.YCenter);
		public static RelativePoint UpperLeft = new RelativePoint(RelativeLoc.XLeft, RelativeLoc.YUpper);
		public static RelativePoint UpperCenter = new RelativePoint(RelativeLoc.XCenter, RelativeLoc.YUpper);
		public static RelativePoint UpperRight = new RelativePoint(RelativeLoc.XRight, RelativeLoc.YUpper);
		public static RelativePoint MiddleRight = new RelativePoint(RelativeLoc.XRight, RelativeLoc.YCenter);
		public static RelativePoint LowerRight = new RelativePoint(RelativeLoc.XRight, RelativeLoc.YLower);
		public static RelativePoint LowerCenter = new RelativePoint(RelativeLoc.XCenter, RelativeLoc.YLower);

		public static RelativePoint UpperQuarterCenter = new RelativePoint(RelativeLoc.XCenter, RelativeLoc.YUpperQuarter);
		public static RelativePoint UpperThirdCenter = new RelativePoint(RelativeLoc.XCenter, RelativeLoc.YSecondThird);
	}

}
