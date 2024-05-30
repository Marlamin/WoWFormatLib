using SereniaBLPLib;
using SixLabors.ImageSharp;
using System.IO;
using WoWFormatLib.FileProviders;

namespace WoWFormatLib.FileReaders
{
    public class BLPReader
    {
        public Image bmp;

        public void LoadBLP(uint fileDataID)
        {
            using (var blp = new BlpFile(FileProvider.OpenFile(fileDataID)))
            {
                bmp = blp.GetImage(0);
            }
        }

        public void LoadBLP(string filename)
        {
            using (var blp = new BlpFile(FileProvider.OpenFile(filename)))
            {
                bmp = blp.GetImage(0);
            }
        }

        public void LoadBLP(Stream file)
        {
            using (var blp = new BlpFile(file))
            {
                bmp = blp.GetImage(0);
            }
        }
    }
}
