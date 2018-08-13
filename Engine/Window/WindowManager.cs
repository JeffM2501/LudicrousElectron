using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Types;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LudicrousElectron.Engine.Window
{
	public static class WindowManager
	{
		public static string WindowTitleText = "LudicrousElectron.Window";

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

            public List<RenderLayer> Layers = new List<RenderLayer>(new RenderLayer[] { RenderLayer.DefaultLayer });

			public virtual void Render()
			{
				foreach (var layer in Layers)
					layer.Render(this);
			}

			public List<Window> Children = new List<Window>();
            public bool IsMasterDisplay = false;

            public int ContextID { get; internal set; } = int.MinValue;

            protected override void OnLoad(EventArgs e)
            {
				base.OnLoad(e);

				SetClearColor(ClearColor);
				GL.Enable(EnableCap.CullFace);
				GL.CullFace(CullFaceMode.Back);
				GL.FrontFace(FrontFaceDirection.Ccw);
				GL.ShadeModel(ShadingModel.Smooth);
				GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
				GL.Enable(EnableCap.ColorMaterial);
				GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
				// GL.Enable(EnableCap.LineSmooth);
				GL.Enable(EnableCap.DepthTest);

				GL.Disable(EnableCap.Lighting);
			}

			protected override void OnUpdateFrame(FrameEventArgs e)
            {
                if (IsMasterDisplay)
                    Core.UpdateMain();
                else
                    Core.UpdateChild(ContextID);

                base.OnUpdateFrame(e);
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				GL.LoadIdentity();

				CurrentContextID = ContextID;
                if (IsMasterDisplay)
                    Core.RenderMain();
                else
                    Core.RenderChild(ContextID);

				Render();

				base.OnRenderFrame(e);


				SwapBuffers();
			}

			protected override void OnClosed(EventArgs e)
			{
				base.OnClosed(e);
				WindowClosed(this);
			}

			internal void SetClearColor(Color color)
			{
				GL.ClearColor(ClearColor);
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

		public static event EventHandler WindowAdded = null;
		public static event EventHandler WindowRemoved = null;
		public static event EventHandler WindowResized = null;

		public static int MainWindowID { get; internal set; } = -1;

		public static int[] ChildWindowIDs { get; internal set; } = new int[0];

		public static void SetClearColor(Color color)
		{
			ClearColor = color;
			MainWindow.SetClearColor(color);
		}

		public static Color ClearColor { get; private set; } = Color.CornflowerBlue;

		public static bool Init(WindowInfo info, List<WindowInfo> childWindows = null)
		{
			if (MainWindow != null)
				return false;

			MainWindowID = 1;

			MainWindow = new Window(info.Size.x, info.Size.y, GraphicsMode.Default, WindowTitleText, info.Fullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.FixedWindow);
            MainWindow.ContextID = MainWindowID;

            CurrentContextID = MainWindow.ContextID;

			WindowAdded?.Invoke(MainWindow, EventArgs.Empty);

			MainWindow.Resize += MainWindow_Resize;

			return true;
		}

		public static bool Inited()
		{
			return MainWindow != null;
		}

		private static void MainWindow_Resize(object sender, EventArgs e)
		{
			WindowResized?.Invoke(MainWindow, e);
		}

		internal static void WindowClosed(Window win)
		{
			WindowRemoved?.Invoke(win, EventArgs.Empty);
			if (MainWindow == win)
			{
				MainWindow.Exit();
				MainWindow = null;
			}
		}

		public static void ClearRenderLayers()
		{
			if (MainWindow != null)
				MainWindow.Layers.Clear();
		}

		public static RenderLayer AddRenderLayer(RenderLayer layer)
		{
			if (MainWindow != null)
			{
				MainWindow.Layers.Add(layer);
			}
				
			return layer;
		}
	}
}
