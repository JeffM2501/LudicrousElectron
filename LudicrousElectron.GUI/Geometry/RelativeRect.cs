using System;
using System.Drawing;

using OpenTK;

namespace LudicrousElectron.GUI.Geometry
{
    public class RelativeRect
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

        public RelativeLoc X = new RelativeLoc();
        public RelativeLoc Y = new RelativeLoc();

        public RelativeSize Width = new RelativeSize();
        public RelativeSize Height = new RelativeSize();

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

        public virtual void Set(RelativeLoc x, RelativeLoc y, RelativeSize w, RelativeSize h)
        {
            SetLocation(x, y);
            SetSize(w, h);
        }

        public virtual void SetLocation(RelativeLoc x, RelativeLoc y)
        {
            X = x;
            Y= y;
        }

        public virtual void SetSize(RelativeSize w, RelativeSize h)
        {
            Width = w;
            Height = h;
        }

        public virtual void Resize(int x, int y)
        {
            ParrentPixelSize = new Size(x, y);

            // figure out how big we are in pixels
            PixelSize = new Vector2(Width.ToScreen(x, y), Height.ToScreen(x, y));

            Vector2 halfSize = PixelSize * 0.5f;

            // see where our lower left is relative to our parent in pixels
            float xOffset = (float)X.Paramater * x;
            float yOffset = (float)Y.Paramater * y;

            if (X.RelativeTo == RelativeLoc.Edge.Middle)       // middle alignment means our center X is xOffset from parent center
                PixelOrigin.X = ((ParrentPixelSize.Width * 0.5f) + xOffset) - halfSize.X;
            else if (X.RelativeTo == RelativeLoc.Edge.Maximal) // maximal alignment means our right X is xOffset from parent right
                PixelOrigin.X = ((ParrentPixelSize.Width) - xOffset) - (PixelSize.X);
            else
                PixelOrigin.X = xOffset;                                                     // minimal alignment means our left X is xOffset from parent left(aka 0)

            if (Y.RelativeTo == RelativeLoc.Edge.Middle)       // middle alignment means our center X is xOffset from parent center
                PixelOrigin.Y = ((ParrentPixelSize.Height * 0.5f) + yOffset) - halfSize.Y;
            else if (Y.RelativeTo == RelativeLoc.Edge.Maximal) // maximal alignment means our right X is xOffset from parent right
                PixelOrigin.Y = ((ParrentPixelSize.Height) + yOffset) - (PixelSize.Y);
            else
                PixelOrigin.Y = yOffset;                                                     // minimal alignment means our left X is xOffset from parent left(aka 0)


            // pick the smallest value, that's the largest radius we can fit
            InscribedRadius = (float)PixelSize.X;
            if (PixelSize.Y < PixelSize.X)
                InscribedRadius = (float)PixelSize.Y;
        }

        public void PushOriginMatrix(OriginLocation origin, GUIRenderLayer layer)
        {
            switch (origin)
            {
                case OriginLocation.LowerLeft: // where we start
                    layer.MatrixStack.Push(Matrix4.Identity);
                    break;

                case OriginLocation.MiddleLeft:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(0, PixelSize.Y, 0));
                    break;

                case OriginLocation.UpperLeft:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(0, PixelSize.Y * 2, 0));
                    break;

                case OriginLocation.UpperCenter:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(PixelSize.X, PixelSize.Y, 0));
                    break;

                case OriginLocation.UpperRight:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(PixelSize.X * 2, PixelSize.Y * 2, 0));
                    break;

                case OriginLocation.MiddleRight:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(PixelSize.X * 2, PixelSize.Y, 0));
                    break;

                case OriginLocation.LowerRight:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(PixelSize.X * 2, 0, 0));
                    break;

                case OriginLocation.LowerCenter:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(PixelSize.X, 0, 0));
                    break;

                case OriginLocation.Center:
                    layer.MatrixStack.Push(Matrix4.CreateTranslation(PixelSize.X, PixelSize.Y, 0));
                    break;
            }
        }
    }
}
