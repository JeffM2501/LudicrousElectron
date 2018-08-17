using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.GUI.Geometry;


namespace LudicrousElectron.GUI.Drawing.Sprite
{
	public static class StrechedBuffer
	{
		public static void Stretched(PrimitivBuffer target, RelativeRect rect)
		{
			var origin = rect.GetPixelOrigin();
			var size = rect.GetPixelSize();

			if (size.X >= size.Y)
				StretchedH(target, rect);
// 			else
// 				StretchedV(target, rect);
		}

		public static void StretchedH(PrimitivBuffer target, RelativeRect rect)
		{
			var origin = rect.GetPixelOrigin();
			var size = rect.GetPixelSize();

			if (size.X <= size.Y)
			{
				ShapeBuffer.TexturedRect(target, rect);
				return;
			}

			float halfHeight = size.Y * 0.5f;

			target.Clear();
			target.DrawType = PrimitiveType.Quads;

			float minimumX = origin.X;
			float minimumY = origin.Y;
			float maxiumX = origin.X + halfHeight;
			float maxiumY = origin.Y + size.Y;

			float minU = 0;
			float maxU = 0.5f;
			float minV = 0;
			float maxV = 1;

			// left side
			target.Vertex(minimumX, minimumY);
			target.UVs.Add(new Vector2(minU, maxV));

			target.Vertex(maxiumX, minimumY);
			target.UVs.Add(new Vector2(maxU, maxV));

			target.Vertex(maxiumX, maxiumY);
			target.UVs.Add(new Vector2(maxU, minV));

			target.Vertex(minimumX, maxiumY);
			target.UVs.Add(new Vector2(minU, minV));

			// middle 
			minimumX = origin.X + halfHeight;
			minimumY = origin.Y;
			maxiumX = origin.X + size.X - halfHeight;
			maxiumY = origin.Y + size.Y;

			minU = 0.5f;
			maxU = 0.5f;
			minV = 0;
			maxV = 1;

			target.Vertex(minimumX, minimumY);
			target.UVs.Add(new Vector2(minU, maxV));

			target.Vertex(maxiumX, minimumY);
			target.UVs.Add(new Vector2(maxU, maxV));

			target.Vertex(maxiumX, maxiumY);
			target.UVs.Add(new Vector2(maxU, minV));

			target.Vertex(minimumX, maxiumY);
			target.UVs.Add(new Vector2(minU, minV));


			// right 
			minimumX = origin.X + size.X - halfHeight;
			minimumY = origin.Y;
			maxiumX = origin.X + size.X ;
			maxiumY = origin.Y + size.Y;

			minU = 0.5f;
			maxU = 1f;
			minV = 0;
			maxV = 1;

			target.Vertex(minimumX, minimumY);
			target.UVs.Add(new Vector2(minU, maxV));

			target.Vertex(maxiumX, minimumY);
			target.UVs.Add(new Vector2(maxU, maxV));

			target.Vertex(maxiumX, maxiumY);
			target.UVs.Add(new Vector2(maxU, minV));

			target.Vertex(minimumX, maxiumY);
			target.UVs.Add(new Vector2(minU, minV));

		}
	}
}
