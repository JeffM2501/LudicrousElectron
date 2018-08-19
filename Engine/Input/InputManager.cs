using System;
using System.Collections.Generic;
using LudicrousElectron.Engine.Window;
using OpenTK;
using OpenTK.Input;

namespace LudicrousElectron.Engine.Input
{
	public static class InputManager
	{
		public static int PrimaryMouseButton = 0;
		public static int SecondaryMouseButton = 1;

		public class MouseFrameEventArgs : EventArgs
		{
			public MouseState CursorState = new MouseState();
			public int ContextID = -1;

			public bool PrimaryDown = false;
			public bool SecondaryDown = false;

			public bool PrimaryClick = false;
			public bool SecondaryClick = false;

			public Vector2 ScreenPosition = Vector2.Zero;
			public Vector2 CursorPostion = Vector2.Zero;
		}

		public static event EventHandler<MouseFrameEventArgs> PreProcessMouseInput = null;

		public static event EventHandler<MouseFrameEventArgs> ProcessMouseInput = null;


		private static MouseFrameEventArgs LastMouseState = null;

		internal static void PollInput(int currentContextID)
		{
			MouseFrameEventArgs args = new MouseFrameEventArgs();
			args.ContextID = currentContextID;
			args.CursorState = Mouse.GetCursorState();

			args.PrimaryDown = args.CursorState.IsButtonDown(MouseButton.Left + PrimaryMouseButton);
			args.SecondaryDown = args.CursorState.IsButtonDown(MouseButton.Left + SecondaryMouseButton);
			args.ScreenPosition = new Vector2(args.CursorState.X, args.CursorState.Y);

			var origin = WindowManager.MainWindow.PointToClient(new System.Drawing.Point(args.CursorState.X,args.CursorState.Y));

			args.CursorPostion = new Vector2(origin.X, WindowManager.MainWindow.Height - origin.Y);

			if (LastMouseState == null)
			{
				args.PrimaryClick = args.PrimaryDown;
				args.SecondaryClick = args.SecondaryDown;
			}
			else
			{
				args.PrimaryClick = args.PrimaryDown && !LastMouseState.PrimaryDown && !LastMouseState.PrimaryClick;
				args.SecondaryClick = args.SecondaryDown && !LastMouseState.SecondaryDown && !LastMouseState.SecondaryClick;
			}

			// let someone modify it, like for special input systems that must happen before actual processing
			PreProcessMouseInput?.Invoke(null, args);

			// now tell everyone to use the input
			ProcessMouseInput?.Invoke(null, args);

			LastMouseState = args;

		}
	}
}
