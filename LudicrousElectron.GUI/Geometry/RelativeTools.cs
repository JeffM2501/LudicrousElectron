using LudicrousElectron.Engine;
using LudicrousElectron.Engine.Graphics.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.GUI.Geometry
{
	public static class RelativeTools
	{
		public static RelativeSize GetRelativeHeightInAspect(RelativeSize width, float aspectWidth, float aspectHeight)
		{
			RelativeSize height = new RelativeSize();
			height.UseWidth = width.UseWidth;
			if (aspectWidth == 0 || aspectHeight == 0)
				return height;

			float aspect = aspectWidth / aspectHeight;

			height.Paramater = width.Paramater / aspect;
			return height;
		}

		public static RelativeSize GetRelativeWidthInAspect(RelativeSize height, float aspectWidth, float aspectHeight)
		{
			RelativeSize width = new RelativeSize();
			width.UseWidth = height.UseWidth;
			if (aspectWidth == 0 || aspectHeight == 0)
				return width;

			float aspect = aspectWidth / aspectHeight;

			width.Paramater = height.Paramater * aspect;
			return width;
		}

		public static RelativeSizeXY GetImageRect(string texture, RelativeSize fixedSize, bool useWidth = true)
		{
			RelativeSizeXY size = new RelativeSizeXY();

			TextureInfo info = TextureManager.GetTexture(texture);
			if (info != null && info.PixelSize.X > 0 && info.PixelSize.Y != 0)
			{
				if (useWidth)
				{
					size.Width = fixedSize;
					size.Height = GetRelativeHeightInAspect(fixedSize, info.PixelSize.X, info.PixelSize.Y);
				}
				else
				{
					size.Width = GetRelativeWidthInAspect(fixedSize, info.PixelSize.X, info.PixelSize.Y);
					size.Height = fixedSize;
				}
			}

			return size;
		}
	}
}
