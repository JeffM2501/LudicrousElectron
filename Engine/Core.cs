using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Engine.Audio;
using LudicrousElectron.Engine.Collisions;
using LudicrousElectron.Engine.Graphics.Textures;
using LudicrousElectron.Engine.Window;

using OpenTK.Input;

namespace LudicrousElectron.Engine
{
    public static class Core
    {
		private static bool Running = false;

		internal delegate void RerunCallback();

		private static RerunCallback RerunFunciton = null;
		private static bool IsRerun = false;

		public static void Setup()
		{
			SoundManager.Setup();


			CollisionManager.Initalize();
            TextureManager.Startup();

            Running = true;
		}

        public static void Run()
		{
			Running = true;

			bool reallyDone = false;
			while (!reallyDone)
			{
				WindowManager.MainWindow?.Run();

				if (IsRerun)
				{
					RerunFunciton?.Invoke();
					IsRerun = false;
					RerunFunciton = null;
				}
				else
					reallyDone = true;
			}
			
			Running = false;

            TextureManager.Cleanup();
            SoundManager.Cleanup();
        }

		internal static void ReRun(RerunCallback callback)
		{
			RerunFunciton = callback;
			IsRerun = true;
		}

		public static void Exit()
		{
			if (!Running)
				return;

			WindowManager.MainWindow.Exit();
		}

		public static bool IsRunning()
		{
			return Running;
		}

        internal static void UpdateMain()
        {
            SoundManager.Update();
        }

        internal static void UpdateChild(int childID)
        {
		}

        internal static void RenderMain()
        {
			TextureManager.CheckForPurge();

		}

        internal static void RenderChild(int childID)
        {

        }
	}
}
