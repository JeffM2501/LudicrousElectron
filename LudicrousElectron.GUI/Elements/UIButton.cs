
using System;
using System.Collections.Generic;
using System.Drawing;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Drawing.Sprite;
using LudicrousElectron.GUI.Geometry;
using LudicrousElectron.GUI.Text;

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

            LabelControl = new UILabel(Font, LabelText, RelativePoint.Center, RelativeSize.FullHeight + (0.3f));
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

        public virtual void Enable()
        {
            Enabled = true;
            FlushMaterial();
            ButtonEnabled?.Invoke(this, this);
        }

        public virtual void Disable()
        {
            Enabled = false;
            FlushMaterial();
            ButtonDisabled?.Invoke(this, this);
        }

        public virtual void Activate()
        {
            Activated = true;
            FlushMaterial();
            ButtonAcivated?.Invoke(this, this);
        }

        public virtual void Deactivate()
        {
            Activated = false;
            FlushMaterial();
            ButtonDeactivated?.Invoke(this, this);
        }

        public virtual void StartHover()
        {
            Hovered = false;
            FlushMaterial();
            ButtonStartHover?.Invoke(this, this);
        }

        public virtual void EndHover()
        {
            Hovered = false;
            FlushMaterial();
            ButtonEndHover?.Invoke(this, this);
        }
    }
}
