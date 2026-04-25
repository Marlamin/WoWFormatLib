using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.ADT;
using WoWFormatLib.Structs.WDT;
using WoWFormatLib.Utils;

namespace WoWFormatLib.FileReaders
{
    public class ADTReader
    {
        public ADT adtfile;
        public uint rootFileDataID;
        private Structs.WDT.WDT wdt;
        public enum ADTType
        {
            Unknown,
            Root,
            Object0,
            Object1,
            Texture0,
            Texture1, // Deprecated
            LevelOfDetail
        }

        public enum ADTVersion
        {
            Unknown,
            Modern,
            WotLK
        }

        /* ROOT */
        /// <param name="wdtFile">WDT file, required to load split ADTs and load MCAL correctly</param>
        /// <param name="tileX">Tile X coordinate</param>
        /// <param name="tileY" >Tile Y coordinate</param>
        /// <param name="loadSecondaryADTs">Load secondary ADTs (OBJ0 and TEX0)</param>
        /// <param name="internalMapName">Internal map name, required for filename based ADT loading</param>
        /// <exception cref="FileNotFoundException"></exception>
        public uint LoadADT(Structs.WDT.WDT? wdtFile, byte tileX, byte tileY, bool loadSecondaryADTs = true, string internalMapName = "")
        {
            adtfile.x = tileX;
            adtfile.y = tileY;

            uint rootFileDataID;
            uint tex0FileDataID;
            uint obj0FileDataID;

            if (wdtFile.HasValue)
            {
                wdt = wdtFile.Value;

                if (!wdt.mphd.flags.HasFlag(Structs.WDT.MPHDFlags.wdt_has_maid))
                {
                    if (string.IsNullOrEmpty(internalMapName))
                    {
                        throw new Exception("Require internal map name for filename based ADT loading!");
                    }

                    // Still filename based
                    rootFileDataID = FileProvider.GetFileDataIdByName("world/maps/" + internalMapName + "/" + internalMapName + "_" + tileX + "_" + tileY + ".adt");
                    obj0FileDataID = FileProvider.GetFileDataIdByName("world/maps/" + internalMapName + "/" + internalMapName + "_" + tileX + "_" + tileY + "_obj0.adt");
                    tex0FileDataID = FileProvider.GetFileDataIdByName("world/maps/" + internalMapName + "/" + internalMapName + "_" + tileX + "_" + tileY + "_tex0.adt");
                }
                else
                {
                    // ID based
                    rootFileDataID = wdt.tileFiles[(tileX, tileY)].rootADT;
                    obj0FileDataID = wdt.tileFiles[(tileX, tileY)].obj0ADT;
                    tex0FileDataID = wdt.tileFiles[(tileX, tileY)].tex0ADT;
                }
            }
            else
            {
                throw new Exception("Need WDT file for filename based ADT/correct MCAL loading!");
            }

            if (!FileProvider.FileExists(rootFileDataID) || !FileProvider.FileExists(obj0FileDataID) || !FileProvider.FileExists(tex0FileDataID))
            {
                throw new FileNotFoundException("One or more ADT files for ADT " + rootFileDataID + " could not be found.");
            }

            ReadRootFile(rootFileDataID, wdt.mphd.flags);

            if (loadSecondaryADTs)
            {
                using (var adtobj0 = FileProvider.OpenFile(obj0FileDataID))
                {
                    ReadObjFile(adtobj0);
                }

                using (var adttex0 = FileProvider.OpenFile(tex0FileDataID))
                {
                    ReadTexFile(adttex0, wdt.mphd.flags);
                }
            }

            return rootFileDataID;
        }

        public uint LoadADT(MPHDFlags wdtMPHDFlags, uint rootFileDataID, uint obj0FileDataID = 0, uint tex0FileDataID = 0, bool loadSecondaryADTs = true)
        {
            ReadRootFile(rootFileDataID, wdtMPHDFlags);

            if (loadSecondaryADTs)
            {
                if (!FileProvider.FileExists(obj0FileDataID))
                    throw new FileNotFoundException("OBJ0 ADT file " + obj0FileDataID + " could not be found.");

                using (var adtobj0 = FileProvider.OpenFile(obj0FileDataID))
                    ReadObjFile(adtobj0);

                if (!FileProvider.FileExists(tex0FileDataID))
                    throw new FileNotFoundException("TEX0 ADT file " + tex0FileDataID + " could not be found.");

                using (var adttex0 = FileProvider.OpenFile(tex0FileDataID))
                    ReadTexFile(adttex0, wdtMPHDFlags);
            }

            return rootFileDataID;
        }

