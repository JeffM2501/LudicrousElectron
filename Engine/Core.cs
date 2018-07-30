using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Engine.Collisions;
using LudicrousElectron.Engine.Graphics.Textures;
using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.Engine
{
    public static class Core
    {
		public class EngineTiming
		{
			public float Update = 0;
			public float Collision = 0;
			public float Render = 0;
			public float ServerUpdate = 0;
		}

		private static double GameSpeed = 1.0;
		private static bool Running = false;

		public static TextureManager Textures = new TextureManager();

		public static void Setup()
		{
			CollisionManager.Initalize();

			Running = true;
		}

        public static void Run()
        {
           WindowManager.MainWindow?.Run();
        }

        internal static void UpdateMain()
        {

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
