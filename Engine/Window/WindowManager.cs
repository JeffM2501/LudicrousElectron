using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Types;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace LudicrousElectron.Engine.Window
{
	public static class WindowManager
	{
		public class Window : GameWindow
		{
			public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags options) : base(width, height, mode, title, options)
            {
                IsMasterDisplay = true;
                ContextID = 1;
            }

            internal  Window(Window parrent) : base()
            {
                IsMasterDisplay = false;
                ContextID = ChildWindows.Count + 2;
                ChildWindows.Add(this);
            }

            public IRenderable RootLayer = RenderLayer.DefaultLayer;
			public virtual void Render() { }

			public List<Window> Children = new List<Window>();
            public bool IsMasterDisplay = false;

            internal int ContextID = int.MinValue;


            protected override void OnLoad(EventArgs e)
            {
                GL.ClearColor(Color.Black);
                base.OnLoad(e);
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                if (IsMasterDisplay)
                    Core.UpdateMain();
                else
                    Core.UpdateChild(ContextID);

                base.OnUpdateFrame(e);

                SwapBuffers();
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                CurrentContextID = ContextID;
                if (IsMasterDisplay)
                    Core.RenderMain();
                else
                    Core.RenderChild(ContextID);

                base.OnRenderFrame(e);
            }
        }

        internal static List<Window> ChildWindows = new List<Window>();

		internal static Window MainWindow = null;

        internal static int CurrentContextID = int.MinValue;

		public class WindowInfo
		{
			public Vector2i Size = new Vector2i();
			public Vector2i Position = null;
			public bool Fullscreen = false;
			public int AntiAliasingFactor = 0;
		}

		public static bool Init(WindowInfo info, IRenderable chain, List<WindowInfo> childWindows = null)
		{
			if (MainWindow != null)
				return false;

			MainWindow = new Window(info.Size.x, info.Size.y, GraphicsMode.Default, "LEWindow", info.Fullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.FixedWindow);
            MainWindow.ContextID = 1;

            CurrentContextID = MainWindow.ContextID;

            return true;
		}
	}
}
