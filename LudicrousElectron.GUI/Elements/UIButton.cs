using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Drawing.Sprite;
using LudicrousElectron.GUI.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI.Elements
{
	public class UIButton : SingleDrawGUIItem
    {
		public string ActiveTexture = string.Empty;
		public string DisabledTexture = string.Empty;
		public string HoverTexture = string.Empty;

		public UIButton(RelativeRect rect) : base(rect)
		{
            FillMode = UIFillModes.StretchMiddle;
		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);

            if (CurrentMaterial.DiffuseTexture == null)
                ShapeBuffer.FilledRect(this, Rect);
            else
                HandleTexturedRect();
		}
	}
}
