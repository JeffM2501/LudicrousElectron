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
		public UIPanel(RelativeRect rect, Color color) : base(rect,color)
		{

		}

		public UIPanel(RelativeRect rect, Color color, string texture) : base(rect, color, texture)
		{

		}

		public UIPanel(RelativeRect rect, string texture) : base(rect, Color.White, texture)
		{

		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

        public override void Resize(int x, int y)
        {
            base.Resize(x, y);

			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			if (Mat.DiffuseTexture == null)
				ShapeBuffer.FilledRect(this, Rect);
			else
				ShapeBuffer.TexturedRect(this, Rect, Mat.DiffuseTexture);
        }
    }
}
