using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
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
        public object Tag = null;

		internal Bitmap ImageData = null;

		public List<Rect2Di> Sprites = new List<Rect2Di>();

        public Dictionary<int, int> ContextIDs = new Dictionary<int, int>();

        public delegate Bitmap GenerateImageDataCB(TextureInfo info);
        public GenerateImageDataCB GenerateImageData = null;

        public bool CacheImageData = true;

        public long LastUseFrame = 0;

        public Vector2 PixelSize = Vector2.Zero;

        public enum TextureFormats
        {
            TextureMap, // mip mapped
            Sprite,
            Text,
        }

        protected TextureFormats TextureFormat = TextureFormats.TextureMap;

        public void SetTextureFormat(TextureFormats format)
        {
            TextureFormat = format;
            Unbind();
        }

        public TextureFormats GetTextureFormat()
        {
            return TextureFormat;
        }

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
            LastUseFrame = TextureManager.UseageTimer.ElapsedMilliseconds;

            if (ContextIDs.ContainsKey(WindowManager.CurrentContextID))
            {
                GL.BindTexture(TextureTarget.Texture2D, ContextIDs[WindowManager.CurrentContextID]);
                return;
            }

            if (ImageData == null)
            {
                if (GenerateImageData == null)
                    return;

                ImageData = GenerateImageData(this);
                if (ImageData == null)
                    return;
            }

            int GLID = int.MinValue;
            GL.GenTextures(1, out GLID);

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, GLID);

            BitmapData data = ImageData.LockBits(new Rectangle(0, 0, ImageData.Width, ImageData.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            switch (TextureFormat)
            {
                case TextureFormats.TextureMap:
                  
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                    break;

                case TextureFormats.Sprite:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    break;

                case TextureFormats.Text:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                    break;

            }

            GL.BindTexture(TextureTarget.Texture2D, GLID);
            ImageData.UnlockBits(data);

            PixelSize = new Vector2(ImageData.Width, ImageData.Height);

            ContextIDs[WindowManager.CurrentContextID] = GLID;

            if (!CacheImageData)
                ImageData.Dispose();
        }
	}
}
