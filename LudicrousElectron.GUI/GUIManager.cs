using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.Window;
using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.Engine.Graphics.Textures;

namespace LudicrousElectron.GUI
{
	public static class GUIManager
	{
		internal static Dictionary<Tuple<string, Color>, Material> MaterialCache = new Dictionary<Tuple<string, Color>, Material>();

		internal static Material GetMaterial(string texture, Color color)
		{
			Tuple<string, Color> id = new Tuple<string, Color>(texture, color);
			if (!MaterialCache.ContainsKey(id))
			{
				Material mat = new Material();
                mat.DefaultDiffuseFormat = TextureInfo.TextureFormats.Sprite;
				mat.DiffuseColor = color;
				mat.DiffuseName = texture;
				MaterialCache.Add(id, mat);
			}
			return MaterialCache[id];
		}

		internal static Material GetMaterial(TextureInfo texture, Color color)
		{
			Tuple<string, Color> id = new Tuple<string, Color>(texture.RelativeName, color);
			if (!MaterialCache.ContainsKey(id))
			{
				Material mat = new Material(color,texture);
				MaterialCache.Add(id, mat);
			}
			return MaterialCache[id];
		}

		public class ContextInfo
		{
			public Stack<Canvas> Canvases = new Stack<Canvas>();
			public GUIRenderLayer Layer = new GUIRenderLayer();

			public int ContextID = -1;
		}

		public static Dictionary<int, ContextInfo> Contexts = new Dictionary<int, ContextInfo>();

		static GUIManager()
		{
			WindowManager.WindowAdded += WindowManager_WindowAdded;
			WindowManager.WindowRemoved += WindowManager_WindowRemoved;
			WindowManager.WindowResized += WindowManager_WindowResized;

			if (WindowManager.Inited())
			{
				ContextInfo info = new ContextInfo();
				info.ContextID = WindowManager.MainWindowID;
                info.Layer.CurrentContext = WindowManager.MainWindow;

                Contexts.Add(info.ContextID, info);

				foreach (var window in WindowManager.ChildWindows)
				{
					info = new ContextInfo();
					info.ContextID = window.ContextID;
                    info.Layer.CurrentContext = window;
					Contexts.Add(info.ContextID, info);
				}
			}
		}

		private static void WindowManager_WindowResized(object sender, EventArgs e)
		{
			WindowManager.Window win = sender as WindowManager.Window;
			if (win == null || !Contexts.ContainsKey(win.ContextID) || Contexts[win.ContextID].Canvases.Count == 0)
				return;

			Contexts[win.ContextID].Canvases.Peek().Resize();
		}

		private static void WindowManager_WindowRemoved(object sender, EventArgs e)
		{
			WindowManager.Window win = sender as WindowManager.Window;
			if (win == null || !Contexts.ContainsKey(win.ContextID))
				return;

			Contexts.Remove(win.ContextID);
		}

		private static void WindowManager_WindowAdded(object sender, EventArgs e)
		{
			WindowManager.Window win = sender as WindowManager.Window;
			if (win == null)
				return;

			ContextInfo info = new ContextInfo();
			info.ContextID = win.ContextID;
            info.Layer.CurrentContext = win;
			Contexts.Add(win.ContextID, info);
		}

		public static GUIRenderLayer GetGUILayer(int context = -1)
		{
			if (context < 0)
				context = WindowManager.MainWindowID;

			if (!Contexts.ContainsKey(context))
				return null;

			return Contexts[context].Layer;
		}

		public static void PushCanvas(Canvas canvas, int context = -1)
		{
			if (context < 0)
				context = WindowManager.MainWindowID;

			if (!Contexts.ContainsKey(context))
				return;
            canvas.BoundWindow = Contexts[context].Layer.CurrentContext;

            Contexts[context].Canvases.Push(canvas);
			Contexts[context].Layer.ChangeCanvas(canvas);
		}

		public static void PopCanvas(int context = -1)
		{
			if (context < 0)
				context = WindowManager.MainWindowID;

			if (!Contexts.ContainsKey(context) || Contexts[context].Canvases.Count == 0)
				return;

			Contexts[context].Canvases.Pop();
			Contexts[context].Layer.ChangeCanvas(Contexts[context].Canvases.Peek());
		}
	}
}
