using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Assets.Providers;

namespace LudicrousElectron.Assets
{
	public static class AssetManager
	{
		// TODO, keep a cache of located items
		private static List<IAssetProvider> AssetProviders = new List<IAssetProvider>();

		public static void AddProvider(IAssetProvider provider)
		{
			AssetProviders.Insert(0,provider);
		}

		public static void AddDir(DirectoryInfo dir)
		{
			if (dir != null && dir.Exists)
				AddProvider(new DirectoryAssetProvider(dir));
		}

		public static List<string> FindAssets(string searchPattern)
		{
			List<string> assets = new List<string>();
			foreach (var provider in AssetProviders)
				assets.AddRange(provider.FindAssets(searchPattern).ToArray());

			return assets;
		}

		public static Stream GetAssetStream(string assetPath)
		{
			Stream outStream = null;
			foreach (var provider in AssetProviders)
			{
				outStream = provider.GetAssetStream(assetPath);
				if (outStream != null)
					return outStream;
			}
			return outStream;
		}

        public static string GetAssetFullPath(string assetPath)
        {
            string outPath = string.Empty;
            foreach (var provider in AssetProviders)
            {
                outPath = provider.GetAssetFullPath(assetPath);
                if (outPath != string.Empty)
                    return outPath;
            }

            return outPath;
        }
	}
}
