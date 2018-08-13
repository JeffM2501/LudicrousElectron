using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.Engine.Graphics;

using LudicrousElectron.Types;
using LudicrousElectron.GUI.Geometry;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace LudicrousElectron.GUI
{
	public abstract class GUIElement : PrimitivBuffer
	{
        public RelativeRect Rect = new RelativeRect();
        public List<GUIElement> Children = new List<GUIElement>();

        public abstract void Draw(GUIRenderLayer layer);
        public virtual void Resize(int x, int y)
        {
            Rect.Resize(x, y);
            var PixelSize = Rect.GetPixelSize();
            foreach (var child in Children)
            {
                child.Resize((int)PixelSize.X * 2, (int)PixelSize.Y * 2);
            }
        }

        public void Render(GUIRenderLayer layer)
        {
            var origin = Rect.GetPixelOrigin();
            layer.MatrixStack.Push(Matrix4.CreateTranslation(origin.X, origin.Y, 0.25f));
            int stackSize = layer.MatrixStack.Count;

            Draw();

            while (layer.MatrixStack.Count > stackSize) // clean out an extra matrices that drawing may have left on the stack
                layer.MatrixStack.Pop();

            foreach (var child in Children)
                child.Render(layer);

            layer.MatrixStack.Pop();
        }
	}

	public abstract class SingleDrawGUIItem :  GUIElement
	{
		public Color BaseColor = Color.White;
		public string Texture = string.Empty;

		public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}
	}
}
