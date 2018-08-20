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
			if (searchPattern == string.Empty)
				searchPattern = "*.*";

			List<string> assetPaths = new List<string>();
			if (RootDir != null)
				SearchDir(RootDir, searchPattern, assetPaths);

			return assetPaths;
		}

		public virtual List<string> FindAssets(string pathStart, string searchPattern)
		{
			if (searchPattern == string.Empty)
				searchPattern = "*.*";

			List<string> assetPaths = new List<string>();
			if (RootDir != null)
			{
				DirectoryInfo subDir = new DirectoryInfo(Path.Combine(RootDir.FullName, pathStart));
				if (subDir.Exists)
					SearchDir(subDir, searchPattern, assetPaths);
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

			if (path.StartsWith("/"))
				path = path.Substring(1);

			if (Path.DirectorySeparatorChar == '\\')
				path = path.Replace("/", "\\");

			FileInfo file = new FileInfo(Path.Combine(RootDir.FullName, path));

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
