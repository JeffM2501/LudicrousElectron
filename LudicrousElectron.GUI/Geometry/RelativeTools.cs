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
		public static RelativeSize GetRelativeHeightInAspect(RelativeSize width, int aspectWidth, int aspectHeight)
		{
			RelativeSize height = new RelativeSize();
			height.UseWidth = width.UseWidth;
			if (aspectWidth == 0 || aspectHeight == 0)
				return height;

			float aspect = (float)aspectWidth / (float)aspectHeight;

			height.Paramater = width.Paramater / aspect;
			return height;
		}

		public static RelativeSize GetRelativeWidthInAspect(RelativeSize height, int aspectWidth, int aspectHeight)
		{
			RelativeSize width = new RelativeSize();
			width.UseWidth = height.UseWidth;
			if (aspectWidth == 0 || aspectHeight == 0)
				return width;

			float aspect = (float)aspectWidth / (float)aspectHeight;

			width.Paramater = height.Paramater * aspect;
			return width;
		}

		public static RelativeSizeXY GetImageRect(string texture, RelativeSize fixedSize, bool useWidth = true)
		{
			RelativeSizeXY size = new RelativeSizeXY();

			TextureInfo info = Core.Textures.GetTexture(texture);
			if (info != null && info.ImageData != null && info.ImageData.Width != 0 && info.ImageData.Height != 0)
			{
				if (useWidth)
				{
					size.Width = fixedSize;
					size.Height = GetRelativeHeightInAspect(fixedSize, info.ImageData.Width, info.ImageData.Height);
				}
				else
				{
					size.Width = GetRelativeWidthInAspect(fixedSize, info.ImageData.Width, info.ImageData.Height);
					size.Height = fixedSize;
				}
			}

			return size;
		}
	}
}
