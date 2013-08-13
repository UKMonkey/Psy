using System;
using System.Collections.Generic;
using System.IO;
using Psy.Core.Logging;

namespace Psy.Core.FileSystem
{
    public static class Lookup
    {
        private static readonly List<string> AssetDirectories = new List<string> { "." };

        public static void AddPath(string path, bool recurse = false)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);

            if (!AssetDirectories.Contains(path))
            {
                AssetDirectories.Add(path);
            }

            if (!recurse)
                return;

            var pathInfo = new DirectoryInfo(path);

            foreach (var directoryInfo in pathInfo.GetDirectories())
            {
                try
                {
                    AddPath(directoryInfo.FullName, true);
                }
                catch (Exception e)
                {
                    Logger.Write(string.Format("Unable to add {0} to the list of data directories", directoryInfo.FullName));
                    Logger.WriteException(e);
                }
            }
        }

        public static string GetFilePath(string filename)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        public static string GetAssetPath(string filename)
        {
            foreach (var assetDirectory in AssetDirectories)
            {
                var testPath = Path.Combine(assetDirectory, filename);

                var fileInfo = new FileInfo(testPath);
                if (!fileInfo.Exists)
                    continue;

                return testPath;
            }

            throw new FileNotFoundException(string.Format("No such file {0}", filename));
        }

        public static bool AssetExists(string filename)
        {
            foreach (var assetDirectory in AssetDirectories)
            {
                var testPath = Path.Combine(assetDirectory, filename);

                var fileInfo = new FileInfo(testPath);
                if (!fileInfo.Exists)
                    continue;

                return true;
            }

            return false;
        }

        public static bool AssetExists(string filename, out string path)
        {
            foreach (var assetDirectory in AssetDirectories)
            {
                var testPath = Path.Combine(assetDirectory, filename);

                var fileInfo = new FileInfo(testPath);
                if (!fileInfo.Exists)
                    continue;

                path = testPath;
                return true;
            }

            path = null;
            return false;
        }

        public static void DumpPaths()
        {
            Logger.Write("Filesystem paths:", LoggerLevel.Debug);
            foreach (var assetDirectory in AssetDirectories)
            {
                Logger.Write(assetDirectory, LoggerLevel.Debug);
            }
        }
    }
}
