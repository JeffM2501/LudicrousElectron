using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Assets
{
	public interface IAssetProvider
	{
		List<string> FindAssets(string searchPattern);
		List<string> FindAssets(string pathStart, string searchPattern);
		Stream GetAssetStream(string assetPath);
        string GetAssetFullPath(string assetPath);
        bool AssetExists(string assetPath);
    }
}
