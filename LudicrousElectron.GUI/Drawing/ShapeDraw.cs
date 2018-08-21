using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using LudicrousElectron.Types;

namespace LudicrousElectron.GUI.Drawing
{
	public static class ShapeDraw
	{
		public static double ArcSegmentation = (7.0).ToRad();

		public static void DrawArc(double startAngle, double endAngle, Vector2 center, double radius)
		{
			DrawArc(startAngle, endAngle, center, radius, 1);
		}

		public static void DrawArc(double startAngle, double endAngle, Vector2 center, double radius, double width)
		{
			GL.LineWidth((float)width);

			startAngle = startAngle.ToRad();
			endAngle = endAngle.ToRad();

			double efectiveSegmentation = ArcSegmentation * (100.0 / radius);

			if (efectiveSegmentation < ArcSegmentation)
				efectiveSegmentation = ArcSegmentation;

			int segments = (int)Math.Ceiling((endAngle - startAngle) / efectiveSegmentation);

			double increment = (endAngle - startAngle) / segments;

			GL.PushMatrix();
			GL.Translate(center.X, center.Y, 0);

			GL.Begin(PrimitiveType.LineStrip);
			for (double d = startAngle; d < endAngle; d += increment)
				GL.Vertex2(d.ToVector2d() * radius);

			GL.Vertex2(endAngle.ToVector2d() * radius);
			GL.End();

			GL.PopMatrix();
			GL.LineWidth(1);
		}

		public static void DrawCircle(Vector2 center, double radius)
		{
			DrawArc(0, 360, center, radius);
		}

		public static void DrawCircle(Vector2 center, double radius, double width)
		{
			DrawArc(0, 360, center, radius, width);
		}

		public static void FillArc(double startAngle, double endAngle, Vector2 center, double radius)
		{
			startAngle = startAngle.ToRad();
			endAngle = endAngle.ToRad();

			int segments = (int)Math.Ceiling((endAngle - startAngle) / ArcSegmentation);

			double increment = (endAngle - startAngle) / segments;

			GL.PushMatrix();
			GL.Translate(center.X, center.Y, 0);

			GL.Begin(PrimitiveType.Polygon);
			for (double d = startAngle; d < endAngle; d += increment)
				GL.Vertex2(d.ToVector2d() * radius);

			GL.Vertex2(endAngle.ToVector2d() * radius);
			GL.End();

			GL.PopMatrix();
		}

		public static void FillCircle(Vector2 center, double radius)
		{
			FillArc(0, 360, center, radius);
		}

		public static void DrawCenteredRect(Vector2 center, Vector2 size, double width)
		{
			GL.LineWidth((float)width);

			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex2(center.X - size.X, center.Y - size.Y);
			GL.Vertex2(center.X - size.X, center.Y + size.Y);
			GL.Vertex2(center.X + size.X, center.Y + size.Y);
			GL.Vertex2(center.X + size.X, center.Y - size.Y);
			GL.End();
			GL.LineWidth(1);
		}

		public static void DrawCenteredRect(Vector2 center, Vector2 size)
		{
			DrawCenteredRect(center, size, 1);
		}

		public static void FillCenteredRect(Vector2 center, Vector2 size)
		{
			GL.Begin(PrimitiveType.Quads);
			GL.Vertex2(center.X - size.X, center.Y - size.Y);
			GL.Vertex2(center.X + size.X, center.Y - size.Y);
			GL.Vertex2(center.X + size.X, center.Y + size.Y);
			GL.Vertex2(center.X - size.X, center.Y + size.Y);
			GL.End();
		}

		public static void FillGradientRect(Vector2 center, Vector2 size, Color maxColor, Color minColor, bool horizontal)
		{
			GL.Begin(PrimitiveType.Quads);
			if (horizontal)
			{
				GL.Color4(minColor);
				GL.Vertex2(center.X - size.X, center.Y + size.Y);
				GL.Vertex2(center.X - size.X, center.Y - size.Y);

				GL.Color4(maxColor);
				GL.Vertex2(center.X + size.X, center.Y - size.Y);
				GL.Vertex2(center.X + size.X, center.Y + size.Y);
			}
			else
			{
				GL.Color4(minColor);
				GL.Vertex2(center.X - size.X, center.Y - size.Y);
				GL.Vertex2(center.X + size.X, center.Y - size.Y);

				GL.Color4(maxColor);
				GL.Vertex2(center.X + size.X, center.Y + size.Y);
				GL.Vertex2(center.X - size.X, center.Y + size.Y);
			}
			GL.End();
		}

		public static void DrawRect(Vector2 minimum, Vector2 maxium, double width)
		{
			DrawRect(minimum.X, minimum.Y, maxium.X, maxium.Y, width);
		}

		public static void DrawRect(Vector2 minimum, Vector2 maxium)
		{
			DrawRect(minimum, maxium, 1);
		}

		public static void DrawRect(float minimumX, float minimumY, float maxiumX, float maxiumY, double width)
		{
			GL.LineWidth((float)width);

			GL.Begin(PrimitiveType.LineLoop);
			GL.Vertex2(minimumX, minimumY);
			GL.Vertex2(minimumX, maxiumY);
			GL.Vertex2(maxiumX, maxiumY);
			GL.Vertex2(maxiumX, minimumY);
			GL.End();
			GL.LineWidth(1);
		}

		public static void FillRect(Vector2 minimum, Vector2 maxium)
		{
			FillRect(minimum.X, minimum.Y, maxium.X, maxium.Y);
		}

		public static void FillRect(float minimumX, float minimumY, float maxiumX, float maxiumY)
		{
			GL.Begin(PrimitiveType.Polygon);
			GL.Vertex2(minimumX, minimumY);
			GL.Vertex2(maxiumX, minimumY);
			GL.Vertex2(maxiumX, maxiumY);
			GL.Vertex2(minimumX, maxiumY);

			GL.End();
		}

		public static void DrawDashedLine(Vector2 sp, Vector2 ep, float dashLen, float dotLen)
		{
			DrawDashedLine(sp, ep, dashLen, dotLen, 1);
		}

		public static void DrawEvenDashedLine(Vector2 sp, Vector2 ep, float len)
		{
			DrawDashedLine(sp, ep, len, len, 1);
		}

		public static void DrawEvenDashedLine(Vector2 sp, Vector2 ep, float len, float width)
		{
			DrawDashedLine(sp, ep, len, len, width);
		}

		public static void DrawDashedLine(Vector2 sp, Vector2 ep, float dashLen, float dotLen, float width)
		{
			Vector2 vec = ep - sp;

			GL.LineWidth(width);

			float len = vec.Length;
			vec.Normalize();

			float l = 0;
			bool dash = true;
			while (l <= len)
			{
				if (dash)
				{
					GL.Begin(PrimitiveType.Lines);

					GL.Vertex2(sp + vec * l);

					float epL = l + dashLen;
					if (epL > len)
						epL = len;

					GL.Vertex2(sp + vec * epL);
					GL.End();

					l += dashLen;
				}
				else
					l += dotLen;

				dash = !dash;

			}

			GL.LineWidth(1);
		}
	}
}
