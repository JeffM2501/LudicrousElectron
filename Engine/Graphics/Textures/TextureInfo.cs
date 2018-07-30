using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using LudicrousElectron.Types;

namespace LudicrousElectron.Engine.Graphics.Textures
{
	public class TextureInfo
	{
		public string RelativeName = string.Empty;
		public string FullPath = string.Empty;

		public Bitmap ImageData = null;

		public List<Rect2Di> Sprites = new List<Rect2Di>();

		public int GLID = int.MinValue;
	}
}
