using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.GUI.Drawing;

using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public enum ImageTextureMode
	{
		None,
		Tiled,
		Centered
	}

	public class Overlay : SingleDrawGUIItem
	{
		public ImageTextureMode ImageMode = ImageTextureMode.None;

		public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public override void Resize(WindowManager.Window window)
		{
			if (Texture == string.Empty || ImageMode == ImageTextureMode.None)
				ShapeBuffer.FilledRect(this, new Vector2(window.Width * -0.5f, window.Height * -0.5f), new Vector2(window.Width, window.Height));
			else
			{
				if (ImageMode == ImageTextureMode.Centered)
				{

				}
				ShapeBuffer.TexturedRect(this, Vector2.Zero, new Vector2(window.Width, window.Height));
			}

		}
	}
}
