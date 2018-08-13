using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.Engine.Graphics
{
	public class PrimitivBuffer : Drawable
	{
		public List<Vector3> Vertecies = new List<Vector3>();
		public List<Vector3> Normals = new List<Vector3>();
		public List<Vector2> UVs = new List<Vector2>();
		public List<Color> Colors = new List<Color>();

		public PrimitiveType DrawType = new PrimitiveType();
		public bool UseDepth = true;
		public float OutlineWidth = 0;

		public override bool Draw()
		{
			bool haveNormals = Normals.Count == Vertecies.Count;
			bool haveUVs = UVs.Count == Vertecies.Count;
			bool haveColors = Colors.Count == Vertecies.Count;

			if (!haveColors && Colors.Count > 0)
				GL.Color4(Colors[0]);

			if (OutlineWidth > 0)
				GL.LineWidth(OutlineWidth);

			GL.Begin(DrawType);

			for(int i = 0; i < Vertecies.Count; i++)
			{
				if (haveColors)
					GL.Color4(Colors[i]);

				if (haveUVs)
					GL.TexCoord2(UVs[i]);

				if (haveNormals)
					GL.Normal3(Normals[i]);

				if (UseDepth)
					GL.Vertex3(Vertecies[i]);
				else
					GL.Vertex2(Vertecies[i].X, Vertecies[i].Y);
			}
			GL.End();

			if (OutlineWidth > 0)
				GL.LineWidth(1);

			return haveColors;
		}

		public virtual void Clear()
		{
			Vertecies.Clear();
			Normals.Clear();
			UVs.Clear();
			Colors.Clear();
			OutlineWidth = -1;
			DrawType = PrimitiveType.Triangles;
		}

		public virtual void Vertex(Vector3 vec)
		{
			Vertecies.Add(vec);
		}

		public virtual void Vertex(float x, float y, float z)
		{
			Vertecies.Add(new Vector3(x,y,z));
		}

		public virtual void Vertex(float x, float y)
		{
			UseDepth = false;
			Vertecies.Add(new Vector3(x, y, 0));
		}

		public virtual void VertexNormal(Vector3 vec, Vector3 normal)
		{
			Vertecies.Add(vec);
			Normals.Add(normal);
		}

		public virtual void VertexNormalUV(Vector3 vec, Vector3 normal, Vector2 uv)
		{
			Vertecies.Add(vec);
			Normals.Add(normal);
			UVs.Add(uv);
		}
	}
}
