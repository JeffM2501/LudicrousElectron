using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.Engine.Graphics;

using LudicrousElectron.Types;
using LudicrousElectron.GUI.Geometry;
using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Drawing.Sprite;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace LudicrousElectron.GUI
{
	public abstract class GUIElement : PrimitivBuffer
	{
		public string Name = string.Empty;

        public RelativeRect Rect = new RelativeRect();
        public List<GUIElement> Children = new List<GUIElement>();

        public event EventHandler<GUIElement> GotDirty = null;
        public void SetDirty() { GotDirty?.Invoke(this, this); }

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

    public enum UIFillModes
    {
        Tilled,
        Stretch,
        StretchMiddle,
        Stretch4Quad,
        Fill9Sprite,
    }


    public abstract class SingleDrawGUIItem :  GUIElement
	{
		public Color DefaultColor = Color.White;
		public string DefaultTexture = string.Empty;

        public UIFillModes FillMode = UIFillModes.Tilled;

        public SingleDrawGUIItem():base(){ }
		public SingleDrawGUIItem(RelativeRect rect) : base(rect) {}

		public SingleDrawGUIItem(RelativeRect rect, Color color) : base(rect)
		{
			DefaultColor = color;
		}

		public SingleDrawGUIItem(RelativeRect rect, Color color, string textureName) : base(rect)
		{
			DefaultColor = color;
			DefaultTexture = textureName;
		}

		public override void Draw(GUIRenderLayer layer)
		{
			if (CurrentMaterial == null)
				CurrentMaterial = GUIManager.GetMaterial(DefaultTexture, DefaultColor);

			layer.AddDrawable(this);
		}

        protected void HandleTexturedRect()
        {
            switch (FillMode)
            {
                default:
                case UIFillModes.Tilled:
                    ShapeBuffer.TexturedRect(this, Rect, CurrentMaterial.DiffuseTexture);
                    break;

                case UIFillModes.Stretch:
                    ShapeBuffer.TexturedRect(this, Rect);
                    break;

                case UIFillModes.StretchMiddle:
                    StrechedBuffer.Stretched(this, Rect);
                    break;

                case UIFillModes.Stretch4Quad:
                    SlicedSprite.FourSlice(this, Rect, CurrentMaterial.DiffuseTexture.PixelSize);
                    break;

                case UIFillModes.Fill9Sprite:
                    SlicedSprite.NineSlice(this, Rect, CurrentMaterial.DiffuseTexture.PixelSize);
                    break;
            }      
        }
	}
}
