using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace WoWFormatLib.FileProviders
{
    public class WagoFileProvider : IFileProvider
    {
        private HttpClient Client = new();
        private string Build;
        private Dictionary<uint, string> Files = [];
        private Lock BuildLock = new();
        private Dictionary<uint, Lock> FileLocks = [];

        public void SetBuild(string build)
        {
            Build = build;

            if (File.Exists("cache/wago/" + Build + ".json"))
            {
                Console.WriteLine("Reading wago file list from cache");
                Files = System.Text.Json.JsonSerializer.Deserialize<Dictionary<uint, string>>(File.ReadAllText("cache/wago/" + Build + ".json"));
                return;
            }

            lock (BuildLock)
            {
                if (File.Exists("cache/wago/" + Build + ".json"))
                {
                    Console.WriteLine("Reading wago file list from cache");
                    Files = System.Text.Json.JsonSerializer.Deserialize<Dictionary<uint, string>>(File.ReadAllText("cache/wago/" + Build + ".json"));
                    return;
                }

                Console.WriteLine("Fetching file list from Wago");
                var response = Client.GetAsync("https://wago.tools/api/files?version=" + Build).Result;

                if (response.IsSuccessStatusCode)
                {
                    var fileList = response.Content.ReadAsStringAsync().Result;

                    if (!Directory.Exists("cache/wago/files"))
                        Directory.CreateDirectory("cache/wago/files");

                    File.WriteAllText("cache/wago/" + Build + ".json", fileList);

                    Files = System.Text.Json.JsonSerializer.Deserialize<Dictionary<uint, string>>(fileList);
                }
                else
                {
                    throw new Exception("Failed to fetch file list from Wago");
                }
            }
        }

        public bool FileExists(uint filedataid)
        {
            return Files.ContainsKey(filedataid);
        }

        public bool FileExists(string filename)
        {
            return Files.ContainsValue(filename);
        }

        public Stream OpenFile(uint filedataid)
        {
            if (!Directory.Exists("cache/wago/files/" + Build))
                Directory.CreateDirectory("cache/wago/files/" + Build);

            if (File.Exists("cache/wago/files/" + Build + "/" + filedataid))
                return new MemoryStream(File.ReadAllBytes("cache/wago/files/" + Build + "/" + filedataid));

            if (!FileLocks.ContainsKey(filedataid))
                FileLocks[filedataid] = new Lock();

            lock(FileLocks[filedataid])
            {
                // Check again if the file exists after acquiring the lock
                if (File.Exists("cache/wago/files/" + Build + "/" + filedataid))
                    return new MemoryStream(File.ReadAllBytes("cache/wago/files/" + Build + "/" + filedataid));

                var response = Client.GetAsync("https://wago.tools/api/casc/" + filedataid + "?version=" + Build).Result;

                if (response.IsSuccessStatusCode)
                {
                    using (var fileStream = File.Create("cache/wago/files/" + Build + "/" + filedataid))
                        response.Content.CopyToAsync(fileStream).Wait();
                }
                else
                {
                    throw new Exception("Failed to fetch file " + filedataid + " from Wago");
                }

                return new MemoryStream(File.ReadAllBytes("cache/wago/files/" + Build + "/" + filedataid));
            }
        }

        public Stream OpenFile(string filename)
        {
            throw new NotImplementedException();
        }
        public uint GetFileDataIdByName(string filename)
        {
            return Files.FirstOrDefault(x => x.Value == filename).Key;
        }
    }
}
