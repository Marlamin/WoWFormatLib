using System;
using System.IO;
using TACTSharp;

namespace WoWFormatLib.FileProviders
{
    public class TACTSharpFileProvider : IFileProvider
    {
        private BuildInstance build;
        public static bool IsTACTInit = false;
        public static string BuildName;

        public void InitTACT(BuildInstance instance)
        {
            build = instance;
            IsTACTInit = true;

            var splitName = build.BuildConfig.Values["build-name"][0].Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];
        }

        public void InitTACT(string program = "", string baseDir = "")
        {
            throw new NotImplementedException();
        }

        public bool FileExists(uint filedataid) => build.Root.FileExists(filedataid);

        public bool FileExists(string filename) => throw new NotImplementedException();

        public uint GetFileDataIdByName(string filename) => throw new NotImplementedException();

        public Stream OpenFile(uint filedataid) => new MemoryStream(build.OpenFileByFDID(filedataid));

        public Stream OpenFile(string filename) => throw new NotImplementedException();

        public void SetBuild(string build)
        {
            if (build != BuildName)
            {
                throw new Exception("Build mismatch, this TACT instance has build " + BuildName + " loaded but " + build + " was requested");
            }
        }
    }
}
