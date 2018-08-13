using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


using LudicrousElectron.Engine.Graphics.Textures;

namespace LudicrousElectron.Engine.Graphics
{
	public class Material
	{
		public string Name = string.Empty;

		public string DiffuseName = string.Empty;
		internal TextureInfo DiffuseTexture = null;
		public Color DiffuseColor = Color.White;

		public string NormalMapName = string.Empty;
		internal TextureInfo NormalMapTexture = null;

		public string SpecularMapName = string.Empty;
		internal TextureInfo SpecularTexture = null;
		public Color SpecularColor = Color.Transparent;

		public string EmissionMapName = string.Empty;
		internal TextureInfo EmissionTexture = null;
		public Color EmissionColor = Color.Transparent;

		public static readonly Material Default = new Material();

		public void Bind()
		{
			// load shaders here when we have them

			// GL 1.X pipeline
			if (DiffuseName == string.Empty)
				GL.Disable(EnableCap.Texture2D);
			else
			{
				GL.Enable(EnableCap.Texture2D);

				if (DiffuseTexture == null)
					DiffuseTexture = Core.Textures.GetTexture(DiffuseName);

				if (DiffuseTexture == null)
				{
					DiffuseName = string.Empty;
					GL.Disable(EnableCap.Texture2D);
				}
				else
					DiffuseTexture.Bind();
			}

			GL.Color4(DiffuseColor);
		}
	}
}
