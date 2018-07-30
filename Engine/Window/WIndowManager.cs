using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.RenderChain;
using LudicrousElectron.Types;

using OpenTK;
using OpenTK.Graphics;

namespace LudicrousElectron.Engine.Window
{
	public static class WindowManager
	{
		public class Window : GameWindow
		{
			public Window(int width, int height, GraphicsMode mode, string title, GameWindowFlags options) : base(width, height, mode, title, options) { }

			public IRenderable RootLayer = RenderLayer.DefaultLayer;
			public virtual void Render() { }

			public List<Window> Children = new List<Window>();
		}

		private static Window MainWindow = null;

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

			return true;
		}
	}
}
