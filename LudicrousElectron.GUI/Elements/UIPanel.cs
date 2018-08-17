using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Geometry;

using OpenTK;

namespace LudicrousElectron.GUI.Elements
{

	public class UIPanel : SingleDrawGUIItem
	{
        public UIFillModes FillMode = UIFillModes.Tilled;

        public UIPanel(RelativeRect rect, Color color) : base(rect,color)
		{

		}

		public UIPanel(RelativeRect rect, Color color, string texture) : base(rect, color, texture)
		{

		}

		public UIPanel(RelativeRect rect, string texture) : base(rect, Color.White, texture)
		{

		}

        public override void Resize(int x, int y)
        {
            base.Resize(x, y);

            CheckMaterial();

            if (CurrentMaterial.DiffuseTexture == null)
                ShapeBuffer.FilledRect(this, Rect);
            else
                HandleTexturedRect();
           
        }
    }
}
