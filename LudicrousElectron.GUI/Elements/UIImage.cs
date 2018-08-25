using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Geometry;

using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public class UIImage : SingleDrawGUIItem
	{
		public UIImage(RelativePoint origin, string texture) : base()
		{
			IgnoreMouse = true;
			DefaultMaterial = new GUIMaterial(texture, Color.White);
            CheckMaterial();

			Rect.X = origin.X;
			Rect.Y = origin.Y;

			Rect.Width.Mode = RelativeSize.SizeModes.Raw;
			Rect.Width.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.X;
			Rect.Height.Mode = RelativeSize.SizeModes.Raw;
            Rect.Height.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.Y;
		}

		public UIImage(RelativeRect rect, string texture) : base()
		{
			IgnoreMouse = true;

			DefaultMaterial = new GUIMaterial(texture, Color.White);
            CheckMaterial();

            if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null || CurrentMaterial.DiffuseTexture.PixelSize.X == 0 || CurrentMaterial.DiffuseTexture.PixelSize.Y == 0)
				return;

			Rect = new RelativeRect(rect.X, rect.Y, rect.Width, rect.Height, rect.AnchorLocation);
		}

		public UIImage(string texture, RelativePoint origin, OriginLocation anchor = OriginLocation.Center,  RelativeSize _width = null, RelativeSize _height = null) : base()
		{
			IgnoreMouse = true;

            RelativeSize height = _height;
            RelativeSize width = _width;

            DefaultMaterial = new GUIMaterial(texture, Color.White);
            CheckMaterial();

            if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null || CurrentMaterial.DiffuseTexture.PixelSize.X == 0 || CurrentMaterial.DiffuseTexture.PixelSize.Y == 0)
				return;

			if (width == null && height == null)
			{
				// going raw.
				width = new RelativeSize();
				width.Mode = RelativeSize.SizeModes.Raw;
                width.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.X;

				height = new RelativeSize();
				height.Mode = RelativeSize.SizeModes.Raw;
                height.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.Y;

			}
			else if (width == null)
				width = RelativeTools.GetRelativeWidthInAspect(height, CurrentMaterial.DiffuseTexture.PixelSize.X, CurrentMaterial.DiffuseTexture.PixelSize.Y);
			else if (height == null)
				height = RelativeTools.GetRelativeHeightInAspect(width, CurrentMaterial.DiffuseTexture.PixelSize.X, CurrentMaterial.DiffuseTexture.PixelSize.Y);

			Rect = new RelativeRect(origin.X, origin.Y, width, height, anchor);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

            CheckMaterial();

			if (CurrentMaterial.DiffuseTexture == null)
				ShapeBuffer.FilledRect(this, Rect);
			else
				ShapeBuffer.TexturedRect(this, Rect);
		}
	}
}
