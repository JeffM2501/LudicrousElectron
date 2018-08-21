using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Assets.Providers
{
	public class DirectoryAssetProvider : IAssetProvider
	{
		protected DirectoryInfo RootDir = null;

		public DirectoryAssetProvider(string path)
		{
			if (Directory.Exists(path))
				RootDir = new DirectoryInfo(path);
		}

		public DirectoryAssetProvider(DirectoryInfo dir)
		{
			if (dir != null && Directory.Exists(dir.FullName))
				RootDir = new DirectoryInfo(dir.FullName);
		}

		public virtual List<string> FindAssets(string searchPattern)
		{
            string actualPattern = string.IsNullOrEmpty(searchPattern) ? "*.*" : searchPattern;

			List<string> assetPaths = new List<string>();
			if (RootDir != null)
				SearchDir(RootDir, actualPattern, assetPaths);

			return assetPaths;
		}

		public virtual List<string> FindAssets(string pathStart, string searchPattern)
		{
            string actualPattern = string.IsNullOrEmpty(searchPattern) ? "*.*" : searchPattern;


            List<string> assetPaths = new List<string>();
			if (RootDir != null)
			{
				DirectoryInfo subDir = new DirectoryInfo(Path.Combine(RootDir.FullName, pathStart));
				if (subDir.Exists)
					SearchDir(subDir, actualPattern, assetPaths);
			}

			return assetPaths;
		}

		protected virtual void SearchDir(DirectoryInfo dir, string searchPattern, List<string> assetPaths)
		{
			foreach (var file in dir.GetFiles(searchPattern))
				assetPaths.Add(GetRelativePath(file.FullName));

			foreach (var subDir in dir.GetDirectories())
			{
				if (!subDir.Name.StartsWith("."))
					SearchDir(subDir, searchPattern, assetPaths);
			}
		}

		protected string GetRelativePath(string path)
		{
			if (RootDir == null)
				return path;

			string relPath = path.Substring(RootDir.FullName.Length);
			return relPath.Replace("\\", "/");
		}

		protected string GetAbsPath(string path)
		{
			if (RootDir == null)
				return path;

            string osPath = path;

			if (osPath.StartsWith("/"))
                osPath = osPath.Substring(1);

			if (Path.DirectorySeparatorChar == '\\')
                osPath = osPath.Replace("/", "\\");

			FileInfo file = new FileInfo(Path.Combine(RootDir.FullName, osPath));

			return file.FullName;
		}

		public virtual Stream GetAssetStream(string assetPath)
		{
			if (assetPath.Contains(".."))
				return null;

			string fullPath = GetAbsPath(assetPath);
			if (!File.Exists(fullPath))
				return null;

			return new FileInfo(fullPath).OpenRead();
		}

        public virtual string GetAssetFullPath(string assetPath)
        {
            if (assetPath.Contains(".."))
                return string.Empty;

            string fullPath = GetAbsPath(assetPath);
            if (!File.Exists(fullPath))
                return string.Empty;

            return fullPath;
        }
    }
}
