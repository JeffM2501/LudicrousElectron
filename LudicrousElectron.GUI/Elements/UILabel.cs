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

        public UILabel() { }

        public UILabel(int font, string text, RelativeRect rect) : base(rect, Color.White)
        {
            Text = text;
            Font = font;
        }

        public UILabel(int font, string text, RelativeRect rect, Color color) : base(rect, color)
        {
            Text = text;
            Font = font;
        }

        public UILabel(int font, string text, RelativePoint origin, RelativeSize height, RelativeSize width = null, OriginLocation anchor = OriginLocation.Center) : base()
        {
            Text = text;
            Font = font;

            if (width == null)
                width = RelativeSize.FullWidth;

            BaseColor = Color.White;
            Rect = new RelativeRect(origin.X, origin.Y, width, height, anchor);
        }

        public UILabel(int font, string text, Color color, RelativePoint origin, RelativeSize height, RelativeSize width = null, OriginLocation anchor = OriginLocation.Center) : base()
        {
            Text = text;
            Font = font;

            if (width == null)
                width = RelativeSize.FullWidth;

            BaseColor = color;
            Rect = new RelativeRect(origin.X, origin.Y, width,  height, anchor);
        }

        public UILabel(int font, string text) : base()
        {
            Text = text;
            Font = font;
            BaseColor = Color.White;
            Rect = new RelativeRect(RelativeLoc.XCenter,RelativeLoc.YCenter, RelativeSize.FullWidth, RelativeSize.FullWidth, OriginLocation.Center);
        }

        public UILabel(int font, string text, Color color) : base()
        {
            Text = text;
            Font = font;
            BaseColor = color;
            Rect = new RelativeRect(RelativeLoc.XCenter, RelativeLoc.YCenter, RelativeSize.FullWidth, RelativeSize.FullWidth, OriginLocation.Center);
        }


        public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
            RelativeSize heightCache = Rect.Height;
            RelativeSize widthCache = Rect.Width;

            float pixelHeight = heightCache.ToScreen(x, y) * 0.75f;

            int fontHeight = (int)(pixelHeight + 1);
            var drawInfo = FontManager.DrawText(Font, fontHeight, Text);
            if (drawInfo == null)
                return;

            Rect.Width = new RelativeSize(drawInfo.Size.X, true);
            Rect.Width.Raw = true;

            Rect.Height = new RelativeSize(drawInfo.Size.Y, true);
            Rect.Height.Raw = true;

            Rect.Resize(x, y);
            var PixelSize = Rect.GetPixelSize();

            if (Mat == null || Mat.DiffuseTexture != drawInfo.CachedTexture)
				Mat = GUIManager.GetMaterial(drawInfo.CachedTexture, BaseColor);

			ShapeBuffer.TexturedRect(this, Rect);

            foreach (var child in Children)
            {
                child.Resize((int)PixelSize.X, (int)PixelSize.Y);
            }

            Rect.Height = heightCache;
            Rect.Width = widthCache;
        }
	}
}
