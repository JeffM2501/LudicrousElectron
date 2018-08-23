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

		public static List<string> FindAssets(string startPath, string searchPattern)
		{
			List<string> assets = new List<string>();
			foreach (var provider in AssetProviders)
				assets.AddRange(provider.FindAssets(startPath, searchPattern).ToArray());

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

        public static string GetAssetText(string assetPath)
        {
            Stream s = GetAssetStream(assetPath);
            if (s == null)
                return string.Empty;

            StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();
        }

        public static string[] GetAssetLines(string assetPath)
        {
            List<string> lines = new List<string>();
            Stream s = GetAssetStream(assetPath);
            if (s == null)
                return lines.ToArray();

            StreamReader reader = new StreamReader(s);
            while (!reader.EndOfStream)
                lines.Add(reader.ReadLine());

            reader.Close();
            return lines.ToArray();
        }

        public static string GetAssetFullPath(string assetPath)
        {
            string outPath = string.Empty;
            foreach (var provider in AssetProviders)
            {
                outPath = provider.GetAssetFullPath(assetPath);
                if (!string.IsNullOrEmpty(outPath))
                    return outPath;
            }

            return outPath;
        }

        public static bool AssetExists(string assetPath)
        {
            foreach (var provider in AssetProviders)
            {
                if (provider.AssetExists(assetPath))
                    return true;
            }

            return false;
        }
    }
}
