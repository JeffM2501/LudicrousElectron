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
			CurrentMaterial = GUIManager.GetMaterial(Texture, BaseColor);
			if (CurrentMaterial == null)
				return;

			Rect.X = origin.X;
			Rect.Y = origin.Y;

			Rect.Width.Raw = true;
			Rect.Width.Paramater = CurrentMaterial.DiffuseTexture.ImageData.Width;
			Rect.Height.Raw = true;
			Rect.Height.Paramater = CurrentMaterial.DiffuseTexture.ImageData.Height;

			BaseColor = Color.White;
		}

		public UIImage(RelativeRect rect, string texture) : base()
		{
			CurrentMaterial = GUIManager.GetMaterial(Texture, BaseColor);
			if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null || CurrentMaterial.DiffuseTexture.ImageData == null || CurrentMaterial.DiffuseTexture.ImageData.Height == 0)
				return;

			Rect = new RelativeRect(rect.X, rect.Y, rect.Width, rect.Height);

			BaseColor = Color.White;
		}

		public UIImage(string texture, RelativePoint origin, OriginLocation anchor = OriginLocation.Center,  RelativeSize width = null, RelativeSize height = null) : base()
		{
			Texture = texture;
			CurrentMaterial = GUIManager.GetMaterial(Texture, BaseColor);
			if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null || CurrentMaterial.DiffuseTexture.ImageData == null || CurrentMaterial.DiffuseTexture.ImageData.Height == 0)
				return;

			if (width == null && height == null)
			{
				// going raw.
				width = new RelativeSize();
				width.Raw = true;
				width.Paramater = CurrentMaterial.DiffuseTexture.ImageData.Width;

				height = new RelativeSize();
				height.Raw = true;
				height.Paramater = CurrentMaterial.DiffuseTexture.ImageData.Height;

			}
			else if (width == null)
				width = RelativeTools.GetRelativeWidthInAspect(height, CurrentMaterial.DiffuseTexture.ImageData.Width, CurrentMaterial.DiffuseTexture.ImageData.Height);
			else if (height == null)
				height = RelativeTools.GetRelativeHeightInAspect(width, CurrentMaterial.DiffuseTexture.ImageData.Width, CurrentMaterial.DiffuseTexture.ImageData.Height);

			Rect = new RelativeRect(origin.X, origin.Y, width, height, anchor);

			BaseColor = Color.White;
		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(Texture, BaseColor);

			if (CurrentMaterial.DiffuseTexture == null)
				ShapeBuffer.FilledRect(this, Rect);
			else
				ShapeBuffer.TexturedRect(this, Rect);
		}
	}
}
