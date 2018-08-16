﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Assets;
using LudicrousElectron.Engine.Graphics.Textures;
using OpenTK;
using LudicrousElectron.Engine;

namespace LudicrousElectron.GUI.Text
{
	public static class FontManager
	{
		private static PrivateFontCollection Fonts = new PrivateFontCollection();

		public static List<FontFamily> TypefaceCache = new List<FontFamily> ();

		private static Bitmap Workspace = null;
		private static Graphics WorkspaceGraphics = null;

		public static int LoadFont(string name)
		{
			string fontFile = AssetManager.GetAssetFullPath(name);
			if (fontFile == string.Empty)
				return -1;

			if (Workspace == null)
			{
				Workspace = new Bitmap(2048, 2048, PixelFormat.Format32bppArgb);
				WorkspaceGraphics = Graphics.FromImage(Workspace);
			}

			Fonts.AddFontFile(fontFile);
			TypefaceCache.Add(Fonts.Families[Fonts.Families.Length - 1]);

			return FontCache.Count - 1;
		}

		internal static Dictionary<Tuple<int, int, string>, FontDrawInfo> StringCache = new Dictionary<Tuple<int, int, string>, FontDrawInfo>();

		internal static Dictionary<Tuple<int, int>, Font> FontCache = new Dictionary<Tuple<int, int>, Font>();

		public class FontDrawInfo
		{
			public string Text = string.Empty;
			public int FontID = -1;
			public int FontSize = 0;
			public Vector2 Size = Vector2.Zero;
			public TextureInfo CachedTexture = null;
		}

		public static FontDrawInfo DrawText(int fontID, int size, string text)
		{
			if (fontID < 0 || fontID >= FontCache.Count)
				return null;

			Tuple<int, int, string> infoId = new Tuple<int, int, string>(fontID, size, text);
			if (!StringCache.ContainsKey(infoId))
			{
				FontDrawInfo info = new FontDrawInfo();

				Tuple<int, int> fontkey = new Tuple<int, int>(fontID, size);

				if (!FontCache.ContainsKey(fontkey))
					FontCache.Add(fontkey, new Font(TypefaceCache[fontID], size));
				Font font = FontCache[fontkey];

				WorkspaceGraphics.Clear(Color.Black);
				var bounds = WorkspaceGraphics.MeasureString(text, font);
				WorkspaceGraphics.DrawString(text, font, Brushes.White, 10, 10);
				WorkspaceGraphics.Flush();

				Bitmap stringMap = Workspace.Clone(new Rectangle(10, 10, (int)(bounds.Width + 1), (int)(bounds.Height + 1)), PixelFormat.Format32bppArgb);

				info.CachedTexture = Core.Textures.CreateTexture("FONTMAN:" + fontID.ToString() + ":" + size.ToString() +":" + text, stringMap);

				info.FontID = fontID;
				info.FontSize = size;
				info.Text = text;
				info.Size = new Vector2(stringMap.Width, stringMap.Height);

				StringCache.Add(infoId, info);

				return info;
			}
			return StringCache[infoId];
		}
	}
}