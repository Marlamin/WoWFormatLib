using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs;
using WoWFormatLib.Structs.WDT;

namespace WoWFormatLib.FileReaders
{
    public class DATReader
    {
        public DAT LoadDAT(uint fileDataID)
        {
            if (!FileProvider.FileExists(fileDataID))
            {
                throw new FileNotFoundException("DAT " + fileDataID + " not found!");
            }

            return LoadDAT(FileProvider.OpenFile(fileDataID));
        }

        public DAT LoadDAT(Stream datStream)
        {
            var datFile = new DAT();
            datFile.chunks = new();
            datFile.doodads = new();
            datFile.textures = new();

            using (var bin = new BinaryReader(datStream))
            {
                long position = 0;
                while (position < datStream.Length)
                {
                    datStream.Position = position;

                    var chunkName = (DATChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    position = datStream.Position + chunkSize;

                    switch (chunkName)
                    {
                        case DATChunks.MVER: // Version
                            datFile.version = bin.ReadUInt32();
                            break;
                        case DATChunks.AHDR: // Header
                            datFile.header = bin.Read<DATHeader>();
                            break;
                        case DATChunks.ALOC: // Location
                            datFile.location = bin.Read<DATLocation>();
                            break;
                        case DATChunks.ADOO: // Doodads
                            datFile.doodads.Add(new string(bin.ReadChars((int)chunkSize)));
                            break;
                        case DATChunks.ATEX: // Textures
                            datFile.textures.Add(new string(bin.ReadChars((int)chunkSize)));
                            break;
                        case DATChunks.AOCH: // ?
                        case DATChunks.AVTX: // Vertices
                        case DATChunks.ACVT: // Vertex colors
                        case DATChunks.ANRM: // Normals
                        case DATChunks.AFBO: // Flight bounds
                            // TODO
                            break;
                        case DATChunks.ACNK: // Chunk (MCNK equiv)
                            var datChunk = new DATChunk();
                            using (var chunkStream = new MemoryStream(bin.ReadBytes((int)chunkSize)))
                            using(var subbin = new BinaryReader(chunkStream))
                            {
                                if(chunkSize >= 0x40)
                                {
                                    datChunk.header = subbin.Read<DATMCNKHeader>();
                                }
                                while (subbin.BaseStream.Position < subbin.BaseStream.Length)
                                {
                                    var subChunkName = (DATChunks)subbin.ReadUInt32();
                                    var subChunkSize = subbin.ReadUInt32();

                                    switch (subChunkName)
                                    {
                                        case DATChunks.ALYR:
                                        case DATChunks.ASHD:
                                            subbin.BaseStream.Position += (uint)subChunkSize;
                                            break;
                                        default:
                                            var debugSubChunkName = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)subChunkName).Reverse().ToArray());
                                            Console.WriteLine(string.Format("Found unknown DAT subchunk at offset {1} \"{0}\" while we should've already read them all!", debugSubChunkName, position.ToString()));
                                            subbin.BaseStream.Position += (uint)subChunkSize;
                                            break;
                                    }
                                }
                            }

                            datFile.chunks.Add(datChunk);
                            break;
                        default:
                            var debugChunkName = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)chunkName).Reverse().ToArray());
                            Console.WriteLine(string.Format("Found unknown DAT chunk at offset {1} \"{0}\" while we should've already read them all!", debugChunkName, position.ToString()));

                            break;
                    }
                }
            }

            return datFile;
        }
    }
}
