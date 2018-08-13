using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.Engine.Graphics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.GUI
{
	public interface IGUIElement
	{
		void Draw(GUIRenderLayer layer);
		void Resize(WindowManager.Window window);
	}

	public abstract class SingleDrawGUIItem : PrimitivBuffer, IGUIElement
	{
		public Color BaseColor = Color.White;
		public string Texture = string.Empty;

		public virtual void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public abstract void Resize(WindowManager.Window window);
	}
}
