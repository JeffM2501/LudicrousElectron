using System;
using System.Collections.Generic;
using System.Drawing;
using LudicrousElectron.Engine;
using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Geometry;
using LudicrousElectron.GUI.Text;
using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public class UILabel : SingleDrawGUIItem
	{
		public string Text = string.Empty;
		public int Font = -1;

        public enum TextFittingModes
        {
            ByHeightExtend,
            ByHeightTrim,
            ByWidth,
        }

        protected TextFittingModes FittingMode = TextFittingModes.ByHeightExtend;

        public TextFittingModes GetFittingMode()
        {
            return FittingMode;
        }

        public void SetFittingMode(TextFittingModes mode)
        {
            FittingMode = mode;
            SetDirty();
        }

        public UILabel() { }

        public UILabel(int font, string text, RelativeRect rect, TextFittingModes mode = TextFittingModes.ByHeightExtend) : base(rect, Color.White)
        {
            FittingMode = mode;
            Text = text;
            Font = font;
        }

        public UILabel(int font, string text, RelativeRect rect, Color color, TextFittingModes mode = TextFittingModes.ByHeightExtend) : base(rect, color)
        {
            FittingMode = mode;
            Text = text;
            Font = font;
        }

        public UILabel(int font, string text, RelativePoint origin, RelativeSize height, RelativeSize width = null, OriginLocation anchor = OriginLocation.Center, TextFittingModes mode = TextFittingModes.ByHeightExtend) : base()
        {
            FittingMode = mode;
            Text = text;
            Font = font;

            if (width == null)
                width = RelativeSize.FullWidth;

            BaseColor = Color.White;
            Rect = new RelativeRect(origin.X, origin.Y, width, height, anchor);
        }

        public UILabel(int font, string text, Color color, RelativePoint origin, RelativeSize height, RelativeSize width = null, OriginLocation anchor = OriginLocation.Center, TextFittingModes mode = TextFittingModes.ByHeightExtend) : base()
        {
            FittingMode = mode;
            Text = text;
            Font = font;

            if (width == null)
                width = RelativeSize.FullWidth;

            BaseColor = color;
            Rect = new RelativeRect(origin.X, origin.Y, width,  height, anchor);
        }

        public UILabel(int font, string text, TextFittingModes mode = TextFittingModes.ByHeightExtend) : base()
        {
            FittingMode = mode;
            Text = text;
            Font = font;
            BaseColor = Color.White;
            Rect = new RelativeRect(RelativeLoc.XCenter,RelativeLoc.YCenter, RelativeSize.FullWidth, RelativeSize.FullWidth, OriginLocation.Center);
        }

        public UILabel(int font, string text, Color color, TextFittingModes mode = TextFittingModes.ByHeightExtend) : base()
        {
            FittingMode = mode;
            Text = text;
            Font = font;
            BaseColor = color;
            Rect = new RelativeRect(RelativeLoc.XCenter, RelativeLoc.YCenter, RelativeSize.FullWidth, RelativeSize.FullWidth, OriginLocation.Center);
        }

        public override void Draw(GUIRenderLayer layer)
		{
			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
            RelativeSize heightCache = Rect.Height;
            RelativeSize widthCache = Rect.Width;

            FontManager.FontDrawInfo drawInfo = null;

            float pixelHeight = heightCache.ToScreen(x, y) * 0.75f;
            float pixelWidth = widthCache.ToScreen(x, y);
            int fontHeight = (int)(pixelHeight + 1);
            float textWidth = 0;

            string effectiveText = Text;

            switch (FittingMode)
            {
                case TextFittingModes.ByHeightExtend:   
                    break;

                case TextFittingModes.ByHeightTrim:
                    effectiveText = String.Copy(Text);

                    textWidth = FontManager.MeasureText(Font, fontHeight, effectiveText).X;

                    while(textWidth > pixelWidth)
                    {
                        if (textWidth > pixelWidth * 2)
                            effectiveText = effectiveText.Substring(0, effectiveText.Length / 2);
                        else
                            effectiveText = effectiveText.Substring(0, effectiveText.Length - 1);

                        textWidth = FontManager.MeasureText(Font, fontHeight, effectiveText).X;
                    }
                    break;

                case TextFittingModes.ByWidth:
                    textWidth = FontManager.MeasureText(Font, fontHeight, Text).X;

                    while (textWidth > pixelWidth)
                    {
                        fontHeight -= 1;
                        if (fontHeight <= 5) // that's the smallest we can go, it just wont' fit
                            break;

                        textWidth = FontManager.MeasureText(Font, fontHeight, effectiveText).X;
                    }
                    break;

            }

            drawInfo = FontManager.DrawText(Font, fontHeight, effectiveText);
            if (drawInfo == null)
                return;

            Rect.Width = new RelativeSize(drawInfo.Size.X, true);
            Rect.Width.Raw = true;

            Rect.Height = new RelativeSize(drawInfo.Size.Y, true);
            Rect.Height.Raw = true;

            Rect.Resize(x, y);
            var pixelSize = Rect.GetPixelSize();

            if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture != drawInfo.CachedTexture)
                CurrentMaterial = GUIManager.GetMaterial(drawInfo.CachedTexture, BaseColor);
            ShapeBuffer.TexturedRect(this, Rect);

            foreach (var child in Children)
            {
                child.Resize((int)pixelSize.X, (int)pixelSize.Y);
            }

            Rect.Height = heightCache;
            Rect.Width = widthCache;
        }
	}
}
