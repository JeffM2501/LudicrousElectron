using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.GUI
{
	public class Canvas
	{
        public WindowManager.Window BoundWindow = null;

        protected List<GUIElement> GUIElements = new List<GUIElement>();

        public void AddElement(GUIElement element)
        {
            GUIElements.Add(element);
            if (BoundWindow != null)
                element.Resize(BoundWindow.Width, BoundWindow.Height);
        }

		public virtual void Render(GUIRenderLayer layer)
		{
			foreach (var element in GUIElements)
				element.Draw(layer);
		}

		public virtual void Resize()
		{
            if (BoundWindow == null)
                return;

			foreach (var element in GUIElements)
				element.Resize(BoundWindow.Width, BoundWindow.Height);
		}
	}
}
