using System;
using System.Collections.Generic;
using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Geometry;
using LudicrousElectron.GUI.Text;
using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public class UILabel : SingleDrawGUIItem
	{
		public string Text = string.Empty;
		public int Font = -1;

		public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			int size = (int)(Rect.GetPixelSize().Y * 0.5f); // TODO pass in some kind of relative size

			// TODO, make sure the rect size is correct for the string

			var drawInfo = FontManager.DrawText(Font, size, Text);
			if (drawInfo == null)
				return;

			if (Mat == null)
				Mat = GUIManager.GetMaterial(drawInfo.CachedTexture, BaseColor);

			ShapeBuffer.TexturedRect(this, Rect);
		}
	}
}
