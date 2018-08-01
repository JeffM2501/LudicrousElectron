using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

using LudicrousElectron.Types;
using LudicrousElectron.Assets;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing.Imaging;

namespace LudicrousElectron.Engine.Graphics.Textures
{
	public class TextureManager
	{
		public bool DefaultRepeat = false;
		public bool DefaultSmooth = false;
		public bool AutoSprite = false;

		private Dictionary<string, TextureInfo> Textures = new Dictionary<string, TextureInfo>();

		public TextureInfo GetTexture(string name, Vector2i subDiv)
		{
			if (!Textures.ContainsKey(name))
				LoadTexture(name, subDiv);

			if (!Textures.ContainsKey(name))
				return null;

			return Textures[name];
		}

		protected int GetRepeat()
		{
			if (DefaultRepeat)
				return (int)TextureWrapMode.Repeat;
			else
				return (int)TextureWrapMode.ClampToEdge;
		}

		protected void LoadTexture(string name, Vector2i subDiv)
		{
			TextureInfo info = LoadTextureData(name);

			Color clearColor = Color.FromArgb(255, 0, 255);

			if (subDiv.x > 0 || subDiv.y > 0)
			{
				subDiv.x = System.Math.Max(subDiv.x, 1);
				subDiv.y = System.Math.Max(subDiv.y, 1);

				int w = info.ImageData.Width / subDiv.x;
				int h = info.ImageData.Height / subDiv.y;

				for (int y = 0; y < subDiv.y; y++)
				{
					for (int x = 0; x < subDiv.x; x++)
					{
						info.Sprites.Add(new Rect2Di(x * w, y * h, w, h));
					}
				}
			}
			else if (AutoSprite)
			{
				for (int y = 0; y < info.ImageData.Height; y++)
				{
					for (int x = 0; x < info.ImageData.Width; x++)
					{
						if (info.ImageData.GetPixel(x, y) == clearColor)
						{
							int x1 = x + 1;
							for (; x1 < info.ImageData.Width; x1++)
							{
								if (info.ImageData.GetPixel(x1, y) == clearColor)
									break;
							}

							int y1 = y + 1;
							for (; y1 < info.ImageData.Height; y1++)
							{
								if (info.ImageData.GetPixel(x, y1) == clearColor)
									break;
							}
							if (x1 - x > 1 && y1 - y > 1 && x1 < info.ImageData.Width && y1 < info.ImageData.Height)
								info.Sprites.Add(new Rect2Di(x + 1, y + 1, x1 - x - 1, y1 - y - 1));
							x = x1 - 1;
						}
					}
				}
			}

			Textures.Add(name, info);
		}

		protected TextureInfo LoadTextureData(string name)
		{
			Stream fs = AssetManager.GetAssetStream(name);
			if (fs == null)
			{
				fs = AssetManager.GetAssetStream(name + ".png");
				if (fs == null)
					return null;
			}

			Bitmap bitmap = Bitmap.FromStream(fs) as Bitmap;
			fs.Close();

			if (bitmap == null)
				return null;

			TextureInfo texture = new TextureInfo();
			texture.RelativeName = name;
			texture.FullPath = string.Empty;
			texture.ImageData = bitmap;

			return texture;
		}

	}
}
