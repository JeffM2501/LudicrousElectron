using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Engine.IO;

namespace LudicrousElectron.Assets.Providers
{
	public class PackAssetProvider : IAssetProvider
	{
		protected FileInfo BaseFile = null;

        protected Dictionary<string, FileInfo> TempFiles = new Dictionary<string, FileInfo>();

		protected class PackedAssetInfo
		{
			public string FileName = string.Empty;
			public int BufferOffset = 0;
			public int BufferSize = 0;
		}
		protected Dictionary<string, PackedAssetInfo> Assets = new Dictionary<string, PackedAssetInfo>();

		public PackAssetProvider (string fileName)
		{
			if (!File.Exists(fileName))
				return;

			BaseFile = new FileInfo(fileName);

			FileStream fs = null;
			try
			{
				fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				int version = StreamUtils.ReadInt(fs);

				switch(version)
				{
					case 0:
						int count = StreamUtils.ReadInt(fs);
						for(int i = 0; i < count; i++)
						{
							PackedAssetInfo info = new PackedAssetInfo();
							info.FileName = StreamUtils.ReadPString(fs);
							info.BufferOffset = StreamUtils.ReadInt(fs);
							info.BufferSize = StreamUtils.ReadInt(fs);

							if (info.BufferOffset > 0 && info.BufferSize > 0)
								Assets.Add(info.FileName, info);
						}
						break;
				}
			}
			catch (Exception)
			{
				Assets.Clear();
			}
			if (fs != null)
				fs.Close();
		}

        ~PackAssetProvider()
        {
            foreach (var tmp in TempFiles)
                tmp.Value.Delete();

            TempFiles.Clear();
        }

        public List<string> FindAssets(string searchPattern)
		{
			return new List<string>(Assets.Keys.ToArray());
		}

		public Stream GetAssetStream(string assetPath)
		{
			if (BaseFile == null || !BaseFile.Exists || !Assets.ContainsKey(assetPath))
				return null;

			var info = Assets[assetPath];

			MemoryStream ms = null;
			FileStream fs = null;
			try
			{
				fs = BaseFile.OpenRead();
				fs.Seek(info.BufferOffset, SeekOrigin.Begin);
				byte[] buffer = new byte[info.BufferSize];
				fs.Read(buffer, 0, info.BufferSize);
				ms = new MemoryStream(buffer);
			}
			catch (Exception)
			{
				Assets.Clear();
			}
			if (fs != null)
				fs.Close();

			return ms;
		}

        public string GetAssetFullPath(string assetPath)
        {
            if (TempFiles.ContainsKey(assetPath))
                return TempFiles[assetPath].FullName;

            Stream s = GetAssetStream(assetPath);
            if (s == null)
                return string.Empty;

            FileInfo tmp = new FileInfo(Path.GetTempFileName());
            FileStream fs = tmp.OpenWrite();

            byte[] buffer = new byte[1024 * 100];
            while (true)
            {
                int read = s.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, read);
                if (read < buffer.Length)
                    break;
            }

            fs.Close();
            s.Close();
            TempFiles.Add(assetPath, tmp);

            return tmp.FullName;
        }
    }
}
