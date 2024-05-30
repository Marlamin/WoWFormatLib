using System.IO;
using WoWFormatLib.FileProviders;

namespace WoWFormatLib.FileReaders
{
    public class SKELReader
    {
        public void LoadSKEL(uint fileDataID)
        {
            using (var bin = new BinaryReader(FileProvider.OpenFile(fileDataID)))
            {
                var header = new string(bin.ReadChars(4));

            }
        }
    }
}
