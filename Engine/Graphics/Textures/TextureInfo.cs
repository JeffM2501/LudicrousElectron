using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK.Graphics.OpenGL;

using LudicrousElectron.Types;
using LudicrousElectron.Engine.Window;
using System.Drawing.Imaging;

namespace LudicrousElectron.Engine.Graphics.Textures
{
	public class TextureInfo
	{
		public string RelativeName = string.Empty;
		public string FullPath = string.Empty;

		public Bitmap ImageData = null;

		public List<Rect2Di> Sprites = new List<Rect2Di>();

        public Dictionary<int, int> ContextIDs = new Dictionary<int, int>();

        internal int RepeatType = 0;
        internal bool BuildMippams = false;

		internal bool IsAlpha = false;

		public void Unbind()
		{
            if (!ContextIDs.ContainsKey(WindowManager.CurrentContextID))
                return;

			int GLID = ContextIDs[WindowManager.CurrentContextID];
			ContextIDs.Remove(WindowManager.CurrentContextID);
			GL.DeleteTextures(1, ref GLID);
		}

        public void Bind()
        {
            if (ContextIDs.ContainsKey(WindowManager.CurrentContextID))
            {
                GL.BindTexture(TextureTarget.Texture2D, ContextIDs[WindowManager.CurrentContextID]);
                return;

            }
            int GLID = int.MinValue;
            GL.GenTextures(1, out GLID);

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, GLID);

//             if (IsAlpha && ImageData.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppRgb)
//             {
//                 BitmapData data = ImageData.LockBits(new Rectangle(0, 0, ImageData.Width, ImageData.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//                 GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
//                 ImageData.UnlockBits(data);
//             }
//             else
            {
                BitmapData data = ImageData.LockBits(new Rectangle(0, 0, ImageData.Width, ImageData.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                ImageData.UnlockBits(data);
            }
            //bitmap.Dispose();

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, RepeatType);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, RepeatType);

            GL.BindTexture(TextureTarget.Texture2D, GLID);

            ContextIDs[WindowManager.CurrentContextID] = GLID;
        }
	}
}
