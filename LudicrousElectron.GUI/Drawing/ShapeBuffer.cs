using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.GUI.Geometry;
using LudicrousElectron.Engine.Graphics.Textures;

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

        public static void FilledGradientRect(PrimitivBuffer target, RelativeRect rect, Color maxColor, Color minColor, bool horizontal)
        {
            FilledGradientRect(target, rect.GetPixelOrigin(), rect.GetPixelSize(), maxColor, minColor,horizontal);
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

        public static void OutlineRect(PrimitivBuffer target, RelativeRect rect, float width = 1)
        {
            var origin = rect.GetPixelOrigin();
            var size = rect.GetPixelSize();

            OutlineRect(target, origin.X, origin.Y, origin.X + size.X, origin.Y + size.Y, width);
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

        public static void FilledRect(PrimitivBuffer target, RelativeRect rect)
        {
            var origin = rect.GetPixelOrigin();
            var size = rect.GetPixelSize();

            FilledRect(target, origin.X, origin.Y, origin.X + size.X, origin.Y + size.Y);
        }

        public static void FilledRect(PrimitivBuffer target, Vector2 minimum, Vector2 maxium)
		{
			FilledRect(target, minimum.X, minimum.Y, maxium.X, maxium.Y);
		}

		public static void FilledRect(PrimitivBuffer target, float minimumX, float minimumY, float maxiumX, float maxiumY)
		{
			target.Clear();
			target.DrawType = PrimitiveType.Quads;

			target.Vertex(minimumX, minimumY);
			target.Vertex(maxiumX, minimumY);
			target.Vertex(maxiumX, maxiumY);
			target.Vertex(minimumX, maxiumY);
		}

		public static void TexturedRect(PrimitivBuffer target, RelativeRect rect, bool reverseU = false, bool revserseV = false)
		{
			var origin = rect.GetPixelOrigin();
			var size = rect.GetPixelSize();

			TexturedRect(target, origin.X, origin.Y, origin.X + size.X, origin.Y + size.Y, Vector2.One, reverseU, revserseV);
		}
		public static void TexturedRect(PrimitivBuffer target, RelativeRect rect, Vector2 uvScale, bool reverseU = false, bool revserseV = false)
		{
			var origin = rect.GetPixelOrigin();
			var size = rect.GetPixelSize();

			TexturedRect(target, origin.X, origin.Y, origin.X + size.X, origin.Y + size.Y, uvScale, reverseU, revserseV);
		}

		public static void TexturedRect(PrimitivBuffer target, RelativeRect rect, TextureInfo texture, bool reverseU = false, bool revserseV = false)
        {
            var origin = rect.GetPixelOrigin();
            var size = rect.GetPixelSize();

			if (texture == null || texture.PixelSize.X == 0 || texture.PixelSize.Y == 0)
				return;

			Vector2 uvScale = new Vector2(size.X / texture.PixelSize.X, size.Y / texture.PixelSize.Y);

            TexturedRect(target, origin.X, origin.Y, origin.X + size.X, origin.Y + size.Y, uvScale, reverseU, revserseV);
        }

        public static void TexturedRect(PrimitivBuffer target, Vector2 minimum, Vector2 maxium, Vector2 uvScale, bool reverseU = false, bool revserseV = false)
		{
			TexturedRect(target, minimum.X, minimum.Y, maxium.X, maxium.Y, uvScale, reverseU, revserseV);
		}

		public static void TexturedRect(PrimitivBuffer target, float minimumX, float minimumY, float maxiumX, float maxiumY, Vector2 uvScale, bool reverseU = false, bool revserseV = false)
		{
			target.Clear();
			target.DrawType = PrimitiveType.Quads;

			target.Vertex(minimumX, minimumY);
			target.UVs.Add(new Vector2(reverseU ? uvScale.X : 0, revserseV ? 0 : uvScale.Y));

			target.Vertex(maxiumX, minimumY);
			target.UVs.Add(new Vector2(reverseU ? 0 : uvScale.X, revserseV ? 0 : uvScale.Y));

			target.Vertex(maxiumX, maxiumY);
			target.UVs.Add(new Vector2(reverseU ? 0 : uvScale.X, revserseV ? uvScale.Y : 0));

			target.Vertex(minimumX, maxiumY);
			target.UVs.Add(new Vector2(reverseU ? uvScale.X : 0, revserseV ? uvScale.Y : 0));
		}
	}
}
