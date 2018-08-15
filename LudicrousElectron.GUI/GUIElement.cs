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

		public GUIElement() { }
		public GUIElement(RelativeRect rect) { Rect = rect; }

		public abstract void Draw(GUIRenderLayer layer);
        public virtual void Resize(int x, int y)
        {
            Rect.Resize(x, y);
            var PixelSize = Rect.GetPixelSize();
            foreach (var child in Children)
            {
                child.Resize((int)PixelSize.X, (int)PixelSize.Y);
            }
        }

        public void Render(GUIRenderLayer layer)
        {
			var origin = Rect.GetPixelOrigin();
            
			int stackSize = layer.MatrixStackSize();

            Draw(layer);

            while (layer.MatrixStackSize() > stackSize) // clean out an extra matrices that drawing may have left on the stack
                layer.PopMatrix();

			// move to our origin, so the children can be relative.
 			layer.PushTranslation(origin.X, origin.Y, 0.25f);
 
 			foreach (var child in Children)
                 child.Render(layer);
 
            layer.PopMatrix();
        }
	}

	public abstract class SingleDrawGUIItem :  GUIElement
	{
		public Color BaseColor = Color.White;
		public string Texture = string.Empty;

		public SingleDrawGUIItem():base(){ }
		public SingleDrawGUIItem(RelativeRect rect) : base(rect) {}

		public SingleDrawGUIItem(RelativeRect rect, Color color) : base(rect)
		{
			BaseColor = color;
		}

		public SingleDrawGUIItem(RelativeRect rect, Color color, string textureName) : base(rect)
		{
			BaseColor = color;
			Texture = textureName;
		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (Mat == null)
				Mat = GUIManager.GetMaterial(Texture, BaseColor);

			layer.AddDrawable(this);
		}
	}
}
