using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Diagnostics;

using LudicrousElectron.Types;
using LudicrousElectron.Assets;


namespace LudicrousElectron.Engine.Graphics.Textures
{
	public static class TextureManager
	{
		public static bool AutoSprite = false;

		private static Dictionary<string, TextureInfo> Textures = new Dictionary<string, TextureInfo>();

        internal static Stopwatch UseageTimer = null;

        public static void Startup()
        {
            if (UseageTimer == null)
            {
                UseageTimer = new Stopwatch();
                UseageTimer.Start();
            }
        }

        public static void Cleanup()
        {
            if (UseageTimer != null)
                UseageTimer.Stop();
        }

        public static TextureInfo GetTexture(string name, Vector2i subDiv = null, TextureInfo.TextureFormats format = TextureInfo.TextureFormats.TextureMap)
		{
			if (!Textures.ContainsKey(name))
				LoadTexture(name, subDiv, format);

			if (!Textures.ContainsKey(name))
				return null;

			return Textures[name];
		}

		public static TextureInfo CreateTexture(string name, Bitmap image, TextureInfo.TextureFormats format = TextureInfo.TextureFormats.TextureMap)
		{
			if (Textures.ContainsKey(name))
			{
				Textures[name].Unbind();
				Textures[name].ImageData = image;
                Textures[name].PixelSize = new OpenTK.Vector2(image.Width, image.Height);
            }
			else
				MakeTexture(name, image, format);

			if (!Textures.ContainsKey(name))
				return null;

			return Textures[name];
		}

        private static void MakeTexture(string name, Bitmap image, TextureInfo.TextureFormats format = TextureInfo.TextureFormats.Text)
		{
			TextureInfo texture = new TextureInfo();
			texture.RelativeName = name;
			texture.FullPath = string.Empty;
			texture.ImageData = image;
            texture.PixelSize = new OpenTK.Vector2(image.Width, image.Height);
            texture.SetTextureFormat(format);

            Textures.Add(name, texture);
		}

        private static void LoadTexture(string name, Vector2i subDiv, TextureInfo.TextureFormats format)
		{
			TextureInfo info = LoadTextureData(name);
            if (info == null)
                return;

            info?.SetTextureFormat(format);

			if (subDiv != null && (subDiv.x > 0 || subDiv.y > 0))
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

			Textures.Add(name, info);
		}

		private static TextureInfo LoadTextureData(string name)
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
            texture.PixelSize = new OpenTK.Vector2(bitmap.Width, bitmap.Height);


            if (AssetManager.AssetExists(name + ".info"))
                texture.MetaData.AddRange(AssetManager.GetAssetLines(name + ".info"));

            return texture;
		}

		internal static void UnbindAll()
		{
			foreach (var texture in Textures.Values)
				texture.Unbind();
		}

		public static int FramesToPurge = 10000;
		public static int MaxPurgePerFrame = 5;

		public static void CheckForPurge()
		{
			if (UseageTimer == null)
				return;

			long now = UseageTimer.ElapsedMilliseconds;
			int count = 0;
			foreach (var texture in Textures.Values)
			{
				if (count >= MaxPurgePerFrame)
					return;

				if (!texture.IsLoaded() || texture.DissalowPurge || texture.GetTextureFormat() != TextureInfo.TextureFormats.Text)
					continue;

				if ((texture.LastUseFrame + FramesToPurge) < now)
				{
					count++;
					texture.Unbind();
					texture.ClearImageData();
#if DEBUG
					System.Diagnostics.Debug.WriteLine("Purged texture " + texture.RelativeName);
#endif
				}
			}
		}
	}
}
