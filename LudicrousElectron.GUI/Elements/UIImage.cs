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
			Mat = GUIManager.GetMaterial(Texture, BaseColor);
			if (Mat == null)
				return;

			Rect.X = origin.X;
			Rect.Y = origin.Y;

			Rect.Width.Raw = true;
			Rect.Width.Paramater = Mat.DiffuseTexture.ImageData.Width;
			Rect.Height.Raw = true;
			Rect.Height.Paramater = Mat.DiffuseTexture.ImageData.Height;

			BaseColor = Color.White;
		}

		public UIImage(RelativeRect rect, string texture) : base()
		{
			Mat = GUIManager.GetMaterial(Texture, BaseColor);
			if (Mat == null || Mat.DiffuseTexture == null || Mat.DiffuseTexture.ImageData == null || Mat.DiffuseTexture.ImageData.Height == 0)
				return;

			Rect = new RelativeRect(rect.X, rect.Y, rect.Width, rect.Height);

			BaseColor = Color.White;
		}

		public UIImage(string texture, RelativePoint origin, OriginLocation anchor = OriginLocation.Center,  RelativeSize width = null, RelativeSize height = null) : base()
		{
			Texture = texture;
			Mat = GUIManager.GetMaterial(Texture, BaseColor);
			if (Mat == null || Mat.DiffuseTexture == null || Mat.DiffuseTexture.ImageData == null || Mat.DiffuseTexture.ImageData.Height == 0)
				return;

			if (width == null && height == null)
			{
				// going raw.
				width = new RelativeSize();
				width.Raw = true;
				width.Paramater = Mat.DiffuseTexture.ImageData.Width;

				height = new RelativeSize();
				height.Raw = true;
				height.Paramater = Mat.DiffuseTexture.ImageData.Height;

			}
			else if (width == null)
				width = RelativeTools.GetRelativeWidthInAspect(height, Mat.DiffuseTexture.ImageData.Width, Mat.DiffuseTexture.ImageData.Height);
			else if (height == null)
				height = RelativeTools.GetRelativeHeightInAspect(width, Mat.DiffuseTexture.ImageData.Width, Mat.DiffuseTexture.ImageData.Height);

			Rect = new RelativeRect(origin.X, origin.Y, width, height, anchor);

			BaseColor = Color.White;
		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			if (Mat.DiffuseTexture == null)
				ShapeBuffer.FilledRect(this, Rect);
			else
				ShapeBuffer.TexturedRect(this, Rect);
		}
	}
}
