using System;
using System.IO;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.ANIM;

namespace WoWFormatLib.FileReaders
{
    public class ANIMReader
    {
        public void LoadAnim(string filename)
        {
            LoadAnim(FileProvider.GetFileDataIdByName(Path.ChangeExtension(filename, "anim")));
        }

        public void LoadAnim(uint fileDataID)
        {
            using (var bin = new BinaryReader(FileProvider.OpenFile(fileDataID)))
            {
                long position = 0;

                while (position < bin.BaseStream.Length)
                {
                    bin.BaseStream.Position = position;

                    var chunkName = (ANIMChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    position = bin.BaseStream.Position + chunkSize;

                    switch (chunkName)
                    {
                        case ANIMChunks.AFM2:
                        case ANIMChunks.AFSA:
                        case ANIMChunks.AFSB:
                            break;
                        default:
                            Console.WriteLine(string.Format("{2} Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkName, position, fileDataID));
                            break;
                    }
                }
            }

        }
    }
}
