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
	public class UIButton : GUIElement
	{
		public string DefaultTexture = string.Empty;

		public string ActiveTexture = string.Empty;
		public string DisabledTexture = string.Empty;
		public string HoverTexture = string.Empty;

		public UIButton(RelativeRect rect) : base(rect)
		{

		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, Color.White);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, Color.White);

			if (CurrentMaterial.DiffuseTexture == null)
				ShapeBuffer.FilledRect(this, Rect);
			else
				StrechedBuffer.Stretched(this, Rect);
		}
	}
}
