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
		Stream GetAssetStream(string assetPath);
	}
}