        /// <summary>
        /// Loads any type of ADT file by detecting the filetype first and then handing it to the right function. Only use this if you don't know the type of ADT file you're loading.
        /// </summary>
        /// <param name="fileDataID"></param>
        /// <param name="wdtMPHDFlags"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void LoadADT(uint fileDataID, MPHDFlags wdtMPHDFlags)
        {
            if (!FileProvider.FileExists(fileDataID))
                throw new FileNotFoundException("ADT file " + fileDataID + " could not be found.");

            var adtVersion = ADTVersion.Modern;
            var adtType = ADTType.Unknown;

            using (var adt = FileProvider.OpenFile(fileDataID))
            using (var bin = new BinaryReader(adt))
            {
                var chunkName = (ADTChunks)bin.ReadUInt32();
                if (chunkName != ADTChunks.MVER)
                    throw new Exception("Invalid ADT file, expected MVER chunk at the beginning!");
                bin.ReadUInt32();
                adtfile.version = bin.ReadUInt32();

                long position = adt.Position;

                while (position < adt.Length)
                {
                    adt.Position = position;
                    chunkName = (ADTChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();
                    position = adt.Position + chunkSize;

                    var chunkNameBytes = BitConverter.GetBytes((uint)chunkName);
                    Array.Reverse(chunkNameBytes);
                    //var chunkNameString = Encoding.ASCII.GetString(chunkNameBytes);
                    //Console.WriteLine(chunkNameString);

                    if (chunkName == ADTChunks.MCIN) // Only exists in WotLK ADTs and WotLK has no other types so bail
                    {
                        adtType = ADTType.Root;
                        adtVersion = ADTVersion.WotLK;
                        break;
                    }

                    if (chunkName == ADTChunks.MHDR || chunkName == ADTChunks.MH2O || chunkName == ADTChunks.MFBO)
                    {
                        adtType = ADTType.Root;
                        break;
                    }
                    else if (chunkName == ADTChunks.MMDX || chunkName == ADTChunks.MMID || chunkName == ADTChunks.MWMO || chunkName == ADTChunks.MWID || chunkName == ADTChunks.MODF || chunkName == ADTChunks.MDDF)
                    {
                        adtType = ADTType.Object0;
                        break;
                    }
                    else if (chunkName == ADTChunks.MLDD || chunkName == ADTChunks.MLDX)
                    {
                        adtType = ADTType.Object1;
                    }
                    else if (chunkName == ADTChunks.MTEX || chunkName == ADTChunks.MTXF || chunkName == ADTChunks.MTXP || chunkName == ADTChunks.MDID || chunkName == ADTChunks.MHID || chunkName == ADTChunks.MCLY)
                    {
                        adtType = ADTType.Texture0;
                        break;
                    }
                    else if (chunkName == ADTChunks.MLHD || chunkName == ADTChunks.MLVH || chunkName == ADTChunks.MLVI)
                    {
                        adtType = ADTType.LevelOfDetail;
                        break;
                    }
                }

                adt.Position = 0;
                switch (adtType)
                {
                    case ADTType.Root:
                        ReadRootFile(adt, wdtMPHDFlags, adtVersion);
                        break;
                    case ADTType.Object0:
                        ReadObjFile(adt);
                        break;
                    case ADTType.Texture0:
                        ReadTexFile(adt, wdtMPHDFlags);
                        break;
                    case ADTType.Object1:
                        break; // NYI
                    case ADTType.LevelOfDetail:
                        var lodReader = new LODADTReader();
                        lodReader.LoadLODADT(fileDataID);
                        adtfile.lod = lodReader.lodadt;
                        break;
                    default:
                        throw new Exception("Could not detect type of ADT file!");
                }
            }
        }

        public void ReadRootFile(uint rootFileDataID, MPHDFlags wdtMPHDFlags)
        {
            if (!FileProvider.FileExists(rootFileDataID))
            {
                throw new FileNotFoundException("Root ADT file " + rootFileDataID + " could not be found.");
            }

            using (var adt = FileProvider.OpenFile(rootFileDataID))
            {
                ReadRootFile(adt, wdtMPHDFlags);
            }
        }

        public void ReadRootFile(Stream adt, MPHDFlags wdtMPHDFlags, ADTVersion versionOverride = ADTVersion.Unknown)
        {
            using (var bin = new BinaryReader(adt))
            {
                var MCNKi = 0;
                adtfile.chunks = new MCNK[16 * 16];

                var mcnkIndex = new MCIN[256];

                while (adt.Position < adt.Length)
                {
                    var chunkName = (ADTChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();
                    var chunkBytes = bin.ReadBytes((int)chunkSize);

                    if (VersionManager.CurrentVersion == VersionManager.FileVersion.WotLK || versionOverride == ADTVersion.WotLK)
                    {
                        switch (chunkName)
                        {
                            case ADTChunks.MVER:
                                var version = BitConverter.ToUInt32(chunkBytes, 0);
                                if (version != 18)
                                {
                                    throw new Exception("Unsupported ADT version!");
                                }
                                else
                                {
                                    adtfile.version = version;
                                }
                                break;
                            case ADTChunks.MHDR:
                                adtfile.header = MemoryMarshal.Read<MHDR>(chunkBytes);
                                break;
                            case ADTChunks.MH2O:
                                adtfile.mh2o = ReadMH20SubChunk((uint)chunkSize, bin);
                                break;
                            case ADTChunks.MTEX:
                                adtfile.textures = ReadMTEXChunk(chunkBytes);
                                break;
                            case ADTChunks.MMDX:
                                adtfile.objects.m2Names = ReadMMDXChunk(chunkBytes);
                                break;
                            case ADTChunks.MMID:
                                adtfile.objects.m2NameOffsets = ReadMMIDChunk(chunkBytes);
                                break;
                            case ADTChunks.MWMO:
                                adtfile.objects.wmoNames = ReadMWMOChunk(chunkBytes);
                                break;
                            case ADTChunks.MWID:
                                adtfile.objects.wmoNameOffsets = ReadMWIDChunk(chunkBytes);
                                break;
                            case ADTChunks.MODF:
                                adtfile.objects.worldModels = ReadMODFChunk(chunkBytes);
                                break;
                            case ADTChunks.MDDF:
                                adtfile.objects.models = ReadMDDFChunk(chunkBytes);
                                break;
                            case ADTChunks.MCIN:
                                for (var i = 0; i < 256; i++)
                                {
                                    mcnkIndex[i] = MemoryMarshal.Read<MCIN>(chunkBytes.AsSpan(i * Unsafe.SizeOf<MCIN>(), Unsafe.SizeOf<MCIN>()));
                                }
                                break;
                            case ADTChunks.MTXF: // Texture flags
                                adtfile.texFlags = ReadMTXFChunk(chunkBytes);
                                break;
                            case ADTChunks.MCNK: // We read these separately
                            case ADTChunks.MFBO: // Flying bounding box
                                break;
                            default:
                                var chunkNameAsString = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)chunkName));
                                Console.WriteLine(string.Format("WotLK Root ADT: Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkNameAsString, adt.Position));
                                break;
                        }
                    }
                    else
                    {
                        switch (chunkName)
                        {
                            case ADTChunks.MVER:
                                var version = BitConverter.ToUInt32(chunkBytes, 0);
                                if (version != 18)
                                {
                                    throw new Exception("Unsupported ADT version!");
                                }
                                else
                                {
                                    adtfile.version = version;
                                }
                                break;
                            case ADTChunks.MCNK:
                                adtfile.chunks[MCNKi] = ReadMCNKChunk(chunkBytes, wdtMPHDFlags);
                                MCNKi++;
                                break;
                            case ADTChunks.MHDR:
                                adtfile.header = MemoryMarshal.Read<MHDR>(chunkBytes);
                                break;
                            case ADTChunks.MH2O:
                                //adtfile.mh2o = ReadMH20SubChunk(chunkSize, bin);
                                break;
                            case ADTChunks.MFBO: // Flying bounding box
                            //model.blob stuff
                            case ADTChunks.MBMH:
                            case ADTChunks.MBBB:
                            case ADTChunks.MBMI:
                            case ADTChunks.MBNV:
                                break;
                            default:
                                var chunkNameAsString = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)chunkName));
                                Console.WriteLine(string.Format("Root ADT: Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkNameAsString, adt.Position));
                                break;
                        }
                    }
                }

                if (VersionManager.CurrentVersion == VersionManager.FileVersion.WotLK)
                {
                    //for (var i = 0; i < 256; i++)
                    //{
                    //    var mcnk = mcnkIndex[i];
                    //    bin.BaseStream.Position = mcnk.offset + 8;
                    //    adtfile.chunks[i] = ReadMCNKChunk(bin.ReadBytes((int)mcnk.size), wdtMPHDFlags);
                    //}
                }
            }
        }

