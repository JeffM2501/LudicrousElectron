using LudicrousElectron.GUI.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI
{
	public class VerticalLayoutGroup : LayoutContainer
	{
		public bool FitChildToWidth = true;

		public int ChildSpacing = 5;
		public int MaxChildSize = -1;

		public bool TopDown = true;

		public VerticalLayoutGroup() : base(null) { }

		public VerticalLayoutGroup(RelativeRect rect) : base(rect)
		{
			IgnoreMouse = true;
		}

		public override void AddChild(GUIElement child)
		{
			if (FitChildToWidth)
				child.Rect.Width = RelativeSize.FullWidth;

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

			float childSize = (PixelSize.Y - (Children.Count * ChildSpacing)) / (Children.Count);
			if (childSize > MaxChildSize)
				childSize = MaxChildSize;

			float originY = ChildSpacing;
			foreach (var child in Children)
			{
				// we adjust each child rect to be raw before we resize them so they fit where we want them to fit.

				if (TopDown)
					child.Rect.Y.Paramater = PixelSize.Y - originY - childSize;
				else
					child.Rect.Y.Paramater = originY;

				child.Rect.Y.RelativeTo = RelativeLoc.Edge.Raw;
				child.Rect.Height.Paramater = childSize;
				child.Rect.Height.Raw = true;

				child.Rect.AnchorLocation = OriginLocation.LowerLeft;

				child.Resize((int)PixelSize.X, (int)PixelSize.Y);

				originY += childSize + ChildSpacing;
			}
				
		}
	}
}
