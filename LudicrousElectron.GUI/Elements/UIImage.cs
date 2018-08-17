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
			CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);
			if (CurrentMaterial == null)
				return;

			Rect.X = origin.X;
			Rect.Y = origin.Y;

			Rect.Width.Raw = true;
			Rect.Width.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.X;
			Rect.Height.Raw = true;
			Rect.Height.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.Y;

			DefaultColor = Color.White;
		}

		public UIImage(RelativeRect rect, string texture) : base()
		{
			CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);
			if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null || CurrentMaterial.DiffuseTexture.PixelSize.X == 0 || CurrentMaterial.DiffuseTexture.PixelSize.Y == 0)
				return;

			Rect = new RelativeRect(rect.X, rect.Y, rect.Width, rect.Height);

			DefaultColor = Color.White;
		}

		public UIImage(string texture, RelativePoint origin, OriginLocation anchor = OriginLocation.Center,  RelativeSize width = null, RelativeSize height = null) : base()
		{
			DefaultTexture = texture;
			CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);
			if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null || CurrentMaterial.DiffuseTexture.PixelSize.X == 0 || CurrentMaterial.DiffuseTexture.PixelSize.Y == 0)
				return;

			if (width == null && height == null)
			{
				// going raw.
				width = new RelativeSize();
				width.Raw = true;
				width.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.X;

				height = new RelativeSize();
				height.Raw = true;
				height.Paramater = CurrentMaterial.DiffuseTexture.PixelSize.Y;

			}
			else if (width == null)
				width = RelativeTools.GetRelativeWidthInAspect(height, CurrentMaterial.DiffuseTexture.PixelSize.X, CurrentMaterial.DiffuseTexture.PixelSize.Y);
			else if (height == null)
				height = RelativeTools.GetRelativeHeightInAspect(width, CurrentMaterial.DiffuseTexture.PixelSize.X, CurrentMaterial.DiffuseTexture.PixelSize.Y);

			Rect = new RelativeRect(origin.X, origin.Y, width, height, anchor);

			DefaultColor = Color.White;
		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);

			if (CurrentMaterial.DiffuseTexture == null)
				ShapeBuffer.FilledRect(this, Rect);
			else
				ShapeBuffer.TexturedRect(this, Rect);
		}
	}
}
