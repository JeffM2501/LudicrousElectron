
using System;
using System.Collections.Generic;
using System.Drawing;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Drawing.Sprite;
using LudicrousElectron.GUI.Geometry;
using LudicrousElectron.GUI.Text;
using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public class UIButton : SingleDrawGUIItem
    {
        public GUIMaterial Active = null;
		public GUIMaterial Disabled = null;
        public GUIMaterial Hover = null;

        protected string LabelText = string.Empty;
        protected int Font = -1;

        public Color DefaultTextColor = Color.White;
        public Color ActiveTextColor = Color.WhiteSmoke;
        public Color DisabledTextColor = Color.DimGray;
        public Color HoverTextColor = Color.OldLace;

        protected UILabel LabelControl = null;

        protected bool Enabled = true;
        protected bool Hovered = false;
        protected bool Activated = false;

        public event EventHandler<UIButton> Clicked = null;

        public event EventHandler<UIButton> ButtonEnabled = null;
        public event EventHandler<UIButton> ButtonDisabled = null;
        public event EventHandler<UIButton> ButtonAcivated = null;
        public event EventHandler<UIButton> ButtonDeactivated = null;
        public event EventHandler<UIButton> ButtonStartHover = null;
        public event EventHandler<UIButton> ButtonEndHover = null;

        public UIButton(RelativeRect rect) : base(rect)
		{
            FillMode = UIFillModes.StretchMiddle;
            Setup();
		}

        public UIButton(RelativeRect rect, string text) : base(rect)
        {
            FillMode = UIFillModes.StretchMiddle;
            SetText(text);
            Setup();
        }

        protected virtual void Setup()
        {

        }

        public virtual void SetText(string text)
        {
            LabelText = text;
            if (LabelControl == null)
                SetupLabel();
            else
            {
                LabelControl.Text = text;
                SetDirty();
            }
        }

        public string GetText()
        {
            return LabelText;
        }

        protected virtual void SetupLabel()
        {
			if (!WindowManager.Inited())
				return;

            if (Font == -1)
                Font = FontManager.DefaultFont;

            LabelControl = new UILabel(Font, LabelText, RelativePoint.Center, RelativeSize.FullHeight + (0.35f));
            LabelControl.DefaultMaterial.Color = GetCurrentTextColor();
            AddChild(LabelControl);
        }

        public override void FlushMaterial()
        {
            base.FlushMaterial();
            if (LabelControl != null)
            {
                LabelControl.DefaultMaterial.Color = GetCurrentTextColor();
                LabelControl.FlushMaterial();
            }
        }

        public override GUIMaterial GetCurrentMaterial()
        {
            if (!Enabled)
                return Disabled == null ? DefaultMaterial : Disabled;

            if (Activated)
                return Active == null ? DefaultMaterial : Active;

            if (Hovered)
                return Hover == null ? DefaultMaterial : Hover;

            return DefaultMaterial;
        }

        protected virtual Color GetCurrentTextColor()
        {
            if (!Enabled)
                return DisabledTextColor;

            if (Activated)
                return ActiveTextColor;

            if (Hovered)
                return HoverTextColor;

            return DefaultTextColor;
        }

        public override void Resize(int x, int y)
		{
			if (LabelText != string.Empty && LabelControl == null)
				SetupLabel();

			base.Resize(x, y);

            CheckMaterial();

            if (CurrentMaterial.DiffuseTexture == null)
                ShapeBuffer.FilledRect(this, Rect);
            else
                HandleTexturedRect();
		}

		public override List<GUIElement> GetElementsUnderPoint(Vector2 location)
		{
			List<GUIElement> elements = new List<GUIElement>();

			if (Inited)
			{
				if (Rect.PointInRect(location))
					elements.Add(this);

				Vector2 childLoc = location - Rect.GetPixelOrigin();
				foreach (var child in Children)
				{
					if (child == LabelControl)
						continue;

					List<GUIElement> childElements = child.GetElementsUnderPoint(childLoc);
					if (childElements.Count > 0)
						elements.AddRange(childElements.ToArray());
				}
			}

			return elements;
		}

		public virtual void Enable()
        {
			if (Enabled)
				return;

            Enabled = true;
            FlushMaterial();
            ButtonEnabled?.Invoke(this, this);
        }

        public virtual void Disable()
        {
			if (!Enabled)
				return;

			Enabled = false;
            FlushMaterial();
            ButtonDisabled?.Invoke(this, this);
        }

		public virtual bool IsEnabled() { return Enabled;  }

		public virtual void Activate()
        {
			if (Activated)
				return;
			Activated = true;
            FlushMaterial();
            ButtonAcivated?.Invoke(this, this);
        }

        public virtual void Deactivate()
        {
			if (!Activated)
				return;

			Activated = false;
            FlushMaterial();
            ButtonDeactivated?.Invoke(this, this);
        }

		public virtual bool IsActive() { return Activated; }

		public virtual void StartHover()
        {
			if (Hovered)
				return;

			Hovered = true;
            FlushMaterial();
            ButtonStartHover?.Invoke(this, this);
        }

        public virtual void EndHover()
        {
			if (!Hovered)
				return;

			Hovered = false;
            FlushMaterial();
            ButtonEndHover?.Invoke(this, this);
        }

		public virtual bool IsHovered() { return Hovered; }

		public virtual void Click()
		{
			Clicked?.Invoke(this, this);
		}
    }
}
