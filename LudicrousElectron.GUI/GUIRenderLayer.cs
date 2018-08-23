using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.Input;
using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.GUI.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.GUI
{
	public class GUIRenderLayer : RenderLayer
	{
		internal Canvas CurrentCanvas = null;

		internal int CurrentContextID = -1;
		internal WindowManager.Window CurrentContext { get {return WindowManager.GetWindow(CurrentContextID); } }

		public Matrix4 OrthoMatrix = Matrix4.Identity;

		public override void RenderSetup(WindowManager.Window target)
		{
			CurrentContextID = target.ContextID;

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			var mat = Matrix4.CreateOrthographic(target.Width, target.Height, 0.01f, 1000);
			GL.LoadMatrix(ref mat);
			GL.MatrixMode(MatrixMode.Modelview);

			ClearDrawables();
			MatrixStack.Clear();
			MatrixStack.Push(Matrix4.CreateTranslation(target.Width * -0.5f, target.Height * -0.5f, -800));
		}

        public override void Sort()
        {
            // GUI layers can not sort they must be drawn in order, so we override this to prevent the base class from doing it's material based sort
        }

        public override void Render(WindowManager.Window target)
		{
			RenderSetup(target);

			if (CurrentCanvas != null)
				CurrentCanvas.Render(this);

			// we don't sort GUI stuff, because stacking order matters.

			Draw();

			foreach (var child in ChildLayers)
				child.Render(target);
		}

		internal void ChangeCanvas(Canvas newCanvas)
		{
			ClearDrawables();

            if (CurrentCanvas != null)
                CurrentCanvas.Active = false;

			CurrentCanvas = newCanvas;
            Reset();
		}

        public void Reset()
        {
            if (CurrentCanvas != null)
            {
                CurrentCanvas.Active = true;
                CurrentCanvas.Reset();
            }
        }

        internal bool HandleMouseInput(InputManager.MouseFrameEventArgs state)
		{
			if (CurrentContext == null || state.CursorPostion.X < 0 || state.CursorPostion.X > CurrentContext.Width || state.CursorPostion.Y < 0 || state.CursorPostion.Y > CurrentContext.Height)
				return false;

			CurrentCanvas.MouseEvent(state.CursorPostion, state.Buttons);

			return true;
		}
	}
}
