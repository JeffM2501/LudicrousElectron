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
			WindowManager.MainWindow?.Run();
			Running = false;

            TextureManager.Cleanup();
            SoundManager.Cleanup();
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
        }

        internal static void RenderChild(int childID)
        {

        }
	}
}
