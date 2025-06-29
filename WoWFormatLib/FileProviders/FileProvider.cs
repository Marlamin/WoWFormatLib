using System.Collections.Generic;
using System.IO;

namespace WoWFormatLib.FileProviders
{
    public static class FileProvider
    {
        private static Dictionary<string, IFileProvider> Providers = [];
        private static string DefaultBuild;
        public static bool HasProvider(string build)
        {
            return Providers.ContainsKey(build);
        }

        public static void SetDefaultBuild(string build)
        {
            DefaultBuild = build;
        }

        public static void SetProvider(IFileProvider provider, string build)
        {
            // Remove existing provider if it is Wago and the new one is CASC
            if (Providers.TryGetValue(build, out var existingProvider))
            {
                if (existingProvider is WagoFileProvider && provider is CASCFileProvider)
                    Providers.Remove(build);
            }

            Providers.TryAdd(build, provider);

            // If we don't have a build set, set it to the first one we get
            if (DefaultBuild == null)
                DefaultBuild = build;
        }

        private static IFileProvider GetBestProviderForBuild(string build = null)
        {
            if (build == null)
                build = DefaultBuild;

            if (Providers.TryGetValue(build, out IFileProvider value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public static bool FileExists(uint filedataid, string build = null)
        {
            var currentProvider = GetBestProviderForBuild(build);

            if (currentProvider == null)
                throw new System.Exception("No file provider set");

            return currentProvider.FileExists(filedataid);
        }

        public static bool FileExists(string filename, string build = null)
        {
            var currentProvider = GetBestProviderForBuild(build);

            if (currentProvider == null)
                throw new System.Exception("No file provider set");

            return currentProvider.FileExists(filename);
        }

        public static Stream OpenFile(uint filedataid, string build = null)
        {
            var currentProvider = GetBestProviderForBuild(build);

            if (currentProvider == null)
                throw new System.Exception("No file provider set");

            return currentProvider.OpenFile(filedataid);
        }

        public static Stream OpenFile(string filename, string build = null)
        {
            var currentProvider = GetBestProviderForBuild(build);

            if (currentProvider == null)
                throw new System.Exception("No file provider set");

            return currentProvider.OpenFile(filename);
        }

        public static Stream OpenFile(byte[] cKey, string build = null)
        {
            var currentProvider = GetBestProviderForBuild(build);

            if (currentProvider == null)
                throw new System.Exception("No file provider set");

            return currentProvider.OpenFile(cKey);
        }

        public static uint GetFileDataIdByName(string filename, string build = null)
        {
            var currentProvider = GetBestProviderForBuild(build);

            if (currentProvider == null)
                throw new System.Exception("No file provider set");

            return currentProvider.GetFileDataIdByName(filename);
        }
    }
}
