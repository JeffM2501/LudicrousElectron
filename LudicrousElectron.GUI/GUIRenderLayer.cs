using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Engine.Window;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.GUI
{
    public class GUIRenderLayer : RenderLayer
    {
		internal Canvas CurrentCanvas = null;

		internal WindowManager.Window CurrentContext = null;

		public Matrix4 OrthoMatrix = Matrix4.Identity;

		protected Matrix4 InitalMatrix = Matrix4.CreateTranslation(Vector3.UnitZ * -100);

		public override void RenderSetup(WindowManager.Window target)
		{
			CurrentContext = target;

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			var mat = Matrix4.CreateOrthographic(target.Width, target.Height, 0.01f, 1000);
			GL.LoadMatrix(ref mat);
			GL.MatrixMode(MatrixMode.Modelview);

			ClearDrawables();
			MatrixStack.Clear();
			MatrixStack.Push(InitalMatrix);
		}

		public override void Sort()
		{
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
			CurrentCanvas = newCanvas;
			CurrentCanvas.Resize(CurrentContext);
		}
	}
}
