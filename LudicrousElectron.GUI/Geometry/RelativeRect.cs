using System;
using System.Drawing;

using OpenTK;

namespace LudicrousElectron.GUI.Geometry
{
	public enum OriginLocation
	{
		Center,
		LowerLeft,
		MiddleLeft,
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleRight,
		LowerRight,
		LowerCenter,
	}

	public class RelativeRect
    {
		public static readonly RelativeRect Full = new RelativeRect(RelativeLoc.XCenter, RelativeLoc.YCenter, RelativeSize.FullWidth, RelativeSize.FullHeight);

		/// <summary>
		/// X position of the origin relative to the parent.
		/// </summary>
        public RelativeLoc X = new RelativeLoc();

		/// <summary>
		/// Y position of the origin relative to the parent
		/// </summary>
		public RelativeLoc Y = new RelativeLoc();

		/// <summary>
		///  total width of the rect relative to its parent.
		/// </summary>
		public RelativeSize Width = new RelativeSize();

		/// <summary>
		///  total height of the rect relative to its parent.
		/// </summary>
		public RelativeSize Height = new RelativeSize();

		/// <summary>
		/// What point on the rect the X and Y origin represent
		/// </summary>
		public OriginLocation AnchorLocation = OriginLocation.Center;

		protected Vector2 PixelOrigin = Vector2.Zero;
        protected Vector2 PixelSize = Vector2.Zero;
        protected Size ParrentPixelSize = new Size(0, 0);
        protected float InscribedRadius = 0;

        public Vector2 GetPixelSize() { return PixelSize; }
        public Vector2 GetPixelOrigin() { return PixelOrigin; }

        public float Radius { get { return InscribedRadius; } }

        public RelativeRect()
        {
        }

        public RelativeRect(RelativeLoc x, RelativeLoc y, RelativeSize w, RelativeSize h)
        {
            Set(x, y, w, h);
        }

		public RelativeRect(RelativeLoc x, RelativeLoc y, RelativeSize w, RelativeSize h, OriginLocation anchor)
		{
			Set(x, y, w, h);
			AnchorLocation = anchor;
		}

		public virtual void Set(RelativeLoc x, RelativeLoc y, RelativeSize w, RelativeSize h)
        {
            SetLocation(x, y);
            SetSize(w, h);
        }

		public virtual void Set(RelativeLoc x, RelativeLoc y, RelativeSize w, RelativeSize h, OriginLocation anchor)
		{
			SetLocation(x, y);
			SetSize(w, h);
			AnchorLocation = anchor;
		}

		public RelativeRect(RelativePoint origin, RelativeSizeXY size)
		{
			SetLocation(origin.X, origin.Y);
			SetSize(size.Width, size.Height);
		}

		public RelativeRect(RelativePoint origin, RelativeSizeXY size, OriginLocation anchor)
		{
			SetLocation(origin.X, origin.Y);
			SetSize(size.Width, size.Height);
			AnchorLocation = anchor;
		}

        public RelativeRect Clone()
        {
            RelativeRect rect = new RelativeRect(X.Clone(), Y.Clone(), Width.Clone(), Height.Clone());

            rect.InscribedRadius = InscribedRadius;
            rect.PixelOrigin = PixelOrigin;
            rect.ParrentPixelSize = ParrentPixelSize;
            rect.AnchorLocation = AnchorLocation;

            return rect;
        }

		public virtual bool PointInRect(Vector2 position)
		{
			if (position.X < PixelOrigin.X || position.Y < PixelOrigin.Y)
				return false;

			if (position.X > PixelOrigin.X + PixelSize.X || position.Y > PixelOrigin.Y + PixelSize.Y)
				return false;

			return true;
		}

		public virtual void Set(RelativePoint origin, RelativeSizeXY size)
		{
			SetLocation(origin.X, origin.Y);
			SetSize(size.Width, size.Height);
		}

		public virtual void Set(RelativePoint origin, RelativeSizeXY size, OriginLocation anchor)
		{
			SetLocation(origin.X, origin.Y);
			SetSize(size.Width, size.Height);
			AnchorLocation = anchor;
		}

		public virtual void SetLocation(RelativeLoc x, RelativeLoc y)
        {
            X = x.Clone();
            Y= y.Clone();
        }

        public virtual void SetSize(RelativeSize w, RelativeSize h)
        {
            Width = w.Clone();
            Height = h.Clone();
        }
		public virtual void SetAnchor(OriginLocation anchor)
		{
			AnchorLocation = anchor;
		}

		protected Vector2 ComputePixelOrigin(int x, int y)
		{
			Vector2 halfSize = PixelSize * 0.5f;

			// where is the origin relative to our parent
			float xOffset = X.ToScreen(x);
			float yOffset = Y.ToScreen(y);
			
			switch(AnchorLocation)
			{
				case OriginLocation.Center:
					xOffset -= halfSize.X;
					yOffset -= halfSize.Y;
					break;

				case OriginLocation.UpperCenter:
					xOffset -= halfSize.X;
					yOffset -= halfSize.Y + (PixelSize.Y * 0.5f);
					break;

				case OriginLocation.LowerCenter:
					xOffset -= halfSize.X;
					yOffset -= halfSize.Y - (PixelSize.Y * 0.5f);
					break;

				case OriginLocation.LowerRight:
					xOffset -= halfSize.X * 2;
					break;

				case OriginLocation.MiddleLeft:
					yOffset -= halfSize.Y;
					break;

				case OriginLocation.MiddleRight:
					xOffset -= halfSize.X * 2;
					yOffset -= halfSize.Y;
					break;

				case OriginLocation.UpperLeft:
					yOffset -= halfSize.Y + (PixelSize.Y * 0.5f);
					break;

				case OriginLocation.UpperRight:
					xOffset -= halfSize.X * 2;
					yOffset -= halfSize.Y + (PixelSize.Y * 0.5f);
					break;

                case OriginLocation.LowerLeft:
                default:
					break;
			}


			return new Vector2(xOffset, yOffset);
		}

        public virtual void Resize(int x, int y)
        {
            ParrentPixelSize = new Size(x, y);

            // figure out how big we are in pixels
            PixelSize = new Vector2(Width.ToScreen(x, y), Height.ToScreen(x, y));
			PixelOrigin = ComputePixelOrigin(x, y);

            // pick the smallest value, that's the largest radius we can fit
            InscribedRadius = (float)PixelSize.X;
            if (PixelSize.Y < PixelSize.X)
                InscribedRadius = (float)PixelSize.Y;
        }
    }
}
