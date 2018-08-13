using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.Graphics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.GUI.Drawing
{
	public static class ShapeBuffer
	{
		public static void OutlineCenteredRect(PrimitivBuffer target, Vector2 center, Vector2 size, float width = 1)
		{
			target.Clear();
			target.OutlineWidth = width;
			target.DrawType = PrimitiveType.LineLoop;

			target.Vertex(center.X - size.X, center.Y - size.Y);
			target.Vertex(center.X - size.X, center.Y + size.Y);
			target.Vertex(center.X + size.X, center.Y + size.Y);
			target.Vertex(center.X + size.X, center.Y - size.Y);
		}

		public static void OutlineCenteredRect(PrimitivBuffer target, Vector2 center, Vector2 size)
		{
			OutlineCenteredRect(target, center, size, 1);
		}

		public static void FilledCenteredRect(PrimitivBuffer target, Vector2 center, Vector2 size)
		{
			target.Clear();
			target.DrawType = PrimitiveType.Quads;

			target.Vertex(center.X - size.X, center.Y - size.Y);
			target.Vertex(center.X + size.X, center.Y - size.Y);
			target.Vertex(center.X + size.X, center.Y + size.Y);
			target.Vertex(center.X - size.X, center.Y + size.Y);
		}

		public static void FilledGradientRect(PrimitivBuffer target, Vector2 center, Vector2 size, Color maxColor, Color minColor, bool horizontal)
		{
			target.Clear();
			target.DrawType = PrimitiveType.Quads;
			if (horizontal)
			{
				target.Colors.Add(minColor);
				target.Vertex(center.X - size.X, center.Y + size.Y);
				target.Colors.Add(minColor);
				target.Vertex(center.X - size.X, center.Y - size.Y);

				target.Colors.Add(maxColor);
				target.Vertex(center.X + size.X, center.Y - size.Y);
				target.Colors.Add(maxColor);
				target.Vertex(center.X + size.X, center.Y + size.Y);
			}
			else
			{
				target.Colors.Add(minColor);
				target.Vertex(center.X - size.X, center.Y - size.Y);
				target.Colors.Add(minColor);
				target.Vertex(center.X + size.X, center.Y - size.Y);

				target.Colors.Add(maxColor);
				target.Vertex(center.X + size.X, center.Y + size.Y);
				target.Colors.Add(maxColor);
				target.Vertex(center.X - size.X, center.Y + size.Y);
			}
		}

		public static void OutlineRect(PrimitivBuffer target, Vector2 minimum, Vector2 maxium, float width)
		{
			OutlineRect(target, minimum.X, minimum.Y, maxium.X, maxium.Y, width);
		}

		public static void DrawRect(PrimitivBuffer target, Vector2 minimum, Vector2 maxium)
		{
			OutlineRect(target, minimum, maxium, 1);
		}

		public static void OutlineRect(PrimitivBuffer target, float minimumX, float minimumY, float maxiumX, float maxiumY, float width)
		{
			target.Clear();
			target.OutlineWidth = width;
			target.DrawType = PrimitiveType.LineLoop;

			target.Vertex(minimumX, minimumY);
			target.Vertex(minimumX, maxiumY);
			target.Vertex(maxiumX, maxiumY);
			target.Vertex(maxiumX, minimumY);
		}

		public static void FilledRect(PrimitivBuffer target, Vector2 minimum, Vector2 maxium)
		{
			FilledRect(target, minimum.X, minimum.Y, maxium.X, maxium.Y);
		}

		public static void FilledRect(PrimitivBuffer target, float minimumX, float minimumY, float maxiumX, float maxiumY)
		{
			target.Clear();
			target.DrawType = PrimitiveType.Polygon;

			target.Vertex(minimumX, minimumY);
			target.Vertex(maxiumX, minimumY);
			target.Vertex(maxiumX, maxiumY);
			target.Vertex(minimumX, maxiumY);
		}

		public static void TexturedRect(PrimitivBuffer target, Vector2 minimum, Vector2 maxium, float uvScale = 1)
		{
			TexturedRect(target, minimum.X, minimum.Y, maxium.X, maxium.Y, uvScale);
		}

		public static void TexturedRect(PrimitivBuffer target, float minimumX, float minimumY, float maxiumX, float maxiumY, float uvScale = 1)
		{
			target.Clear();
			target.DrawType = PrimitiveType.Polygon;

			target.Vertex(minimumX, minimumY);
			target.UVs.Add(new Vector2(0, uvScale));

			target.Vertex(maxiumX, minimumY);
			target.UVs.Add(new Vector2(1, uvScale));

			target.Vertex(maxiumX, maxiumY);
			target.UVs.Add(new Vector2(1, uvScale));

			target.Vertex(minimumX, maxiumY);
			target.UVs.Add(new Vector2(1, uvScale));
		}
	}
}
