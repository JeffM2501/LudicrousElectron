
using System;
using System.Collections.Generic;
using System.Drawing;
using LudicrousElectron.Engine.Audio;
using LudicrousElectron.Engine.Input;
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
        public GUIMaterial ActiveMaterial = null;
		public GUIMaterial DisabledMaterial = null;
        public GUIMaterial HoverMaterial = null;
        public GUIMaterial CheckedMaterial = null;

        protected string LabelText = string.Empty;
        protected int Font = -1;

        public Color DefaultTextColor = Color.White;
        public Color ActiveTextColor = Color.WhiteSmoke;
        public Color DisabledTextColor = Color.DimGray;
        public Color HoverTextColor = Color.OldLace;
        public Color CheckedTextColor = Color.Transparent;

        protected UILabel LabelControl = null;

        protected bool Enabled = true;
        protected bool Hovered = false;
        protected bool Activated = false;

        protected bool Checked = false;

        public event EventHandler<UIButton> Clicked = null;

        public event EventHandler<UIButton> ButtonEnabled = null;
        public event EventHandler<UIButton> ButtonDisabled = null;
        public event EventHandler<UIButton> ButtonAcivated = null;
        public event EventHandler<UIButton> ButtonDeactivated = null;
        public event EventHandler<UIButton> ButtonStartHover = null;
        public event EventHandler<UIButton> ButtonEndHover = null;

        public event EventHandler<UIButton> ButtonCheckChanged = null;

        public string ClickSound = string.Empty;
		public string HoverSound = string.Empty;

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

        public override void FlushMaterials(bool children = false)
        {
            base.FlushMaterials(children);
            if (LabelControl != null)
            {
                LabelControl.DefaultMaterial.Color = GetCurrentTextColor();
                LabelControl.FlushMaterials(children);
            }
        }

        public override GUIMaterial GetCurrentMaterial()
        {
            if (!Enabled)
                return DisabledMaterial == null ? DefaultMaterial : DisabledMaterial;

            if (Checked)
                 return CheckedMaterial == null ? (ActiveMaterial == null ? DefaultMaterial : ActiveMaterial) : CheckedMaterial;
            if (Activated)
                return ActiveMaterial == null ? DefaultMaterial : ActiveMaterial;

            if (Hovered)
                return HoverMaterial == null ? DefaultMaterial : HoverMaterial;

            return DefaultMaterial;
        }

        protected virtual Color GetCurrentTextColor()
        {
            if (!Enabled)
                return DisabledTextColor;

            if (Checked)
                return CheckedTextColor != Color.Transparent ? CheckedTextColor : ActiveTextColor;

            if (Activated)
                return ActiveTextColor;

            if (Hovered)
                return HoverTextColor;

            return DefaultTextColor;
        }

        public override void Resize(int x, int y)
		{
			if (!string.IsNullOrEmpty(LabelText) && LabelControl == null)
				SetupLabel();

			base.Resize(x, y);

            CheckMaterial();

            if (CurrentMaterial.DiffuseTexture == null)
                ShapeBuffer.FilledRect(this, Rect);
            else
                HandleTexturedRect();
		}

		public override List<GUIElement> GetElementsUnderPoint(Vector2 location, InputManager.LogicalButtonState buttons)
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

					List<GUIElement> childElements = child.GetElementsUnderPoint(childLoc, buttons);
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
            FlushMaterials();
            ButtonEnabled?.Invoke(this, this);
        }

        public virtual void Disable()
        {
			if (!Enabled)
				return;

			Enabled = false;
            FlushMaterials();
            ButtonDisabled?.Invoke(this, this);
        }

		public virtual bool IsEnabled() { return Enabled;  }

		public virtual void Activate()
        {
			if (Activated)
				return;
			Activated = true;
            FlushMaterials();
            ButtonAcivated?.Invoke(this, this);
        }

        public virtual void Deactivate()
        {
			if (!Activated)
				return;

			Activated = false;
            FlushMaterials();
            ButtonDeactivated?.Invoke(this, this);
        }

		public virtual bool IsActive() { return Activated; }

		public virtual void StartHover()
        {
			if (Hovered)
				return;

			Hovered = true;
            FlushMaterials();
            ButtonStartHover?.Invoke(this, this);

			if (!string.IsNullOrEmpty(HoverSound))
				SoundManager.PlaySound(HoverSound);
		}

        public virtual void EndHover()
        {
			if (!Hovered)
				return;

			Hovered = false;
            FlushMaterials();
            ButtonEndHover?.Invoke(this, this);
        }

        public virtual void Check()
        {
            if (Checked)
                return;
            Checked = true;

            FlushMaterials();
            ButtonCheckChanged?.Invoke(this, this);
        }

        public virtual void UnCheck()
        {
            if (!Checked)
                return;
            Checked = false;
            FlushMaterials();
            ButtonCheckChanged?.Invoke(this, this);
        }

        public virtual bool IsChecked()
        {
            return Checked;
        }

        public virtual bool IsHovered() { return Hovered; }

		public virtual void Click()
		{
			Clicked?.Invoke(this, this);

			if (!string.IsNullOrEmpty(ClickSound))
				SoundManager.PlaySound(ClickSound);
		}
    }
}
