using LudicrousElectron.GUI.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI
{
	public class HorzontalLayoutGroup : LayoutContainer
    {
        public bool FitChildToHeight = true;
        public bool ConstantChildWidths = true;

        public bool LeftToRight = true;

        public HorzontalLayoutGroup() : base(null) { }

        public HorzontalLayoutGroup(RelativeRect rect) : base(rect)
        {
            IgnoreMouse = true;
        }

        public override void AddChild(GUIElement child)
        {
            if (FitChildToHeight)
                child.Rect.Height = RelativeSize.FullHeight;

            base.AddChild(child);

            if (Inited)
                Resize((int)LastParentSize.X, (int)LastParentSize.Y);
        }

        public override void Resize(int x, int y)
        {
            LastParentSize.X = x;
            LastParentSize.Y = y;

            Inited = true;

            Rect.Resize(x, y);
            var PixelSize = Rect.GetPixelSize();

            if (ConstantChildWidths)
            {
                float childSize = (PixelSize.Y - (Children.Count * ChildSpacing)) / (Children.Count);
                if (childSize > MaxChildSize)
                    childSize = MaxChildSize;

                float originX = FirstElementHasSpacing ? ChildSpacing : 0;
                foreach (var child in Children)
                {
                    // we adjust each child rect to be raw before we resize them so they fit where we want them to fit.

                    if (LeftToRight)
                        child.Rect.X.Paramater = PixelSize.X - originX - childSize;
                    else
                        child.Rect.X.Paramater = originX;

                    child.Rect.X.RelativeTo = RelativeLoc.Edge.Raw;
                    child.Rect.Height.Paramater = childSize;
                    child.Rect.Height.Mode = RelativeSize.SizeModes.Raw;

                    child.Rect.AnchorLocation = OriginLocation.LowerLeft;

                    child.Resize((int)PixelSize.X, (int)PixelSize.Y);

                    originX += childSize + ChildSpacing;
                }
            }
            else
            {
                float originX = ChildSpacing;

                foreach (var child in Children)
                {
                    // we resize the child to get it's height first
                    child.Resize((int)PixelSize.X, (int)PixelSize.Y);

                    float thisWidth = child.Rect.GetPixelSize().Y;

                    if (LeftToRight)
                        child.Rect.Y.Paramater = PixelSize.X - originX - thisWidth;
                    else
                        child.Rect.Y.Paramater = originX;

                    child.Rect.AnchorLocation = GetLowerAnchor(child.Rect.AnchorLocation);

                    // resize the child with the correct origin
                    child.Resize((int)PixelSize.X, (int)PixelSize.Y);

                    originX += thisWidth + ChildSpacing;
                }
            }
        }
    }
}

