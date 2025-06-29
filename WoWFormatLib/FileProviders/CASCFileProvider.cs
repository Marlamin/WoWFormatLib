using CASCLib;
using System;
using System.IO;

namespace WoWFormatLib.FileProviders
{
    public class CASCFileProvider : IFileProvider
    {
        private static CASCHandler cascHandler;
        public static bool IsCASCInit = false;
        public static string BuildName;

        public void InitCasc(CASCHandler cascHandler)
        {
            CASCFileProvider.cascHandler = cascHandler;
            IsCASCInit = true;
        }

        // Handle via CASCLib
        public void InitCasc(BackgroundWorkerEx worker = null, string basedir = null, string program = "wowt", LocaleFlags locale = LocaleFlags.enUS)
        {
            CASCConfig.LoadFlags &= ~(LoadFlags.Download | LoadFlags.Install);
            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;
            CASCConfig.ThrowOnMissingDecryptionKey = false;

            if (basedir == null)
            {
                Console.WriteLine("Initializing CASC from web for program " + program);
                cascHandler = CASCHandler.OpenOnlineStorage(program, "eu", worker);
            }
            else
            {
                basedir = basedir.Replace("_retail_", "").Replace("_ptr_", "");
                Console.WriteLine("Initializing CASC from local disk with basedir " + basedir + " and program " + program);
                cascHandler = CASCHandler.OpenLocalStorage(basedir, program, worker);
            }

            var splitName = cascHandler.Config.BuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split('_')[0] + "." + splitName[0];

            cascHandler.Root.SetFlags(locale);

            IsCASCInit = true;
        }

        public void SetBuild(string build)
        {
            if(build != BuildName)
            {
                throw new Exception("Build mismatch, this CASC instance has build " + BuildName + " loaded but " + build + " was requested");
            }
        }

        public uint GetFileDataIdByName(string filename)
        {
            return (uint)(cascHandler.Root as WowRootHandler)?.GetFileDataIdByName(filename);
        }

        public Stream OpenFile(string filename)
        {
            return cascHandler.OpenFile(filename);
        }

        public Stream OpenFile(uint filedataid)
        {
            return cascHandler.OpenFile((int)filedataid);
        }

        public bool FileExists(string filename)
        {
            return cascHandler.FileExists(filename);
        }

        public bool FileExists(uint filedataid)
        {
            return cascHandler.FileExists((int)filedataid);
        }

        public Stream OpenFile(byte[] cKey)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(byte[] cKey)
        {
            throw new NotImplementedException();
        }
    }
}
