﻿using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.Engine.Window;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.Engine.RenderChain
{
	public class RenderLayer
	{
		protected Stack<Matrix4> MatrixStack = new Stack<Matrix4>();

		public static RenderLayer DefaultLayer = new RenderLayer();

		protected List<IRenderable> RenderItemList = new List<IRenderable>();
		protected List<RenderLayer> ChildLayers = new List<RenderLayer>();

		public class RenderInfo : IComparable<RenderInfo>, IEquatable<RenderInfo>
		{
			public Matrix4 objectMatrix = Matrix4.Identity;
			public Drawable DrawObject = null;

			public RenderInfo(Drawable d, ref Matrix4 mat)
			{
				DrawObject = d;
				objectMatrix = mat;
			}

            public override int GetHashCode()
            {
                return objectMatrix.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj as RenderInfo == null)
                    return false;

                return objectMatrix.Equals((obj as RenderInfo).objectMatrix) && DrawObject == (obj as RenderInfo).DrawObject;
            }

            public int CompareTo(RenderInfo obj)
			{
				RenderInfo other = obj;
				if (other == null)
					return 1;

				if ((DrawObject.CurrentMaterial == null) != (other.DrawObject.CurrentMaterial == null))
					return 1;

				if (DrawObject.CurrentMaterial == null)
					return 1;

				int textureComp = string.Compare(DrawObject.CurrentMaterial.DiffuseName, other.DrawObject.CurrentMaterial.DiffuseName);
				if (textureComp != 0)
					return textureComp;

				if (DrawObject.CurrentMaterial.DiffuseColor.ToArgb() != other.DrawObject.CurrentMaterial.DiffuseColor.ToArgb())
					return DrawObject.CurrentMaterial.DiffuseColor.ToArgb() > other.DrawObject.CurrentMaterial.DiffuseColor.ToArgb() ? 1 : -1;

				return 0;
			}

            public bool Equals(RenderInfo other)
            {
                return objectMatrix.Equals(other.objectMatrix) && DrawObject == other.DrawObject;
            }

            public static bool operator ==(RenderInfo left, RenderInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator >(RenderInfo left, RenderInfo right)
            {
                return left.CompareTo(right) > 0;
            }

            public static bool operator <(RenderInfo left, RenderInfo right)
            {
                return left.CompareTo(right) < 0;
            }

            public static bool operator !=(RenderInfo left, RenderInfo right)
            {
                return !(left == right);
            }
        }
		public List<RenderInfo> Drawables = new List<RenderInfo>();

        public RenderLayer()
        {
			MatrixStack.Push(Matrix4.Identity);
		}

        public void AddChildLayer(RenderLayer child)
		{
			ChildLayers.Add(child);
		}

		public void RemoveItem(IRenderable item)
		{
			if (RenderItemList.Contains(item))
				RenderItemList.Remove(item);
		}

		public void AddItem(IRenderable item)
		{
			if (!RenderItemList.Contains(item))
				RenderItemList.Add(item);
		}

		public void AddDrawable(Drawable draw)
		{
			Matrix4 currMat = MatrixStack.Peek();
			AddDrawable(draw, ref currMat);
		}

		public void AddDrawable(Drawable draw, ref Matrix4 matrix)
		{
			Drawables.Add(new RenderInfo(draw, ref matrix));
		}

		public void ClearDrawables()
		{
			Drawables.Clear();
		}

		public virtual void RenderSetup(WindowManager.Window target)
		{
			if (MatrixStack.Count == 0)
				MatrixStack.Push(Matrix4.Identity);
		}

		public virtual void Render(WindowManager.Window target)
		{
			RenderSetup(target);
			if (Drawables.Count == 0)
			{
				foreach (var item in RenderItemList)
					item.Render(this);
				Sort();
			}
			Draw();

			foreach (var child in ChildLayers)
				child.Render(target);
		}

		public virtual void Sort()
		{
			Drawables.Sort();
		}

		protected virtual void BindMaterial(Material mat)
		{
			if (mat == null)
				return;
			mat.Bind();
		}

		public virtual void Draw()
		{
			Material lastMat = null;
			foreach (var item in Drawables)
			{
				if (item.DrawObject.CurrentMaterial != lastMat)
				{
					BindMaterial(item.DrawObject.CurrentMaterial);
					lastMat = item.DrawObject.CurrentMaterial;
				}

				if (!item.DrawObject.ReadDepth)
					GL.DepthFunc(DepthFunction.Never);
				if (!item.DrawObject.WriteDepth)
					GL.DepthMask(false);

				GL.LoadMatrix(ref item.objectMatrix);
				if (item.DrawObject.Draw())
					lastMat = null;

				if (!item.DrawObject.WriteDepth)
					GL.DepthMask(true);
				if (!item.DrawObject.ReadDepth)
					GL.DepthFunc(DepthFunction.Less);
			}
		}

		public virtual void PushMatrix(Matrix4 mat, bool contact = true)
		{
            if (contact)
                MatrixStack.Push(mat * MatrixStack.Peek());
            else
                MatrixStack.Push(mat);
        }

		public virtual void PopMatrix()
		{
			MatrixStack.Pop();
		}

		public virtual Matrix4 PeekMatrix()
		{
			return MatrixStack.Peek();
		}

		public virtual void Clear()
		{
			MatrixStack.Clear();
			MatrixStack.Push(Matrix4.Identity);
		}

		public void PushTranslation(float x, float y, float z)
		{
            MatrixStack.Push(Matrix4.CreateTranslation(x, y, z) * MatrixStack.Peek());
        }

        public int MatrixStackSize()
		{
			return MatrixStack.Count;
		}
	}
}
