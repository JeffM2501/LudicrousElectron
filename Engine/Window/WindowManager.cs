using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.Graphics.Textures;
using LudicrousElectron.Engine.Input;
using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Types;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace LudicrousElectron.Engine.Window
{
    public static class WindowManager
    {
        public static string WindowTitleText = "LudicrousElectron.Window";

        public class Window : GameWindow
        {
			internal WindowInfo SetupInfo = null;

            public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags options) : base(width, height, mode, title, options)
            {
                IsMasterDisplay = true;
                ContextID = 1;
            }

            internal Window(Window parrent) : base()
            {
                IsMasterDisplay = false;
                ContextID = ChildWindowList.Count + 2;
                ChildWindowList.Add(this);
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

				GL.Viewport(0, 0, this.Width, this.Height);
                SetGLClearColor(ClearColor);
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
                GL.FrontFace(FrontFaceDirection.Ccw);
                GL.ShadeModel(ShadingModel.Smooth);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);

                GL.Enable(EnableCap.DepthTest);

                GL.Disable(EnableCap.Lighting);
            }

			internal void ForceReload()
			{
				OnLoad(null);
			}

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
				if (RestartData != null)
				{
					RemakeWindows();
					this.Close();
					return;
				}
                SetCurrentWindow(this);

				InputManager.PollInput(CurrentContextID);

				if (IsMasterDisplay)
                    Core.UpdateMain();
                else
                    Core.UpdateChild(ContextID);

                base.OnUpdateFrame(e);
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
				if (RestartData != null)
					return;

                SetCurrentWindow(this);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.LoadIdentity();

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

             internal void SetGLClearColor(Color color)
             {
                MakeCurrent();
                GL.ClearColor(ClearColor);
             }
        }

        internal static List<Window> ChildWindowList = new List<Window>();
        public static Window[] ChildWindows { get { return ChildWindowList.ToArray(); } }

        public static Window MainWindow {get; internal set;} = null;

        internal static int CurrentContextID { get; private set; } = int.MinValue;
        private static void SetCurrentWindow(Window win)
        {
            if (win == null)
                return;

            win.MakeCurrent();
            CurrentContextID = win.ContextID;
        }

		public class WindowInfo
		{
			public Vector2i Size = new Vector2i();
			public Vector2i Position = null;

			public enum WindowSizeTypes
			{
				Normal,
				Maximized,
				Fullscreen,
			}
			public WindowSizeTypes SizeType = WindowSizeTypes.Normal;
			public int AntiAliasingFactor = 0;
		}

		public static event EventHandler WindowAdded = null;
		public static event EventHandler WindowRemoved = null;
		public static event EventHandler WindowResized = null;

		public static int MainWindowID { get; internal set; } = -1;

		public static int[] ChildWindowIDs { get; internal set; } = new int[0];

		public static int MainWindowAAFactor { get; private set; } = 0;

		internal class RestartInfo
		{
			public class WinData
			{
				public WindowInfo NewInfo = null;
				public List<RenderLayer> Layers = null;
			}
			public WinData MainData = null;
			public Dictionary<int, WinData> NewChildInfos = new Dictionary<int, WinData>();
		}

		internal static RestartInfo RestartData = null;

		public static WindowInfo GetWindowInfo(int contextID)
		{
			if (contextID == MainWindowID)
				return MainWindow?.SetupInfo;

			return ChildWindowList.Find((x) => x.ContextID == contextID)?.SetupInfo;
		}

		public static Window GetWindow(int contextID)
		{
			if (contextID == MainWindowID)
				return MainWindow;

			return ChildWindowList.Find((x) => x.ContextID == contextID);
		}

		public static void SetClearColor(Color color)
		{
			ClearColor = color;
			MainWindow.SetGLClearColor(color);
		}

		public static Color ClearColor { get; private set; } = Color.CornflowerBlue;

		public static bool Init(WindowInfo info, List<WindowInfo> childWindows = null)
		{
			if (MainWindow != null)
				return false;

			MainWindowID = 1;

			GraphicsMode thisMode = new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, info.AntiAliasingFactor);

			MainWindow = new Window(info.Size.x, info.Size.y, thisMode, WindowTitleText, info.SizeType == WindowInfo.WindowSizeTypes.Fullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.FixedWindow);
            MainWindow.ContextID = MainWindowID;
			MainWindow.SetupInfo = info;

            MainWindow.KeyPress += Window_KeyPress;

            SetCurrentWindow(MainWindow);

			MainWindowAAFactor = MainWindow.SetupInfo.AntiAliasingFactor; // so that prefs can save it when we close, it'll be the last one that actualy worked

			if (RestartData == null)
				WindowAdded?.Invoke(MainWindow, EventArgs.Empty);

			MainWindow.Resize += MainWindow_Resize;

			if (info.SizeType == WindowInfo.WindowSizeTypes.Maximized)
				SetMaximized();

			return true;
		}

        private static void Window_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputManager.ProcessKeyPress(e);
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
			if (RestartData != null)
				return;

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

		public static void SetFullscreen()
		{
			if (MainWindow == null || MainWindow.SetupInfo.SizeType == WindowInfo.WindowSizeTypes.Fullscreen)
				return;

			MainWindow.WindowState = WindowState.Fullscreen;
			MainWindow.SetupInfo.SizeType = WindowInfo.WindowSizeTypes.Fullscreen;
			MainWindow.ForceReload();

			foreach (var child in ChildWindowList)
			{
				child.WindowState = WindowState.Fullscreen;
				child.SetupInfo.SizeType = WindowInfo.WindowSizeTypes.Fullscreen;
				MainWindow.ForceReload();
			}
		}

		public static void SetMaximized()
		{
			if (MainWindow == null || MainWindow.SetupInfo.SizeType == WindowInfo.WindowSizeTypes.Maximized)
				return;

			MainWindow.WindowState = WindowState.Maximized;
			MainWindow.SetupInfo.SizeType = WindowInfo.WindowSizeTypes.Maximized;
			MainWindow.ForceReload();

			foreach (var child in ChildWindowList)
			{
				child.WindowState = WindowState.Maximized;
				child.SetupInfo.SizeType = WindowInfo.WindowSizeTypes.Maximized;
				MainWindow.ForceReload();
			}
		}

		public static void SetNormal()
		{
			if (MainWindow == null || MainWindow.SetupInfo.SizeType == WindowInfo.WindowSizeTypes.Normal)
				return;

			MainWindow.WindowState = WindowState.Normal;
			MainWindow.SetupInfo.SizeType = WindowInfo.WindowSizeTypes.Normal;
			MainWindow.ForceReload();

			foreach (var child in ChildWindowList)
			{
				child.WindowState = WindowState.Normal;
				child.SetupInfo.SizeType = WindowInfo.WindowSizeTypes.Normal;
				MainWindow.ForceReload();
			}
		}

		public static void ToggleFullscreen()
		{
			if (MainWindow == null)
				return;

			if (MainWindow.SetupInfo.SizeType == WindowInfo.WindowSizeTypes.Fullscreen)
				SetNormal();
			else
				SetFullscreen();
		}

		public static void SetFSAALevel(int level)
		{
			if (MainWindow == null || MainWindow.SetupInfo.AntiAliasingFactor == level || RestartData != null)
				return;

			RestartData = new RestartInfo();
			RestartData.MainData = new RestartInfo.WinData();
			RestartData.MainData.NewInfo = MainWindow.SetupInfo;
			RestartData.MainData.NewInfo.AntiAliasingFactor = level;
			RestartData.MainData.Layers = MainWindow.Layers;

			foreach (var child in ChildWindowList)
			{
				RestartInfo.WinData dat = new RestartInfo.WinData();
				dat.NewInfo = child.SetupInfo;
				dat.NewInfo.AntiAliasingFactor = level;
				dat.Layers = child.Layers;
				RestartData.NewChildInfos.Add(child.ContextID, dat);
			}
		}

		internal static void RemakeWindows()
		{
			if (MainWindow == null)
				return;

			MainWindow.MakeCurrent();
			TextureManager.UnbindAll();

			foreach (var child in ChildWindowList)
			{
				child.MakeCurrent();
				TextureManager.UnbindAll();
			}

			MainWindow = null;
			ChildWindowList.Clear();
			Core.ReRun(RemakeCallback);
		}

		internal static void RemakeCallback()
		{
			if (RestartData == null || RestartData.MainData == null || RestartData.MainData.NewInfo == null)
				return;

			List<WindowInfo> children = new List<WindowInfo>();
			foreach (var child in RestartData.NewChildInfos)
				children.Add(child.Value.NewInfo);

			Init(RestartData.MainData.NewInfo, children);

			MainWindow.Layers = RestartData.MainData.Layers;

			for (int i = 0; i < RestartData.NewChildInfos.Count; i++)
			{
				ChildWindowList[i].Layers = RestartData.NewChildInfos[i].Layers;
			}

			RestartData = null;
		}

	}
}
