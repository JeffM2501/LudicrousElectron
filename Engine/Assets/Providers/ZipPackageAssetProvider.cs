using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using LudicrousElectron.Assets;
using LudicrousElectron.Engine.IO;

namespace LudicrousElectron.Assets.Providers
{
    public class ZipPackageAssetProvider : IAssetProvider
    {
        protected FileInfo BaseFile = null;

        protected Dictionary<string, FileInfo> TempFiles = new Dictionary<string, FileInfo>();

        protected ZipArchive Zip;

        protected class PackedAssetInfo
        {
            public string FileName = string.Empty;
            public int BufferOffset = 0;
            public int BufferSize = 0;
        }
        protected Dictionary<string, ZipArchiveEntry> Assets = new Dictionary<string, ZipArchiveEntry>();

        public ZipPackageAssetProvider(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            BaseFile = new FileInfo(fileName);

            try
            {
                Zip = new ZipArchive(BaseFile.OpenRead(), ZipArchiveMode.Read, false);

                foreach (var item in Zip.Entries)
                {
                    Assets.Add(item.FullName, item);
                }
            }
            catch (Exception)
            {
                Assets.Clear();
            }
        }

        ~ZipPackageAssetProvider()
        {
            Assets.Clear();

            foreach (var tmp in TempFiles)
            {
                try
                {
                    if (File.Exists(tmp.Value.FullName))
                        tmp.Value.Delete();
                }
                catch (Exception)
                {
                    // we tried to clean up, let it go.
                }
            }

            TempFiles.Clear();

            Zip.Dispose();
        }

        public List<string> FindAssets(string searchPattern)
        {
            string actaulPattern = searchPattern;
            if (!string.IsNullOrEmpty(actaulPattern))
                actaulPattern = actaulPattern.Substring(1).ToUpperInvariant();
            else
                actaulPattern = string.Empty;

            List<string> assets = new List<string>();
            foreach (var asset in Assets.Keys)
            {
                if (string.IsNullOrEmpty(actaulPattern) || Path.GetExtension(asset).ToUpperInvariant() == actaulPattern)
                    assets.Add(asset);
            }
            return assets;
        }

        public virtual List<string> FindAssets(string pathStart, string searchPattern)
        {
            string actaulPattern = searchPattern;
            if (!string.IsNullOrEmpty(actaulPattern))
                actaulPattern = actaulPattern.Substring(1).ToUpperInvariant();
            else
                actaulPattern = string.Empty;

            List<string> assets = new List<string>();
            foreach (var asset in Assets.Keys)
            {
                if (pathStart != null && !asset.ToUpperInvariant().StartsWith(pathStart.ToUpperInvariant()))
                    continue;

                if (string.IsNullOrEmpty(actaulPattern) || Path.GetExtension(asset).ToUpperInvariant() == actaulPattern)
                    assets.Add(asset);
            }
            return assets;
        }

        public Stream GetAssetStream(string assetPath)
        {
            if (BaseFile == null || !BaseFile.Exists || !Assets.ContainsKey(assetPath))
                return null;

            var info = Assets[assetPath];
            try
            {
                return info.Open();
            }
            catch (Exception)
            {
                // if it can't be opened, just give em back a null.
            }

            return null;
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

        public bool AssetExists(string assetPath)
        {
            if (BaseFile == null || !BaseFile.Exists || !Assets.ContainsKey(assetPath))
                return false;

            return true;
        }
    }
}