        private static MCNK ReadMCNKChunk(ReadOnlySpan<byte> bytes, MPHDFlags wdtMPHDFlags)
        {
            var mapchunk = new MCNK()
            {
                header = MemoryMarshal.Read<MCNKheader>(bytes)
            };

            if (VersionManager.CurrentVersion == VersionManager.FileVersion.WotLK)
            {
               // TODO
            }
            else
            {
                using (var stream = new MemoryStream(bytes.Slice(128, bytes.Length - 128).ToArray()))
                using (var subbin = new BinaryReader(stream))
                {
                    long subpos = 0;
                    while (subpos < stream.Length)
                    {
                        subbin.BaseStream.Position = subpos;

                        var subChunkName = (ADTChunks)subbin.ReadUInt32();
                        var subChunkSize = subbin.ReadUInt32();

                        subpos = stream.Position + subChunkSize;

                        switch (subChunkName)
                        {
                            case ADTChunks.MCVT:
                                mapchunk.vertices = ReadMCVTSubChunk(subbin);
                                break;
                            case ADTChunks.MCCV:
                                mapchunk.vertexShading = ReadMCCVSubChunk(subbin);
                                break;
                            case ADTChunks.MCNR:
                                mapchunk.normals = ReadMCNRSubChunk(subbin);
                                break;
                            case ADTChunks.MCSE:
                                mapchunk.soundEmitters = ReadMCSESubChunk(subChunkSize, subbin);
                                break;
                            case ADTChunks.MCBB:
                                mapchunk.blendBatches = ReadMCBBSubChunk(subChunkSize, subbin);
                                break;
                            case ADTChunks.MCLQ:
                            case ADTChunks.MCLV:
                            case ADTChunks.MPTX: // Predominant textures for >4 layer ADTs for detail doodads
                                continue;
                            default:
                                var subChunkNameAsString = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)subChunkName));
                                Console.WriteLine(string.Format("Root ADT MCNK: Found unknown header at offset {1} \"{0}\" while we should've already read them all!", subChunkNameAsString, subpos));
                                break;
                        }
                    }


                }
            }

            return mapchunk;
        }
        private static MCVT ReadMCVTSubChunk(BinaryReader bin)
        {
            var vtchunk = new MCVT()
            {
                vertices = new float[145]
            };

            for (var i = 0; i < 145; i++)
            {
                vtchunk.vertices[i] = bin.ReadSingle();
            }
            return vtchunk;
        }
        private static MCCV ReadMCCVSubChunk(BinaryReader bin)
        {
            var vtchunk = new MCCV()
            {
                red = new byte[145],
                green = new byte[145],
                blue = new byte[145],
                alpha = new byte[145]
            };

            for (var i = 0; i < 145; i++)
            {
                vtchunk.red[i] = bin.ReadByte();
                vtchunk.green[i] = bin.ReadByte();
                vtchunk.blue[i] = bin.ReadByte();
                vtchunk.alpha[i] = bin.ReadByte();
            }

            return vtchunk;
        }
        private static MCNR ReadMCNRSubChunk(BinaryReader bin)
        {
            var nrchunk = new MCNR()
            {
                normal_0 = new short[145],
                normal_1 = new short[145],
                normal_2 = new short[145]
            };

            for (var i = 0; i < 145; i++)
            {
                nrchunk.normal_0[i] = bin.ReadSByte();
                nrchunk.normal_1[i] = bin.ReadSByte();
                nrchunk.normal_2[i] = bin.ReadSByte();
            }

            return nrchunk;
        }
        private static MCSE ReadMCSESubChunk(uint size, BinaryReader bin)
        {
            var sechunk = new MCSE()
            {
                raw = bin.ReadBytes((int)size)
            };

            return sechunk;
        }
        private static MCBB[] ReadMCBBSubChunk(uint size, BinaryReader bin)
        {
            var count = size / 20;
            var bbchunk = new MCBB[count];
            for (var i = 0; i < count; i++)
            {
                bbchunk[i] = bin.Read<MCBB>();
            }
            return bbchunk;
        }

        private static MH2O ReadMH20SubChunk(uint size, BinaryReader bin)
        {
            var chunkBasePos = bin.BaseStream.Position;
            var chunk = new MH2O();
            chunk.headers = new MH2OHeader[256];

            var sortedOffsetList = new List<uint>();

            for (var i = 0; i < 256; i++)
            {
                chunk.headers[i].offsetInstances = bin.ReadUInt32();
                chunk.headers[i].layerCount = bin.ReadUInt32();
                chunk.headers[i].offsetAttributes = bin.ReadUInt32();
            }

            chunk.attributes = new MH2OAttribute[256][];
            chunk.instances = new MH2OInstance[256][];
            chunk.vertexData = new MH2OVertexData[256][];

            for (short i = 0; i < 256; i++)
            {
                var header = chunk.headers[i];
                if (header.offsetAttributes != 0)
                {
                    bin.BaseStream.Position = chunkBasePos + header.offsetAttributes;

                    chunk.attributes[i] = new MH2OAttribute[header.layerCount];

                    for (short j = 0; j < header.layerCount; j++)
                    {
                        chunk.attributes[i][j] = bin.Read<MH2OAttribute>();
                    }
                }

                if (header.offsetInstances != 0)
                {
                    bin.BaseStream.Position = chunkBasePos + header.offsetInstances;

                    chunk.instances[i] = new MH2OInstance[header.layerCount];
                    chunk.vertexData[i] = new MH2OVertexData[header.layerCount];

                    for (short j = 0; j < header.layerCount; j++)
                    {
                        chunk.instances[i][j] = bin.Read<MH2OInstance>();

                        if (chunk.instances[i][j].offsetExistsBitmap != 0)
                            sortedOffsetList.Add(chunk.instances[i][j].offsetExistsBitmap);

                        if (chunk.instances[i][j].offsetVertexData != 0)
                            sortedOffsetList.Add(chunk.instances[i][j].offsetVertexData);
                    }
                }
            }

            sortedOffsetList.Sort();

            for (short i = 0; i < 256; i++)
            {
                var header = chunk.headers[i];

                if (header.offsetInstances != 0)
                {
                    for (short j = 0; j < header.layerCount; j++)
                    {
                        if (chunk.instances[i][j].offsetVertexData == 0)
                            continue;

                        try
                        {
                            var vertexCount = (chunk.instances[i][j].width + 1) * (chunk.instances[i][j].height + 1);
                            uint bytesPerVertex = 0;

                            if (chunk.instances[i][j].liquidObjectOrLVF < 42)
                            {
                                bytesPerVertex = chunk.instances[i][j].liquidObjectOrLVF switch
                                {
                                    0 => 5,
                                    1 => 8,
                                    2 => 1,
                                    3 => 9,
                                    _ => throw new Exception("Encountered unexpected MH2O liquidObjectOrLVF: " + chunk.instances[i][j].liquidObjectOrLVF),
                                };
                            }
                            else
                            {
                                bin.BaseStream.Position = chunkBasePos + chunk.instances[i][j].offsetVertexData;

                                var nextOffsetIndex = sortedOffsetList.IndexOf(chunk.instances[i][j].offsetVertexData) + 1;

                                uint nextOffset;
                                if (nextOffsetIndex > sortedOffsetList.Count - 1)
                                {
                                    nextOffset = size;
                                }
                                else
                                {
                                    nextOffset = sortedOffsetList[nextOffsetIndex];
                                }

                                var calculatedTotalVertexSize = nextOffset - chunk.instances[i][j].offsetVertexData;
                                bytesPerVertex = calculatedTotalVertexSize / (uint)vertexCount;
                            }

                            var vertexChunkSize = 0;
                            switch (bytesPerVertex)
                            {
                                case 1: // Case 2, Depth only data
                                    chunk.vertexData[i][j].liquidVertexFormat = 2;
                                    vertexChunkSize += 1 * vertexCount; // depthmap
                                    break;
                                case 5: // Case 0, Height and Depth data
                                    chunk.vertexData[i][j].liquidVertexFormat = 0;
                                    vertexChunkSize += 4 * vertexCount; // heightmap
                                    vertexChunkSize += 1 * vertexCount; // depthmap
                                    break;
                                case 8: // Case 1, Height and UV data
                                    chunk.vertexData[i][j].liquidVertexFormat = 1;
                                    vertexChunkSize += 4 * vertexCount; // heightmap
                                    vertexChunkSize += 4 * vertexCount; // uvmap
                                    break;
                                case 9: // Case 3, Height, UV and Depth data
                                    chunk.vertexData[i][j].liquidVertexFormat = 3;
                                    vertexChunkSize += 4 * vertexCount; // heightmap
                                    vertexChunkSize += 4 * vertexCount; // uvmap
                                    vertexChunkSize += 1 * vertexCount; // depthmap
                                    break;
                                default:
                                    throw new Exception("Encountered unexpected MH2O bytesPerVertex: " + bytesPerVertex);
                            }

                            chunk.vertexData[i][j].vertexData = bin.ReadBytes(vertexChunkSize);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error during MH2O parsing: " + e.Message);
                        }
                    }
                }
            }
            return chunk;
        }

        /* OBJ */
        private void ReadObjFile(Stream adtObjStream)
        {
            long position = 0;

            adtfile.objects = new Obj();

            using (var bin = new BinaryReader(adtObjStream))
            {
                while (position < adtObjStream.Length)
                {
                    adtObjStream.Position = position;

                    var chunkName = (ADTChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();
                    position = adtObjStream.Position + chunkSize;

                    switch (chunkName)
                    {
                        case ADTChunks.MVER:
                            if (bin.ReadUInt32() != 18)
                            {
                                throw new Exception("Unsupported ADT version!");
                            }
                            break;
                        case ADTChunks.MMDX:
                            adtfile.objects.m2Names = ReadMMDXChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MMID:
                            adtfile.objects.m2NameOffsets = ReadMMIDChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MWMO:
                            adtfile.objects.wmoNames = ReadMWMOChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MWID:
                            adtfile.objects.wmoNameOffsets = ReadMWIDChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MDDF:
                            adtfile.objects.models = ReadMDDFChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MODF:
                            adtfile.objects.worldModels = ReadMODFChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MWDR: // WMO doodad references (multiple)
                            adtfile.objects.worldModelDoodadRefs = ReadMWDRChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MWDS: // WMO doodad sets
                            adtfile.objects.worldModelDoodadSets = ReadMWDSChunk(bin.ReadBytes((int)chunkSize));
                            break;
                        case ADTChunks.MLMB:
                        case ADTChunks.MCNK:
                            break;
                        default:
                            Console.WriteLine(string.Format("ADT OBJ: Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)chunkName))));
                            break;
                    }
                }
            }
        }

        private static MWDR[] ReadMWDRChunk(ReadOnlySpan<byte> bytes)
        {
            var stride = Unsafe.SizeOf<MWDR>();
            var mwdrArr = new MWDR[bytes.Length / stride];
            for (var i = 0; i < mwdrArr.Length; i++)
            {
                mwdrArr[i] = MemoryMarshal.Read<MWDR>(bytes.Slice(i * stride, stride));
            }
            return mwdrArr;
        }

        private static ushort[] ReadMWDSChunk(ReadOnlySpan<byte> bytes)
        {
            var mwdsArr = new ushort[bytes.Length / 2];
            var stride = Unsafe.SizeOf<ushort>();
            for (var i = 0; i < mwdsArr.Length; i++)
            {
                mwdsArr[i] = MemoryMarshal.Read<ushort>(bytes.Slice(i * stride, stride));
            }
            return mwdsArr;
        }

        private static MMDX ReadMMDXChunk(ReadOnlySpan<byte> bytes)
        {
            var mmdx = new MMDX();
            var str = new StringBuilder();

            var offsets = new List<uint>();
            var m2Files = new List<string>();

            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == '\0')
                {
                    m2Files.Add(str.ToString());
                    offsets.Add((uint)(i - str.Length));
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)bytes[i]);
                }
            }

            mmdx.filenames = m2Files.ToArray();
            mmdx.offsets = offsets.ToArray();
            return mmdx;
        }
        private static MMID ReadMMIDChunk(ReadOnlySpan<byte> bytes)
        {
            var count = bytes.Length / 4;

            var mmid = new MMID()
            {
                offsets = new uint[count]
            };

            for (var i = 0; i < count; i++)
            {
                mmid.offsets[i] = MemoryMarshal.Read<uint>(bytes.Slice(i * 4, 4));
            }

            return mmid;
        }

        public static MWMO ReadMWMOChunk(ReadOnlySpan<byte> bytes)
        {
            var mwmo = new MWMO();
            var str = new StringBuilder();

            var offsets = new List<uint>();
            var wmoFiles = new List<string>();

            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == '\0')
                {
                    wmoFiles.Add(str.ToString());
                    offsets.Add((uint)(i - str.Length));
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)bytes[i]);
                }
            }

            mwmo.filenames = wmoFiles.ToArray();
            mwmo.offsets = offsets.ToArray();
            return mwmo;
        }

        public static MWID ReadMWIDChunk(ReadOnlySpan<byte> bytes)
        {
            var count = bytes.Length / 4;

            var mwid = new MWID()
            {
                offsets = new uint[count]
            };

            for (var i = 0; i < count; i++)
            {
                mwid.offsets[i] = MemoryMarshal.Read<uint>(bytes.Slice(i * 4, 4));
            }

            return mwid;
        }
        private static MDDF ReadMDDFChunk(ReadOnlySpan<byte> bytes)
        {
            var mddf = new MDDF();

            var count = bytes.Length / 36;
            mddf.entries = new MDDFEntry[count];

            for (var i = 0; i < count; i++)
            {
                mddf.entries[i] = MemoryMarshal.Read<MDDFEntry>(bytes.Slice(i * 36, 36));
            }

            return mddf;
        }

        public static MODF ReadMODFChunk(ReadOnlySpan<byte> bytes)
        {
            var modf = new MODF();

            var count = bytes.Length / 64;
            modf.entries = new MODFEntry[count];
            for (var i = 0; i < count; i++)
            {
                modf.entries[i] = MemoryMarshal.Read<MODFEntry>(bytes.Slice(i * 64, 64));
            }

            return modf;
        }

        /* TEX */
        private void ReadTexFile(Stream adtTexStream, MPHDFlags wdtMPHDFlags)
        {
            using (var bin = new BinaryReader(adtTexStream))
            {
                uint MCNKi = 0;

                while (adtTexStream.Position < adtTexStream.Length)
                {
                    var chunkName = (ADTChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();
                    var chunkBytes = bin.ReadBytes((int)chunkSize);

                    switch (chunkName)
                    {
                        case ADTChunks.MVER:
                            if (BitConverter.ToUInt32(chunkBytes, 0) != 18)
                            { throw new Exception("Unsupported ADT version!"); }
                            break;
                        case ADTChunks.MTEX:
                            adtfile.textures = ReadMTEXChunk(chunkBytes);
                            break;
                        case ADTChunks.MCNK:
                            ReadTexMCNKChunk(MCNKi, chunkBytes, wdtMPHDFlags);
                            MCNKi++;
                            break;
                        case ADTChunks.MTXP:
                            adtfile.texParams = ReadMTXPChunk(chunkBytes);
                            break;
                        case ADTChunks.MHID: // Height texture fileDataIDs
                            adtfile.heightTextureFileDataIDs = ReadFileDataIDChunk(chunkBytes);
                            break;
                        case ADTChunks.MDID: // Diffuse texture fileDataIDs
                            adtfile.diffuseTextureFileDataIDs = ReadFileDataIDChunk(chunkBytes);
                            break;
                        case ADTChunks.MTCG: // Texture color gradings
                            adtfile.textureColorGradings = ReadMTCGChunk(chunkBytes);
                            break;
                        case ADTChunks.MAMP:
                            break;
                        default:
                            Console.WriteLine(string.Format("ADT TEX: Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", chunkName.ToString("X"), adtTexStream.Position.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)chunkName))));
                            break;
                    }
                }
            }
        }

        private static MTCG[] ReadMTCGChunk(ReadOnlySpan<byte> bytes)
        {
            var mtcg = new MTCG[bytes.Length / 16];
            for (var i = 0; i < mtcg.Length; i++)
            {
                mtcg[i] = MemoryMarshal.Read<MTCG>(bytes.Slice(i * 16, 16));
            }
            return mtcg;
        }

        private void ReadTexMCNKChunk(uint mcnkIndex, ReadOnlySpan<byte> bytes, MPHDFlags wdtMPHDFlags)
        {
            var offset = 0;
            var length = bytes.Length;

            while (offset < length)
            {
                var subChunkName = (ADTChunks)MemoryMarshal.Read<uint>(bytes.Slice(offset, 4));
                offset += 4;
                var subChunkSize = MemoryMarshal.Read<uint>(bytes.Slice(offset, 4));
                offset += 4;
                var subChunkBytes = bytes.Slice(offset, (int)subChunkSize);
                offset += (int)subChunkSize;

                adtfile.chunks ??= new MCNK[16 * 16];

                switch (subChunkName)
                {
                    case ADTChunks.MCLY:
                        adtfile.chunks[mcnkIndex].layers = ReadMCLYSubChunk(subChunkBytes);
                        break;
                    case ADTChunks.MCAL:
                        adtfile.chunks[mcnkIndex].alphaLayer = ReadMCALSubChunk(subChunkBytes, adtfile.chunks[mcnkIndex], wdtMPHDFlags);
                        break;
                    case ADTChunks.MCSH:
                    case ADTChunks.MCMT:
                        break;
                    default:
                        Console.WriteLine(string.Format("ADT TEX MCNK: Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", subChunkName.ToString("X"), offset.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)subChunkName))));
                        break;
                }
            }
        }

        private static MTEX ReadMTEXChunk(ReadOnlySpan<byte> bytes)
        {
            var txchunk = new MTEX();

            var blpFiles = new List<string>();
            var str = new StringBuilder();

            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == '\0')
                {
                    blpFiles.Add(str.ToString());
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)bytes[i]);
                }
            }

            txchunk.filenames = blpFiles.ToArray();
            return txchunk;
        }

        private static MTXF ReadMTXFChunk(ReadOnlySpan<byte> bytes)
        {
            var count = bytes.Length / 4;

            var mtxf = new MTXF
            {
                flags = new uint[count]
            };

            for (var i = 0; i < count; i++)
                mtxf.flags[i] = MemoryMarshal.Read<uint>(bytes.Slice(i * 4, 4));

            return mtxf;
        }

        private static MTXP[] ReadMTXPChunk(ReadOnlySpan<byte> bytes)
        {
            var count = bytes.Length / 16;

            var txparams = new MTXP[count];

            for (var i = 0; i < count; i++)
            {
                txparams[i] = MemoryMarshal.Read<MTXP>(bytes.Slice(i * 16, 16));
            }

            return txparams;
        }

        private static byte[][] ReadMCALSubChunk(ReadOnlySpan<byte> span, MCNK mapchunk, MPHDFlags wdtMPHDFlags)
        {
            var layerCount = mapchunk.layers.Length;
            var mcal = new byte[layerCount][];

            mcal[0] = new byte[64 * 64];
            for (var i = 0; i < 64 * 64; i++)
            {
                mcal[0][i] = 255;
            }

            var immediate_offset = 0;

            for (var layer = 1; layer < layerCount; ++layer)
            {
                if (mapchunk.layers[layer].flags.HasFlag(MCLYFlags.mcly_alpha_map_compressed)) // Compressed
                {
                    //Console.WriteLine("Compressed");
                    // first layer is always fully opaque -> you can let that out
                    // array of 3 x array of 64*64 chars: unpacked alpha values
                    mcal[layer] = new byte[64 * 64];

                    // sorry, I have no god damn idea about c#
                    // *x = value at x. x = pointer to data. ++x = advance pointer a byte
                    uint in_offset = 0;
                    uint out_offset = 0;
                    while (out_offset < 4096)
                    {
                        var info = span[immediate_offset++];
                        ++in_offset;
                        var mode = (uint)(info & 0x80) >> 7; // 0 = copy, 1 = fill
                        var count = (uint)(info & 0x7f); // do mode operation count times

                        if (mode != 0)
                        {
                            var val = span[immediate_offset++];
                            ++in_offset;
                            while (count-- > 0 && out_offset < 4096)
                            {
                                mcal[layer][out_offset] = val;
                                ++out_offset;
                            }


                        }
                        else // mode == 1
                        {
                            while (count-- > 0 && out_offset < 4096)
                            {
                                var val = span[immediate_offset++];
                                ++in_offset;
                                mcal[layer][out_offset] = val;
                                ++out_offset;
                            }
                        }
                    }

                    if (out_offset != 4096)
                        throw new Exception("we somehow overshoot. this should not be the case, except for broken adts");
                }
                else if (wdtMPHDFlags.HasFlag(MPHDFlags.adt_has_big_alpha) || wdtMPHDFlags.HasFlag(MPHDFlags.adt_has_height_texturing)) // Uncompressed (4096)
                {
                    //Console.WriteLine("Uncompressed (4096)");
                    mcal[layer] = span.Slice(immediate_offset, 4096).ToArray();
                    immediate_offset += 4096;
                }
                else // Uncompressed (2048)
                {
                    //Console.WriteLine("Uncompressed (2048)");
                    mcal[layer] = new byte[64 * 64];
                    var mcal_data = span.Slice(immediate_offset, 2048);
                    immediate_offset += 2048;
                    for (var i = 0; i < 2048; ++i)
                    {
                        // maybe nibbles swapped
                        mcal[layer][2 * i + 0] = (byte)(((mcal_data[i] & 0x0F) >> 0) * 17);
                        mcal[layer][2 * i + 1] = (byte)(((mcal_data[i] & 0xF0) >> 4) * 17);
                    }
                }
            }

            return mcal;
        }

        private static MCLY[] ReadMCLYSubChunk(ReadOnlySpan<byte> bytes)
        {
            var stride = Unsafe.SizeOf<MCLY>();
            var count = bytes.Length / stride;
            var mclychunks = new MCLY[count];
            
            for (var i = 0; i < count; i++)
                mclychunks[i] = MemoryMarshal.Read<MCLY>(bytes.Slice(i * stride, stride));

            return mclychunks;
        }

        private static uint[] ReadFileDataIDChunk(ReadOnlySpan<byte> bytes)
        {
            var count = bytes.Length / 4;
            var filedataids = new uint[count];
            for (var i = 0; i < count; i++)
            {
                filedataids[i] = MemoryMarshal.Read<uint>(bytes.Slice(i * 4, 4));
            }
            return filedataids;
        }
    }
}
