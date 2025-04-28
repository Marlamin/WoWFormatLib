using BLPSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using WoWFormatLib.FileProviders;

namespace WoWFormatLib.FileReaders
{
    public class BLPReader
    {
        public Image bmp;

        public void LoadBLP(uint fileDataID)
        {
            using (var blp = new BLPFile(FileProvider.OpenFile(fileDataID)))
            {
                var pixels = blp.GetPixels(0, out var w, out var h);
                bmp = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixels, w, h);
            }
        }

        public void LoadBLP(string filename)
        {
            using (var blp = new BLPFile(FileProvider.OpenFile(filename)))
            {
                var pixels = blp.GetPixels(0, out var w, out var h);
                bmp = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixels, w, h);
            }
        }

        public void LoadBLP(Stream file)
        {
            using (var blp = new BLPFile(file))
            {
                var pixels = blp.GetPixels(0, out var w, out var h);
                bmp = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixels, w, h);
            }
        }
    }
}
