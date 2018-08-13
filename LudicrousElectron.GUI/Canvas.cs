using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.GUI
{
	public class Canvas
	{
		public List<GUIElement> GUIElements = new List<GUIElement>();

		public virtual void Render(GUIRenderLayer layer)
		{
			foreach (var element in GUIElements)
				element.Draw(layer);
		}

		public virtual void Resize(WindowManager.Window window)
		{
			foreach (var element in GUIElements)
				element.Resize(window.Width,window.Height);
		}
	}
}
