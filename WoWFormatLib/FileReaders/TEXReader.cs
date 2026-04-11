using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.TEX;

namespace WoWFormatLib.FileReaders
{
    public class TEXReader
    {
        public TEXFile LoadTEX(string filename)
        {
            if (FileProvider.FileExists(filename))
            {
                using (var tex = FileProvider.OpenFile(filename))
                {
                    return ReadTEX(tex);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public TEXFile LoadTEX(uint fileDataID)
        {
            if (FileProvider.FileExists(fileDataID))
            {
                using (var tex = FileProvider.OpenFile(fileDataID))
                {
                    return ReadTEX(tex);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private TEXFile ReadTEX(Stream tex)
        {
            uint version = 0;
            var blobTextures = Array.Empty<BlobTexture>();
            var mipMapData = new List<byte[]>();
            var txmdOffsetsToIndex = new Dictionary<int, int>();
            var blobTextureDict = new Dictionary<int, BlobTexture>();
            var postTXBTPos = 0L;
            using (var bin = new BinaryReader(tex))
            {
                long position = 0;

                while (position < tex.Length)
                {
                    tex.Position = position;

                    var chunkName = (TEXChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    position = tex.Position + chunkSize;

                    switch (chunkName)
                    {
                        case TEXChunks.TXVR: // Version
                            version = bin.ReadUInt32();
                            break;
                        case TEXChunks.TXFN: // File Names
                            ReadTXFNChunk(bin, chunkSize); // Removed in BfA, not sure if we should keep it around for older versions?
                            break;
                        case TEXChunks.TXBT: // Blob Texture
                            blobTextures = ReadTXBTChunk(bin, chunkSize);
                            for (var i = 0; i < blobTextures.Length; i++)
                                blobTextureDict.Add((int)blobTextures[i].fileDataID, blobTextures[i]);
                            postTXBTPos = position;
                            break;
                        case TEXChunks.TXMD: // Mipmap Data
                            txmdOffsetsToIndex[((int)tex.Position - 8) - (int)postTXBTPos] = mipMapData.Count;
                            mipMapData.Add(bin.ReadBytes((int)chunkSize));
                            break;
                        default:
                            Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkName, position.ToString()));
                            break;
                    }
                }
            }

            return new TEXFile
            {
                version = version,
                blobTextures = blobTextureDict,
                mipMapData = [.. mipMapData],
                txmdOffsetsToIndex = txmdOffsetsToIndex
            };
        }

        private static BlobTexture[] ReadTXBTChunk(BinaryReader bin, uint size)
        {
            var blobTextures = new BlobTexture[size / 12];
            for (var i = 0; i < blobTextures.Length; i++)
            {
                blobTextures[i] = new BlobTexture
                {
                    fileDataID = bin.ReadUInt32(),
                    txmdOffset = bin.ReadUInt32(),
                    sizeX = bin.ReadByte(),
                    sizeY = bin.ReadByte(),
                    numMipMaps = (byte)(bin.ReadByte() & 0x7F),
                    dxtFormat = (byte)(bin.ReadByte() & 0x0F)
                };
            }
            return blobTextures;
        }

        private static void ReadTXFNChunk(BinaryReader bin, uint size)
        {
            var blpFilesChunk = bin.ReadBytes((int)size);

            var str = new StringBuilder();

            for (var i = 0; i < blpFilesChunk.Length; i++)
            {
                if (blpFilesChunk[i] == '\0')
                {
                    if (str.Length > 1)
                    {
                        str.Replace("..", ".");
                        str.Append(".blp"); //Filenames in TEX dont have have BLP extensions
                    }
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)blpFilesChunk[i]);
                }
            }
        }
    }
}
