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
using LudicrousElectron.Engine.Input;

namespace LudicrousElectron.GUI
{
	public abstract class GUIElement : PrimitivBuffer
	{
		public string Name = string.Empty;
        public object Tag = null;

        internal object LayoutTag = null;

		public Canvas ParentCanvas = null;
        public GUIElement Parent = null;

        public RelativeRect Rect = new RelativeRect();
        public List<GUIElement> Children = new List<GUIElement>();

        protected bool Inited = false;
		public bool IgnoreMouse = false;

		protected Vector2 LastParentSize = new Vector2(-1, -1);

        public event EventHandler<GUIElement> GotDirty = null;
        public void SetDirty() { if (Inited) GotDirty?.Invoke(this, this); }

		protected GUIElement() { }
        protected GUIElement(RelativeRect rect)
        {
            Rect = rect.Clone();
        }

        public virtual void SetParentCanvas(Canvas canvas)
        {
            ParentCanvas = canvas;
            foreach (var child in Children)
                child.SetParentCanvas(canvas);
        }

        public virtual void AddChild(GUIElement child)
        {
            child.Parent = this;
            child.ParentCanvas = ParentCanvas;
            Children.Add(child);
        }

		public abstract void Draw(GUIRenderLayer layer);
        public virtual void Resize(int x, int y)
        {
			LastParentSize.X = x;
			LastParentSize.Y = y;

			Inited = true;

            Rect.Resize(x, y);
            var PixelSize = Rect.GetPixelSize();
            foreach (var child in Children)
                child.Resize((int)PixelSize.X, (int)PixelSize.Y);
        }

        public virtual Vector2 GetScreenOrigin()
        {
            Vector2 origin = Vector2.Zero;
            if (Parent != null)
                origin = Parent.GetScreenOrigin();

            return origin + Rect.GetPixelOrigin();
        }

        public virtual Vector2 GetScreenOriginCenter()
        {
            Vector2 origin = Vector2.Zero;
            if (Parent != null)
                Parent.GetScreenOrigin();

            return origin + Rect.GetPixelOrigin() + (Rect.GetPixelSize() * 0.5f);
        }

        public virtual void ForceRefresh()
		{
			CurrentMaterial = null;
			Resize((int)LastParentSize.X, (int)LastParentSize.Y);
		}

        public void Render(GUIRenderLayer layer)
        {
            var origin = Rect.GetPixelOrigin();
            
			int stackSize = layer.MatrixStackSize();

            Draw(layer);

			// move to our origin, so the children can be relative.
 			layer.PushTranslation(origin.X, origin.Y, 0);

			foreach (var child in Children)
			{
				layer.PushTranslation(0, 0, 0.125f);
				child.Render(layer);
			}
 
            layer.PopMatrix();

			while (layer.MatrixStackSize() > stackSize) // clean out an extra matrices that drawing may have left on the stack
				layer.PopMatrix();
		}

		public virtual List<GUIElement> GetElementsUnderPoint(Vector2 location, InputManager.LogicalButtonState buttons)
		{
			List<GUIElement> elements = new List<GUIElement>();

			if (Inited && !IgnoreMouse)
			{
				if (Rect.PointInRect(location))
				{
					ProcessMouseEvent(location, buttons);
					elements.Add(this);
				}

				Vector2 childLoc = location - Rect.GetPixelOrigin();
				foreach (var child in Children)
				{
					List<GUIElement> childElements = child.GetElementsUnderPoint(childLoc, buttons);
					if (childElements.Count > 0)
						elements.AddRange(childElements.ToArray());
				}
			}

			return elements;
		}

		public virtual void ProcessMouseEvent(Vector2 location, InputManager.LogicalButtonState buttons)
		{
            // the child class may want to do something with this.
		}

        public virtual void FlushMaterials(bool withChildren = false)
        {
            if (withChildren)
            {
                foreach (var child in Children)
                    child.FlushMaterials(true);
            }
        }
    }

    public enum UIFillModes
    {
        Tilled,
        Stretch,
        StretchMiddle,
        Stretch4Quad,
        Fill9Sprite,
        SmartStprite,
    }

	public class LayoutContainer : GUIElement
	{
        public float ChildSpacing = 5;
        public float MaxChildSize = -1;

        public bool FirstElementHasSpacing = false;
        public LayoutContainer(RelativeRect rect) : base (rect)
		{

		}

		public override void Draw(GUIRenderLayer layer)
		{
			// containers don't draw
		}

		public override List<GUIElement> GetElementsUnderPoint(Vector2 location, InputManager.LogicalButtonState buttons)
		{
			List<GUIElement> elements = new List<GUIElement>();

			if (Inited)
			{
				if (!Rect.PointInRect(location))	// we don't add ourself, we are a container.
					return elements;

				Vector2 childLoc = location - Rect.GetPixelOrigin();
				foreach (var child in Children)
				{
					List<GUIElement> childElements = child.GetElementsUnderPoint(childLoc, buttons);
					if (childElements.Count > 0)
						elements.AddRange(childElements.ToArray());
				}
			}

			return elements;
		}
    }

	public abstract class SingleDrawGUIItem :  GUIElement
	{
        public GUIMaterial DefaultMaterial = new GUIMaterial();
        public UIFillModes FillMode = UIFillModes.Tilled;

		protected Vector2 UVScale = new Vector2(1, 1);
		protected bool ReverseU = false;
		protected bool ReverseV = false;

        protected SingleDrawGUIItem():base(){ }
        protected SingleDrawGUIItem(RelativeRect rect) : base(rect) {}

        protected SingleDrawGUIItem(RelativeRect rect, Color color) : base(rect)
		{
            DefaultMaterial.Color = color;
		}

        protected SingleDrawGUIItem(RelativeRect rect, Color color, string textureName) : base(rect)
		{
            DefaultMaterial = new GUIMaterial(textureName, color);
        }

        public virtual GUIMaterial GetCurrentMaterial()
        {
            return DefaultMaterial;
        }

        public override void FlushMaterials( bool withChildren = false)
        {
            CurrentMaterial = null;
            base.FlushMaterials(withChildren);
        }

        protected virtual void CheckMaterial()
        {
            if (CurrentMaterial == null)
                CurrentMaterial = GUIManager.GetMaterial(GetCurrentMaterial());
        }

        public override void Draw(GUIRenderLayer layer)
		{
            CheckMaterial();
            layer.AddDrawable(this);
		}

		public virtual void FlipTextureAxes(int x, int y)
		{
			ReverseU = x < 0;
			ReverseV = y < 0;
		}

        protected void HandleTexturedRect()
        {
            switch (FillMode)
            {
                default: // and titled
                    ShapeBuffer.TexturedRect(this, Rect, CurrentMaterial.DiffuseTexture, ReverseU, ReverseV);
                    break;

                case UIFillModes.Stretch:
                    ShapeBuffer.TexturedRect(this, Rect, UVScale, ReverseU, ReverseV);
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

                case UIFillModes.SmartStprite:
                    if (CurrentMaterial == null || CurrentMaterial.DiffuseTexture == null)
                        ShapeBuffer.TexturedRect(this, Rect, UVScale, ReverseU, ReverseV);
                    else
                    {
                        if (CurrentMaterial.DiffuseTexture != null && CurrentMaterial.DiffuseTexture.HasMetaData("9Sprite"))
                            SlicedSprite.NineSlice(this, Rect, CurrentMaterial.DiffuseTexture.PixelSize);
                        else
                            SlicedSprite.FourSlice(this, Rect, CurrentMaterial.DiffuseTexture.PixelSize);
                    }
                    break;
            }      
        }
	}
}
