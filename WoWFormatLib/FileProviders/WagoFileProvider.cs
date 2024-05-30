using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace WoWFormatLib.FileProviders
{
    public class WagoFileProvider : IFileProvider
    {
        private HttpClient Client = new();
        private string Build;
        private Dictionary<uint, string> Files = [];

        public void SetBuild(string build)
        {
            Build = build;

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
            if (Files.ContainsKey(filedataid))
            {
                if (!Directory.Exists("cache/wago/files/" + Build))
                    Directory.CreateDirectory("cache/wago/files/" + Build);

                if (File.Exists("cache/wago/" + Build + "/" + filedataid))
                    return File.OpenRead("cache/wago/files/" + Build + "/" + filedataid);

                var response = Client.GetAsync("https://wago.tools/api/casc/" + filedataid + "?version=" + Build).Result;

                if (response.IsSuccessStatusCode)
                {
                    using (var fileStream = File.Create("cache/wago/files/" + Build + "/" + filedataid))
                    {
                        response.Content.CopyToAsync(fileStream).Wait();
                    }
                        
                    return File.OpenRead("cache/wago/files/" + Build + "/" + filedataid);
                }
                else
                {
                    throw new Exception("Failed to fetch file " + filedataid + " from Wago");
                }
            }
            else
            {
                throw new FileNotFoundException("File with filedataid " + filedataid + " not found");
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
