using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI.Geometry
{
    public class RelativeSize
    {
        public float Paramater = 0;

        public enum SizeModes
        {
            Width,
            Height,
            Raw,
        }
        public SizeModes Mode = SizeModes.Width;

        public RelativeSize() { }

        public RelativeSize(float param, bool width)
        {
            Paramater = param;
            Mode = width ? SizeModes.Width : SizeModes.Height;
        }

		public RelativeSize(float param)
		{
			Paramater = param;
            Mode = SizeModes.Raw;
        }

		public RelativeSize(RelativeSize o, float offset)
        {
            Paramater = o.Paramater - offset;
            Mode = o.Mode;
        }

        public RelativeSize Clone()
        {
            return new RelativeSize(this, 0);
        }

		public float ToScreen(float x, float y)
		{
			return ToScreen((int)x, (int)y);
		}

		public float ToScreen(Vector2 size)
		{
			return ToScreen(size.X,size.Y);
		}

		public float ToScreen(int x, int y)
        {
            switch (Mode)
            {
                case SizeModes.Width:
                    return x * Paramater;

                case SizeModes.Height:
                    return y * Paramater;

                default:
                    return Paramater;
            }
        }

        public static RelativeSize FullWidth = new RelativeSize(1, true);
        public static RelativeSize ThreeQuarterWidth = new RelativeSize(0.75f, true);
        public static RelativeSize HalfWidth = new RelativeSize(0.5f, true);
		public static RelativeSize HalfBorderInsetWidth = new RelativeSize(0.5f - (RelativeLoc.BorderOffset * 2), true);

        public static RelativeSize TwoColumnWidth = new RelativeSize(0.5f - (RelativeLoc.BorderOffset * 3.0f), true);
        public static RelativeSize ThreeColumnWidth = new RelativeSize(0.3f, true);

        public static RelativeSize ThirdWidth = new RelativeSize(1.0f / 3.0f, true);
        public static RelativeSize QuarterWidth = new RelativeSize(0.25f, true);
        public static RelativeSize EightWidth = new RelativeSize(0.125f, true);

        public static RelativeSize FullHeight = new RelativeSize(1, false);
		public static RelativeSize SevenEightsHeight = new RelativeSize(7.0f/8.0f, false);
		public static RelativeSize ThreeQuarterHeight = new RelativeSize(0.75f, false);
		public static RelativeSize TwoThirdHeight = new RelativeSize(2.0f / 3.0f, false);
        public static RelativeSize HalfHeight = new RelativeSize(0.5f, false);
        public static RelativeSize ThirdHeight = new RelativeSize(1.0f / 3.0f, false);
        public static RelativeSize QuarterHeight = new RelativeSize(0.25f, false);
        public static RelativeSize EightHeight = new RelativeSize(0.125f, false);

        public static RelativeSize BorderInsetWidth = new RelativeSize(1 - (RelativeLoc.BorderOffset * 2), true);
        public static RelativeSize BorderInsetHeight = new RelativeSize(1 - (RelativeLoc.BorderOffset * 2), false);

        public static RelativeSize BorderWidth = new RelativeSize(RelativeLoc.BorderOffset, true);
        public static RelativeSize BorderHeight = new RelativeSize(RelativeLoc.BorderOffset, false);

        public static RelativeSize FixedPixelSize (int pixels) { return new RelativeSize(pixels); }
        public static RelativeSize FixedPixelSize(float pixels) { return new RelativeSize(pixels); }

        public static RelativeSize operator + (RelativeSize o, float offset)
        {
            return new RelativeSize(o, offset);
        }

        public static RelativeSize operator - (RelativeSize o, float offset)
        {
            return new RelativeSize(o, -offset);
        }

        public static RelativeSize operator * (RelativeSize o, float factor)
        {
            RelativeSize loc = new RelativeSize(o, 0);
            loc.Paramater *= factor;
            return loc;
        }

        public static RelativeSize operator / (RelativeSize o, float factor)
        {
            RelativeSize loc = new RelativeSize(o, 0);
            loc.Paramater /= factor;
            return loc;
        }
    }

	public class RelativeSizeXY
	{
		public RelativeSize Width = RelativeSize.FullWidth;
		public RelativeSize Height = RelativeSize.HalfHeight;

		public RelativeSizeXY() { }

		public RelativeSizeXY(RelativeSize widht, RelativeSize height)
		{
			Width = widht;
			Height = height;
		}

        public RelativeSizeXY(float widht, float height)
        {
            Width = new RelativeSize(widht);
            Height = new RelativeSize(height);
        }

        public RelativeSizeXY(Vector2 size)
        {
            Width = new RelativeSize(size.X);
            Height = new RelativeSize(size.Y);
        }

        public RelativeSizeXY Clone()
        {
            return new RelativeSizeXY(Width.Clone(), Height.Clone());
        }

		public static RelativeSizeXY Full = new RelativeSizeXY(RelativeSize.FullWidth, RelativeSize.FullHeight);
		public static RelativeSizeXY ThreeQuarter = new RelativeSizeXY(RelativeSize.ThreeQuarterWidth, RelativeSize.ThreeQuarterHeight);
		public static RelativeSizeXY Half = new RelativeSizeXY(RelativeSize.HalfWidth, RelativeSize.HalfHeight);
		public static RelativeSizeXY Third = new RelativeSizeXY(RelativeSize.ThirdWidth, RelativeSize.ThirdHeight);
		public static RelativeSizeXY Quarter = new RelativeSizeXY(RelativeSize.QuarterWidth, RelativeSize.QuarterHeight);
		public static RelativeSizeXY Eight = new RelativeSizeXY(RelativeSize.EightWidth, RelativeSize.EightHeight);

		public static RelativeSizeXY BorderInset = new RelativeSizeXY(RelativeSize.BorderInsetWidth, RelativeSize.BorderInsetHeight);
		public static RelativeSizeXY Border = new RelativeSizeXY(RelativeSize.BorderWidth, RelativeSize.BorderWidth);

	}
}
