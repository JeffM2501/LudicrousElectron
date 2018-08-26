using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.GUI.Geometry;
using OpenTK;

namespace LudicrousElectron.GUI
{
    public class GridLayoutGroup : LayoutContainer
    {
        public int Rows = 1;
        public int Columns = 1;

		protected class ColSpanInfo
		{
			public int StartRow = 0;
			public int Lenght = 1;
		}
        protected List<ColSpanInfo> ColSpanRows = new List<ColSpanInfo>(); // the rows that span all

        public float CellPadding = 0;

        internal class GridLayoutInfo
        {
            public int Row = 0;
            public int Col = 0;
        }

        public GridLayoutGroup() : base(null) { }

        public GridLayoutGroup(RelativeRect rect) : base(rect)
        {
            IgnoreMouse = false;
        }

        public GridLayoutGroup(RelativeRect rect, int rows, int cols) : base(rect)
        {
            IgnoreMouse = false;
            Rows = rows;
            Columns = cols;
        }

        public override void AddChild(GUIElement child)
        {
            GridLayoutInfo info = new GridLayoutInfo();

            for(int i = 0; i < Children.Count; i++)
            {
				var colInfo = ColSpanRows.Find((x) => x.StartRow == info.Row);

				if (colInfo != null)
                    info.Row += colInfo.Lenght;
                else
                {
                    info.Col++;
                    if (info.Col >= Columns)
                    {
                        info.Col = 0;
                        info.Row++;
                    }
                }

                if (info.Row > Rows)
                    return;
            }
            child.LayoutTag = info;
            base.AddChild(child);

            if (Inited)
                Resize((int)LastParentSize.X, (int)LastParentSize.Y);
        }

        public bool SetColSpan(int row, int count = 1)
        {
            if (Children.Count != 0)
                return false; // can't change layout after items are added.

			if (ColSpanRows.Find((x) => x.StartRow == row) != null)
				return false;

			ColSpanInfo info = new ColSpanInfo();
			info.StartRow = row;
			info.Lenght = count;
            ColSpanRows.Add(info);

            return true;
        }

        protected Vector2 PixelSize = Vector2.Zero;
        protected Vector2 AvalableSize = Vector2.Zero;
        protected Vector2 CellSize = Vector2.Zero;
        protected Vector2 ColSpanSize = Vector2.Zero;

        // find the lower left of the cell
        protected Vector2 GetCellOrigin(int row, int col, int count = 1)
        {
			int startRow = row + count - 1;

			float y = CellPadding;
			y += (startRow * CellSize.Y) + (startRow * CellPadding);
          
            float x = CellPadding;
            x += (col * CellSize.X) + (col * CellPadding);

            return new Vector2(x, (PixelSize.Y - y) - CellSize.Y);
        }

        protected static Vector2 GetChildOrigin(Vector2 cellOrigin, Vector2 cellSize, float actualHeight, OriginLocation anchor)
        {
            float halfX = cellSize.X * 0.5f;
            float halfY = cellSize.Y * 0.5f;

            float cellY = cellOrigin.Y;
            float cellHeight = cellSize.Y;

            if (actualHeight < cellSize.Y)
            {
                cellY += ((cellSize.Y - actualHeight) * 0.5f);
                halfY = actualHeight * 0.5f;
                cellHeight = actualHeight;
            }

            switch(anchor)
            {
                case OriginLocation.Center:
                    return new Vector2(cellOrigin.X + halfX, cellY + halfY);
	
		        case OriginLocation.MiddleLeft:
                    return new Vector2(cellOrigin.X, cellY + halfY);

                case OriginLocation.UpperLeft:
                    return new Vector2(cellOrigin.X, cellY + cellHeight);

                case OriginLocation.UpperCenter:
                    return new Vector2(cellOrigin.X + halfX, cellY + cellHeight);

                case OriginLocation.UpperRight:
                    return new Vector2(cellOrigin.X + cellSize.X, cellY + cellHeight);

                case OriginLocation.MiddleRight:
                    return new Vector2(cellOrigin.X + cellSize.X, cellY + halfY);

                case OriginLocation.LowerRight:
                    return new Vector2(cellOrigin.X + cellSize.X, cellY);

                case OriginLocation.LowerCenter:
                    return new Vector2(cellOrigin.X + halfX, cellY);

                default:
                    return cellOrigin;
            }
        }

        public override void Resize(int x, int y)
        {
            LastParentSize.X = x;
            LastParentSize.Y = y;

            Inited = true;

            Rect.Resize(x, y);

            if (Columns == 0 || Rows == 0)
                return; // must have at least one of each

            PixelSize = Rect.GetPixelSize();

            AvalableSize.X = PixelSize.X - (CellPadding * (Columns + 1));
            AvalableSize.Y = PixelSize.Y - (CellPadding * (Rows + 1));

            CellSize = new Vector2(AvalableSize.X / Columns, AvalableSize.Y / Rows);
            ColSpanSize = new Vector2(PixelSize.X - (CellPadding * 2), CellSize.Y);

            if (CellSize.X < 2 || CellSize.Y < 2)
                return; // there isn't enough room to do anything

            foreach (var child in Children)
            {
                GridLayoutInfo info = child.LayoutTag as GridLayoutInfo;
                if (info == null)
                    continue;

				Vector2 thisCellSize = CellSize;

				var colInfo = ColSpanRows.Find((i) => i.StartRow == info.Row);
				if (colInfo != null)
				{
					thisCellSize.X = ColSpanSize.X;
					thisCellSize.Y = (ColSpanSize.Y * colInfo.Lenght) + (CellPadding * (colInfo.Lenght - 1));
				}

                OriginLocation location = child.Rect.AnchorLocation;
                float itemHeight = thisCellSize.Y;

				if ((colInfo == null || colInfo.Lenght == 1) && (MaxChildSize > 0 && thisCellSize.Y > MaxChildSize)) // if the cell is larger than one high, they can't clamp it
				{
					itemHeight = MaxChildSize;
					location = OriginTools.GetMiddleAnhcor(location);
				}

                // compute where the child's origin needs to be for it's anchor relative to the cell
                Vector2 childOrigin = GetChildOrigin(GetCellOrigin(info.Row, colInfo != null ? 0 : info.Col, colInfo != null ? colInfo.Lenght : 1), thisCellSize, itemHeight, location);

                child.Rect.X.RelativeTo = RelativeLoc.Edge.Raw;
                child.Rect.X.Paramater = childOrigin.X;

                child.Rect.Y.RelativeTo = RelativeLoc.Edge.Raw;
                child.Rect.Y.Paramater = childOrigin.Y;

                child.Rect.Width.Mode  = RelativeSize.SizeModes.Raw;
                child.Rect.Width.Paramater = thisCellSize.X;

                child.Rect.Height.Mode = RelativeSize.SizeModes.Raw;
                child.Rect.Height.Paramater = itemHeight;

                // have the child size itself and it's children to it's new location
                child.Resize(PixelSize);
            }
        }
    }
}
