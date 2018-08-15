using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;

using OpenTK;

namespace LudicrousElectron.GUI
{
	public class Canvas
	{
        public WindowManager.Window BoundWindow = null;

		public float LayerDepthShift = 1.0f;

        protected Dictionary<int,List<GUIElement>> GUIElements = new Dictionary<int,List<GUIElement>>();

		public void AddElement(GUIElement element, int layer = 0)
        {
			if (!GUIElements.ContainsKey(layer))
				GUIElements.Add(layer, new List<GUIElement>());

			GUIElements[layer].Add(element);
            if (BoundWindow != null)
                element.Resize(BoundWindow.Width, BoundWindow.Height);
        }

		public virtual void Render(GUIRenderLayer layer)
		{
			foreach (var l in GUIElements)
			{
				layer.PushTranslation(0, 0, l.Key * LayerDepthShift);
				foreach (var element in l.Value)
					element.Render(layer);

				layer.PopMatrix();
			}
		}

		public virtual void Resize()
		{
			if (BoundWindow == null)
				return;
			foreach (var l in GUIElements)
			{
				foreach (var element in l.Value)
					element.Resize(BoundWindow.Width, BoundWindow.Height);
			}
		}
	}
}
