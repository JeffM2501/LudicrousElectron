using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.GUI.Geometry;

namespace LudicrousElectron.GUI
{
    public class GridLayoutGroup : LayoutContainer
    {
        public int Rows = 0;
        public int Columns = 0;

        public GridLayoutGroup() : base(null) { }

        public GridLayoutGroup(RelativeRect rect) : base(rect)
        {
            IgnoreMouse = true;
        }

        public override void AddChild(GUIElement child)
        {
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
          //  var PixelSize = Rect.GetPixelSize();
        }
    }
}
