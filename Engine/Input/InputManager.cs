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

		public class LogicalButtonState
		{
			public bool PrimaryDown = false;
			public bool SecondaryDown = false;

			public bool PrimaryClick = false;
			public bool SecondaryClick = false;

            public bool AnyButtonIsDown()
            {
                return PrimaryClick || SecondaryClick || PrimaryDown || SecondaryDown;
            }
		}

		public class MouseFrameEventArgs : EventArgs
		{
			public MouseState CursorState = new MouseState();
			public int ContextID = -1;

			public LogicalButtonState Buttons = new LogicalButtonState();

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

			args.Buttons.PrimaryDown = args.CursorState.IsButtonDown(MouseButton.Left + PrimaryMouseButton);
			args.Buttons.SecondaryDown = args.CursorState.IsButtonDown(MouseButton.Left + SecondaryMouseButton);
			args.ScreenPosition = new Vector2(args.CursorState.X, args.CursorState.Y);

			var origin = WindowManager.MainWindow.PointToClient(new System.Drawing.Point(args.CursorState.X,args.CursorState.Y));

			args.CursorPostion = new Vector2(origin.X, WindowManager.MainWindow.Height - origin.Y);

			if (LastMouseState == null)
			{
				args.Buttons.PrimaryClick = args.Buttons.PrimaryDown;
				args.Buttons.SecondaryClick = args.Buttons.SecondaryDown;
			}
			else
			{
				args.Buttons.PrimaryClick = args.Buttons.PrimaryDown && !LastMouseState.Buttons.PrimaryDown && !LastMouseState.Buttons.PrimaryClick;
				args.Buttons.SecondaryClick = args.Buttons.SecondaryDown && !LastMouseState.Buttons.SecondaryDown && !LastMouseState.Buttons.SecondaryClick;
			}

			// let someone modify it, like for special input systems that must happen before actual processing
			PreProcessMouseInput?.Invoke(null, args);

			// now tell everyone to use the input
			ProcessMouseInput?.Invoke(null, args);

			LastMouseState = args;

		}
	}
}
