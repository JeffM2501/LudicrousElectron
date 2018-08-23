using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.Engine.Window;
using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.Engine.Graphics.Textures;
using LudicrousElectron.Engine.Input;

namespace LudicrousElectron.GUI
{
    public class GUIMaterial
    {
        public Tuple<string, Color> ID = new Tuple<string, Color>(string.Empty, Color.White);

        public string Texture { get { return ID.Item1; } set { ID = new Tuple<string, Color>(value, ID.Item2); }}
        public System.Drawing.Color Color { get { return ID.Item2; } set { ID = new Tuple<string, Color>(ID.Item1,value); } }

        public GUIMaterial() { }
        public GUIMaterial(string texture, System.Drawing.Color color)
        {
            ID = new Tuple<string, Color>(texture, color);
        }
    }

	public static class GUIManager
	{
		internal static Dictionary<Tuple<string, Color>, Material> MaterialCache = new Dictionary<Tuple<string, Color>, Material>();

		internal static Material GetMaterial(GUIMaterial matDef)
		{
			if (!MaterialCache.ContainsKey(matDef.ID))
			{
				Material mat = new Material();
                mat.DefaultDiffuseFormat = TextureInfo.TextureFormats.Sprite;
				mat.DiffuseColor = matDef.Color;
				mat.DiffuseName = matDef.Texture;
				MaterialCache.Add(matDef.ID, mat);
			}
			return MaterialCache[matDef.ID];
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

		public static Dictionary<int, ContextInfo> Contexts { get; private set; } = new Dictionary<int, ContextInfo>();

		static GUIManager()
		{
			WindowManager.WindowAdded += WindowManager_WindowAdded;
			WindowManager.WindowRemoved += WindowManager_WindowRemoved;
			WindowManager.WindowResized += WindowManager_WindowResized;

			InputManager.ProcessMouseInput += InputManager_ProcessMouseInput;

			if (WindowManager.Inited())
			{
				ContextInfo info = new ContextInfo();
				info.ContextID = WindowManager.MainWindowID;
                info.Layer.CurrentContextID = WindowManager.MainWindow.ContextID;

                Contexts.Add(info.ContextID, info);

				foreach (var window in WindowManager.ChildWindows)
				{
					info = new ContextInfo();
					info.ContextID = window.ContextID;
                    info.Layer.CurrentContextID = window.ContextID;
					Contexts.Add(info.ContextID, info);
				}
			}
		}

		private static void InputManager_ProcessMouseInput(object sender, InputManager.MouseFrameEventArgs e)
		{
			if (!Contexts.ContainsKey(e.ContextID) || Contexts[e.ContextID].Layer == null|| Contexts[e.ContextID].Canvases.Count ==0)
				return;

			Contexts[e.ContextID].Layer.HandleMouseInput(e);
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
            info.Layer.CurrentContextID = win.ContextID;
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

			canvas.BoundWindowID = Contexts[context].Layer.CurrentContext.ContextID;

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

		public static T PeekCanvas<T>(int context = -1) where T : Canvas
		{
			if (context < 0)
				context = WindowManager.MainWindowID;

			if (!Contexts.ContainsKey(context) || Contexts[context].Canvases.Count == 0)
				return null;

			return Contexts[context].Canvases.Peek() as T;
		}

        public static void Reset()
        {
            foreach (var context in Contexts.Values)
                context.Layer.Reset();
        }
	}
}
