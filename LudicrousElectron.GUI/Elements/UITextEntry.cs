﻿using System;
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
    public class UITextEntry : SingleDrawGUIItem
    {
        public static double CursorBlinkRate = 0.5;

        public GUIMaterial FocusedMaterial = null;
        public Color DefaultTextColor = Color.White;
        public Color FocusedTextColor = Color.White;

        public char CursorChar = '_';
        public Color CursorColor = Color.Transparent;

        protected bool DrawCursor = true;
        protected int CursorAnimationTimer = -1;

        protected string CurrentText = string.Empty;
        protected int Font = -1;

        protected UILabel TextLabel = null;

        protected Vector2 CursorPostion = new Vector2(0, 0);
        protected UIImage CursorImage = null;

        protected bool Focused = false;

        public event EventHandler<UITextEntry> GotFocus = null;
        public event EventHandler<UITextEntry> LostFocus = null;
        public event EventHandler<UITextEntry> TextChanged = null;
        public event EventHandler<UITextEntry> EnterPressed = null;

        public string FocusSound = string.Empty;
        public string TypeSound = string.Empty;

        public UITextEntry(RelativeRect rect) : base(rect)
        {
            FillMode = UIFillModes.StretchMiddle;
            IgnoreMouse = false;
            Setup();
        }

        public UITextEntry(RelativeRect rect, string initalText) : base(rect)
        {
            FillMode = UIFillModes.StretchMiddle;
            SetCurrentText(initalText);
            IgnoreMouse = false;
            Setup();
        }

        ~UITextEntry()
        {
            if (CursorAnimationTimer < 0)
                Engine.Core.State.RemoveTimer(CursorAnimationTimer);
        }

        protected virtual void Setup()
        {

        }

        public virtual void SetCurrentText(string text)
        {
            CurrentText = text;
            if (TextLabel == null)
                SetupLabel();
            else
            {
                TextLabel.Text = text;
                SetDirty();
            }
        }

        public string GetCurrentText()
        {
            return CurrentText;
        }

        protected virtual void SetupLabel()
        {
            if (!WindowManager.Inited() || !Inited)
                return;

            if (Font == -1)
                Font = FontManager.DefaultFont;

            TextLabel = new UILabel(Font, GetCurrentText(), RelativePoint.MiddleLeft, RelativeSize.FullHeight + (0.35f), null, OriginLocation.MiddleLeft,UILabel.TextFittingModes.ByHeightReverseTrim);
			TextLabel.WriteDepth = false;

			TextLabel.DefaultMaterial.Color = GetCurrentTextColor();
            AddChild(TextLabel);

            var thisSize = Rect.GetPixelSize();
            TextLabel.Resize(thisSize);

			SetupCursor();

			CursorPostion.Y = 0;
			SetCursorPostion();
		}

		protected void SetupCursor()
		{
			if(CursorChar == char.MinValue)
				CursorImage = null;
            else 
            {
				int fontSize = (int)(Rect.GetPixelSize().Y * 0.75);
				if (TextLabel != null)
					fontSize = TextLabel.ActualFontSize;

				var cursor = FontManager.DrawText(Font, fontSize, CursorChar.ToString());

				CursorImage = new UIImage(new RelativeRect(RelativePoint.LowerLeft, new RelativeSizeXY(cursor.Size), OriginLocation.LowerCenter), cursor.CachedTexture.RelativeName);
				CursorImage.WriteDepth = false;

				Color color = CursorColor;
				if (color == Color.Transparent)
					color = FocusedTextColor;
				if (color == Color.Transparent)
					color = DefaultTextColor;

				CursorImage.CurrentMaterial = GUIManager.GetMaterial(cursor.CachedTexture, color);
				CursorImage.Resize(LastParentSize);
			}
		}

        public override void FlushMaterials(bool children = false)
        {
            base.FlushMaterials(children);
            if (TextLabel != null)
            {
                TextLabel.DefaultMaterial.Color = GetCurrentTextColor();
                TextLabel.FlushMaterials(children);
            }
        }

        public override GUIMaterial GetCurrentMaterial()
        {
            if (Focused)
                return FocusedMaterial == null ? DefaultMaterial : FocusedMaterial;

            return DefaultMaterial;
        }

        protected virtual Color GetCurrentTextColor()
        {
            if (Focused)
                return FocusedTextColor;

            return DefaultTextColor;
        }

		protected void SetCursorPostion()
		{
			if (CursorImage == null)
				SetupCursor();

			if (GetCurrentText() == string.Empty)
				CursorPostion.X = CursorImage.Rect.GetPixelSize().X * 0.5f;
			else
				CursorPostion.X = TextLabel.Rect.GetPixelSize().X;
		}

        internal void ChangeText(string newText)
        {
           bool hadEnter = false;
            if (CurrentText.Length < 0 && CurrentText.Length < newText.Length)
            {
                if (newText.Substring(CurrentText.Length - 1).Contains("\r"))
                    hadEnter = true;
            }
            else
                hadEnter = newText.Contains("\r");

			bool wasEmpty = CurrentText == string.Empty;
            CurrentText = newText;

            if (TextLabel != null)
            {
				TextLabel.Text = GetCurrentText();
				TextLabel.ForceRefresh();
				SetCursorPostion();
			}

			if (wasEmpty)
				SetupCursor();

			TextChanged?.Invoke(this, this);
            if (hadEnter)
                EnterPressed?.Invoke(this, this);
        }

        public override void Resize(int x, int y)
        {
            base.Resize(x, y);

            if (TextLabel == null)
                SetupLabel();

            CheckMaterial();

            if (CurrentMaterial.DiffuseTexture == null)
                ShapeBuffer.FilledRect(this, Rect);
            else
                HandleTexturedRect();

			SetCursorPostion();
		}

        public override void Draw(GUIRenderLayer layer)
        {
            base.Draw(layer);

            if (DrawCursor && Focused && CursorImage != null)
            {
				Vector2 origin = Rect.GetPixelOrigin();

                layer.PushTranslation(origin.X + CursorPostion.X, origin.Y + CursorPostion.Y, 10);
                layer.AddDrawable(CursorImage);
                layer.PopMatrix();
            }
        }

        public override List<GUIElement> GetElementsUnderPoint(Vector2 location, InputManager.LogicalButtonState buttons)
        {
            List<GUIElement> elements = new List<GUIElement>();

            if (Inited)
            {
                if (Rect.PointInRect(location))
                {
                    ProcessMouseEvent(location, buttons);
                    elements.Add(this);
                }

                Vector2 childLoc = location - Rect.GetPixelOrigin();
                foreach (var child in Children)
                {
                    if (child == TextLabel)
                        continue;

                    List<GUIElement> childElements = child.GetElementsUnderPoint(childLoc, buttons);
                    if (childElements.Count > 0)
                        elements.AddRange(childElements.ToArray());
                }
            }

            return elements;
        }

        public virtual void SetFocus()
        {
            if (Focused)
                return;

            Focused = true;
            DrawCursor = true;
            FlushMaterials();
            GotFocus?.Invoke(this, this);

            if (!string.IsNullOrEmpty(FocusSound))
                SoundManager.PlaySound(FocusSound);

            if (CursorAnimationTimer < 0)
                CursorAnimationTimer = Engine.Core.State.AddTimer(CursorBlinkRate, true, ToggleCursorDraw);
            else
                Engine.Core.State.ResumeTimer(CursorAnimationTimer);
        }

        public virtual void ClearFocus()
        {
            if (!Focused)
                return;

            Focused = false;
            FlushMaterials();
            LostFocus?.Invoke(this, this);

            if (CursorAnimationTimer < 0)
                Engine.Core.State.PauseTimer(CursorAnimationTimer);
        }

        internal void ToggleCursorDraw(object sender, Engine.Core.EngineState state)
        {
            DrawCursor = !DrawCursor;
        }

        public override void ProcessMouseEvent(Vector2 location, InputManager.LogicalButtonState buttons)
        {
            if (!buttons.PrimaryClick)
                return;

            ParentCanvas.SetFocusedTextArea(this);
        }
    }
}
