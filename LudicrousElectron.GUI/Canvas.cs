using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.GUI.Elements;
using LudicrousElectron.Engine.Input;
using OpenTK;

namespace LudicrousElectron.GUI
{
	public class Canvas
	{
		public int BoundWindowID = -1;
        public WindowManager.Window BoundWindow { get { return WindowManager.GetWindow(BoundWindowID); } }

		public float LayerDepthShift = 1.0f;

        protected SortedDictionary<int,List<GUIElement>> GUIElements = new SortedDictionary<int,List<GUIElement>>();

		protected GUIElement PopUpCTL = null;

		protected List<UIButton> HoveredControlls = new List<UIButton>();
		protected List<UIButton> ActivatedControlls = new List<UIButton>();

		public void AddElement(GUIElement element, int layer = 0)
        {
			if (!GUIElements.ContainsKey(layer))
				GUIElements.Add(layer, new List<GUIElement>());

			element.SetParentCanvas(this);

			GUIElements[layer].Add(element);
            if (BoundWindow != null)
                element.Resize(BoundWindow.Width, BoundWindow.Height);
        }

        public bool PopupEnabled()
        {
            return PopUpCTL != null;
        }

		public void SetPopupElement(GUIElement element)
		{
			if (PopUpCTL != null)
				PopUpCTL = null;	// disable it here? it may need to flush a list?

			PopUpCTL = element;
			if (element != null)
			{
				element.ParentCanvas = this;

				if (BoundWindow != null)
					element.Resize(BoundWindow.Width, BoundWindow.Height);
			}
		}

		public virtual void Render(GUIRenderLayer layer)
		{
			int lastLayer = 1;
			foreach (var l in GUIElements)
			{
				lastLayer = l.Key;
				layer.PushTranslation(0, 0, l.Key * LayerDepthShift);
				foreach (var element in l.Value)
					element.Render(layer);

				layer.PopMatrix();
			}

            lastLayer *= 2;
			layer.PushTranslation(0, 0, lastLayer * LayerDepthShift);
			PopUpCTL?.Render(layer);
			layer.PopMatrix();
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

			PopUpCTL?.Resize(BoundWindow.Width, BoundWindow.Height);
		}

		List<UIButton> NewHover = new List<UIButton>();
		List<UIButton> NewActive = new List<UIButton>();

		protected virtual bool HandleControlClick(UIButton button, InputManager.LogicalButtonState mouseButtons)
		{
			if (button == null || !button.IsEnabled())
				return false;

			if (mouseButtons.PrimaryClick || mouseButtons.PrimaryDown)
			{
				if (button.IsHovered())
					button.EndHover();

				if (!button.IsActive())
					button.Activate();

				if (mouseButtons.PrimaryClick)
					button.Click();
				NewActive.Add(button);
			}
			else
			{
				if (!button.IsHovered())
					button.StartHover();

				NewHover.Add(button);
			}

			return true;
		}

		public virtual bool MouseEvent(Vector2 position, InputManager.LogicalButtonState buttons)
		{
			List<GUIElement> affectedElements = new List<GUIElement>();

			// check the popup element for clicks
			if (PopUpCTL != null)
			{
				if (PopUpCTL.Rect.PointInRect(position))
					affectedElements.AddRange(PopUpCTL.GetElementsUnderPoint(position, buttons));
				else if (buttons.AnyButtonIsDown())
					SetPopupElement(null);
			}

			if (PopUpCTL == null) // only check the other controls if there is not a popup.
			{
				List<int> keys = new List<int>(GUIElements.Keys);
				keys.Reverse();

				foreach (var layer in keys)
				{
					foreach (var item in GUIElements[layer])
						affectedElements.AddRange(item.GetElementsUnderPoint(position, buttons));
				}
			}

			if (affectedElements.Count == 0) // nothing is affected, clear out the states
			{
				foreach (var item in ActivatedControlls)
				{
					if (item.IsActive())
						item.Deactivate();
				}
				ActivatedControlls.Clear();

				foreach (var item in HoveredControlls)
				{
					if (item.IsHovered())
						item.EndHover();
				}
				HoveredControlls.Clear();
				return false;
			}

			NewHover = new List<UIButton>();
			NewActive = new List<UIButton>();

			foreach (var element in affectedElements)
			{
				UIButton button = element as UIButton;
                HandleControlClick(button, buttons);
			}

			foreach(var item in ActivatedControlls)
			{
				if (!NewActive.Contains(item) && item.IsActive())
					item.Deactivate();
			}
			ActivatedControlls = NewActive;

			foreach(var item in HoveredControlls)
			{
				if (!NewHover.Contains(item) && item.IsHovered())
					item.EndHover();
			}
			HoveredControlls = NewHover;

			NewHover = null;
			NewActive = null;

			return true;
		}
	}
}
