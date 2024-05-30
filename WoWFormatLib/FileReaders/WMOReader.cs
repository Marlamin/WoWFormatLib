using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.WMO;

namespace WoWFormatLib.FileReaders
{
    public class WMOReader
    {
        private WMO wmofile;
        private byte lodLevel;

        public WMO LoadWMO(Stream wmo, byte lod = 0, string filename = "")
        {
            lodLevel = lod;

            ReadWMO(wmo);
            return wmofile;
        }

        public WMO LoadWMO(uint filedataid, byte lod = 0, string filename = "")
        {
            lodLevel = lod;

            if (FileProvider.FileExists(filedataid))
            {
                using (var wmoStream = FileProvider.OpenFile(filedataid))
                {
                    ReadWMO(wmoStream, filename);
                }
            }
            else
            {
                throw new FileNotFoundException("File " + filedataid + " was not found");
            }

            return wmofile;
        }

        public WMO LoadWMO(string filename, byte lod = 0)
        {
            lodLevel = lod;

            if (FileProvider.FileExists(filename))
            {
                using (var wmoStream = FileProvider.OpenFile(filename))
                {
                    ReadWMO(wmoStream, filename);
                }
            }
            else
            {
                throw new FileNotFoundException("File " + filename + " was not found");
            }

            return wmofile;
        }

        /* PARENT */
        private void ReadWMO(Stream wmo, string filename = "")
        {
            using (var bin = new BinaryReader(wmo))
            {
                long position = 0;
                while (position < wmo.Length)
                {
                    wmo.Position = position;

                    var chunkName = (WMOChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    position = wmo.Position + chunkSize;

                    switch (chunkName)
                    {
                        case WMOChunks.MVER:
                            wmofile.version = bin.ReadUInt32();
                            if (wmofile.version != 17)
                            {
                                throw new Exception("Unsupported WMO version! (" + wmofile.version + ")");
                            }
                            break;
                        case WMOChunks.MOGP:
                            throw new NotSupportedException("Trying to parse group WMO as root WMO!");
                        case WMOChunks.MOHD:
                            wmofile.header = bin.Read<MOHD>();
                            break;
                        case WMOChunks.MOTX:
                            wmofile.textures = ReadMOTXChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MOMT:
                            wmofile.materials = ReadMOMTChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MOGN:
                            wmofile.groupNames = ReadMOGNChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MOGI:
                            wmofile.groupInfo = ReadMOGIChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MODS:
                            wmofile.doodadSets = ReadMODSChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MODI:
                            wmofile.doodadIds = ReadMODIChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MODN:
                            wmofile.doodadNames = ReadMODNChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MODD:
                            wmofile.doodadDefinitions = ReadMODDChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MOSB:
                            wmofile.skybox = ReadMOSBChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MOSI:
                            wmofile.skyboxFileDataID = ReadMOSIChunk(chunkSize, bin);
                            break;
                        case WMOChunks.GFID:
                            wmofile.groupFileDataIDs = ReadGFIDChunk(chunkSize, bin);
                            break;
                        case WMOChunks.MGI2: // MGI2 Group Info (LOD)
                            wmofile.groupInfo2 = ReadMGI2Chunk(chunkSize, bin);
                            break;
                        case WMOChunks.MOPV: // MOPV Portal Vertices
                        case WMOChunks.MOPR: // MOPR Portal References
                        case WMOChunks.MOPT: // MOPT Portal Information
                        case WMOChunks.MOVV: // MOVV Visible block vertices
                        case WMOChunks.MOVB: // MOVB Visible block list
                        case WMOChunks.MOLT: // MOLT Lighting Information
                        case WMOChunks.MFOG: // MFOG Fog Information
                        case WMOChunks.MCVP: // MCVP Convex Volume Planes
                        case WMOChunks.MOUV: // MOUV Animated texture UVs
                        case WMOChunks.MLSP: // MLSP Light Set Pointlights
                        case WMOChunks.MDDI: // MDDI Detail Doodad Information
                        case WMOChunks.MDDL: // MODL Detail Doodad Layers
                        case WMOChunks.MNLD: // MNLD New Light Defs
                        case WMOChunks.MAVD: // MAVD Ambient Volumne Definition
                        case WMOChunks.MFED: // ?
                        case WMOChunks.MAVG: // ?
                        case WMOChunks.MPVD: // ?
                        case WMOChunks.MBVD: // ?
                        case WMOChunks.MOLV: // ?
                        case WMOChunks.MOMX: // ?
                            break;
                        default:
                            Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)chunkName))));
                            break;
                    }
                }
            }

            var groupFiles = new WMOGroupFile[wmofile.header.nGroups];

            if (wmofile.header.nLod != 0 && (lodLevel + 1) > wmofile.header.nLod)
            {
                throw new Exception("Requested LOD (" + lodLevel + ") exceeds the max LOD for this WMO (" + (wmofile.header.nLod - 1) + ")");
            }

            var start = wmofile.header.nGroups * lodLevel;

            if (wmofile.groupFileDataIDs == null && !string.IsNullOrEmpty(filename))
            {
                for (var i = 0; i < wmofile.header.nGroups; i++)
                {
                    var groupFilename = filename.Replace(".wmo", "_" + i.ToString().PadLeft(3, '0') + ".wmo");
                    if (FileProvider.FileExists(groupFilename))
                    {
                        var groupFileDataID = FileProvider.GetFileDataIdByName(groupFilename);
                        using (var wmoStream = FileProvider.OpenFile(groupFileDataID))
                        {
                            groupFiles[i] = ReadWMOGroupFile(groupFileDataID, wmoStream);
                        }
                    }
                    else
                    {
                        Console.WriteLine("CASC is reporting " + groupFilename + " does not exist!");
                    }
                }
            }
            else
            {
                for (var i = 0; i < wmofile.header.nGroups; i++)
                {
                    if (wmofile.groupFileDataIDs == null)
                        continue;

                    var groupFileDataID = wmofile.groupFileDataIDs[start + i];

                    if (lodLevel == 3 && groupFileDataID == 0) // if lod is 3 and there's no lod3 available, fall back to lod1
                    {
                        groupFileDataID = wmofile.groupFileDataIDs[i + (wmofile.header.nGroups * 2)];
                    }

                    if (lodLevel >= 2 && groupFileDataID == 0) // if lod is 2 or higher and there's no lod2 available, fall back to lod1
                    {
                        groupFileDataID = wmofile.groupFileDataIDs[i + (wmofile.header.nGroups * 1)];
                    }

                    if (lodLevel >= 1 && groupFileDataID == 0) // if lod is 1 or higher check if lod1 available, fall back to lod0
                    {
                        groupFileDataID = wmofile.groupFileDataIDs[i];
                    }

                    if (FileProvider.FileExists(groupFileDataID))
                    {
                        using (var wmoStream = FileProvider.OpenFile(groupFileDataID))
                        {
                            groupFiles[i] = ReadWMOGroupFile(groupFileDataID, wmoStream);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("CASC is reporting " + groupFileDataID + " does not exist! This shouldn't happen.");
                    }
                }
            }

            wmofile.group = groupFiles;
        }

        private static uint[] ReadGFIDChunk(uint size, BinaryReader bin)
        {
            var count = size / 4;
            var gfids = new uint[count];
            for (var i = 0; i < count; i++)
            {
                gfids[i] = bin.ReadUInt32();
            }
            return gfids;
        }

        private static MOTX[] ReadMOTXChunk(uint size, BinaryReader bin)
        {
            //List of BLP filenames
            var blpFilesChunk = bin.ReadBytes((int)size);
            var blpFiles = new List<string>();
            var blpOffset = new List<int>();
            var str = new StringBuilder();

            var buildingString = false;
            for (var i = 0; i < blpFilesChunk.Length; i++)
            {
                if (blpFilesChunk[i] == '\0')
                {
                    if (buildingString)
                    {
                        str.Replace("..", ".");
                        blpFiles.Add(str.ToString());
                        blpOffset.Add(i - str.ToString().Length);
                    }
                    buildingString = false;
                    str = new StringBuilder();
                }
                else
                {
                    buildingString = true;
                    str.Append((char)blpFilesChunk[i]);
                }
            }

            var textures = new MOTX[blpFiles.Count];
            for (var i = 0; i < blpFiles.Count; i++)
            {
                textures[i].filename = blpFiles[i];
                textures[i].startOffset = (uint)blpOffset[i];
            }

            return textures;
        }
        private static MOMT[] ReadMOMTChunk(uint size, BinaryReader bin)
        {
            var count = size / 64;
            var materials = new MOMT[count];
            for (var i = 0; i < count; i++)
            {
                materials[i] = bin.Read<MOMT>();
            }
            return materials;
        }
        private static MOGN[] ReadMOGNChunk(uint size, BinaryReader bin)
        {
            var wmoGroupsChunk = bin.ReadBytes((int)size);
            var str = new StringBuilder();
            var nameList = new List<string>();
            var nameOffset = new List<int>();
            for (var i = 0; i < wmoGroupsChunk.Length; i++)
            {
                if (wmoGroupsChunk[i] == '\0')
                {
                    if (str.Length > 1)
                    {
                        nameOffset.Add(i - str.ToString().Length);
                        nameList.Add(str.ToString());
                    }
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)wmoGroupsChunk[i]);
                }
            }

            var groupNames = new MOGN[nameList.Count];
            for (var i = 0; i < nameList.Count; i++)
            {
                groupNames[i].name = nameList[i];
                groupNames[i].offset = nameOffset[i];
            }
            return groupNames;
        }
        private static MOGI[] ReadMOGIChunk(uint size, BinaryReader bin)
        {
            var count = size / 32;
            var groupInfo = new MOGI[count];
            for (var i = 0; i < count; i++)
            {
                groupInfo[i] = bin.Read<MOGI>();
            }
            return groupInfo;
        }

        private static MGI2[] ReadMGI2Chunk(uint size, BinaryReader bin)
        {
            var count = size / 8;
            var groupInfo2 = new MGI2[count];
            for (var i = 0; i < count; i++)
            {
                groupInfo2[i] = bin.Read<MGI2>();
            }
            return groupInfo2;
        }

        private static MODS[] ReadMODSChunk(uint size, BinaryReader bin)
        {
            var numDoodadSets = size / 32;
            var doodadSets = new MODS[numDoodadSets];
            for (var i = 0; i < numDoodadSets; i++)
            {
                doodadSets[i].setName = new string(bin.ReadChars(20)).Replace("\0", string.Empty);
                doodadSets[i].firstInstanceIndex = bin.ReadUInt32();
                doodadSets[i].numDoodads = bin.ReadUInt32();
                doodadSets[i].unused = bin.ReadUInt32();
            }
            return doodadSets;
        }
        private static MODN[] ReadMODNChunk(uint size, BinaryReader bin)
        {
            //List of M2 filenames, but are still named after MDXs internally. Have to rename!
            var m2FilesChunk = bin.ReadBytes((int)size);
            var m2Files = new List<string>();
            var m2Offset = new List<int>();
            var str = new StringBuilder();

            for (var i = 0; i < m2FilesChunk.Length; i++)
            {
                if (m2FilesChunk[i] == '\0')
                {
                    if (str.Length > 1)
                    {
                        str.Replace("..", ".");
                        str.Replace(".mdx", ".m2");

                        m2Files.Add(str.ToString());
                        m2Offset.Add(i - str.ToString().Length);
                    }
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)m2FilesChunk[i]);
                }
            }

            var num = m2Files.Count;

            var doodadNames = new MODN[num];
            for (var i = 0; i < num; i++)
            {
                doodadNames[i].filename = m2Files[i];
                doodadNames[i].startOffset = (uint)m2Offset[i];
            }
            return doodadNames;
        }
        private static uint[] ReadMODIChunk(uint size, BinaryReader bin)
        {
            var numIds = size / 4;
            var fileDataIDs = new uint[numIds];
            for (var i = 0; i < numIds; i++)
            {
                fileDataIDs[i] = bin.ReadUInt32();
            }
            return fileDataIDs;
        }
        private static MODD[] ReadMODDChunk(uint size, BinaryReader bin)
        {
            var numDoodads = size / 40;
            var doodads = new MODD[numDoodads];
            for (var i = 0; i < numDoodads; i++)
            {
                var raw_offset = bin.ReadBytes(3);
                doodads[i].offset = (uint)(raw_offset[0] | raw_offset[1] << 8 | raw_offset[2] << 16);
                doodads[i].flags = (byte)(MODDFlags)bin.ReadByte();
                doodads[i].position = bin.Read<Vector3>();
                doodads[i].rotation = bin.Read<Quaternion>();
                doodads[i].scale = bin.ReadSingle();
                doodads[i].color = bin.ReadBytes(4);
            }
            return doodads;
        }

        private static string ReadMOSBChunk(uint size, BinaryReader bin)
        {
            return bin.ReadCString();
        }

        private static uint ReadMOSIChunk(uint size, BinaryReader bin)
        {
            return bin.ReadUInt32();
        }

        /* GROUP */
        public static WMOGroupFile ReadWMOGroupFile(uint filedataid, Stream wmo)
        {
            var groupFile = new WMOGroupFile();

            using (var bin = new BinaryReader(wmo))
            {
                long position = 0;
                while (position < wmo.Length)
                {
                    wmo.Position = position;
                    var chunkName = (WMOChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();
                    position = wmo.Position + chunkSize;

                    switch (chunkName)
                    {
                        case WMOChunks.MVER:
                            groupFile.version = bin.ReadUInt32();
                            if (groupFile.version != 17)
                            {
                                throw new Exception("Unsupported WMO version! (" + groupFile.version + ")");
                            }
                            continue;
                        case WMOChunks.MOGP:
                            groupFile.mogp = ReadMOGPChunk(chunkSize, bin);
                            continue;
                        default:
                            throw new Exception(string.Format("{2} Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString(), filedataid));
                    }
                }
            }

            return groupFile;
        }
        private static MOGP ReadMOGPChunk(uint size, BinaryReader bin)
        {
            var mogp = new MOGP()
            {
                nameOffset = bin.ReadUInt32(),
                descriptiveNameOffset = bin.ReadUInt32(),
                flags = (MOGPFlags)bin.ReadUInt32(),
                boundingBox1 = bin.Read<Vector3>(),
                boundingBox2 = bin.Read<Vector3>(),
                ofsPortals = bin.ReadUInt16(),
                numPortals = bin.ReadUInt16(),
                numBatchesA = bin.ReadUInt16(),
                numBatchesB = bin.ReadUInt16(),
                numBatchesC = bin.ReadUInt16(),
                numBatchesD = bin.ReadUInt16(),
                fogIndices_0 = bin.ReadByte(),
                fogIndices_1 = bin.ReadByte(),
                fogIndices_2 = bin.ReadByte(),
                fogIndices_3 = bin.ReadByte(),
                liquidType = bin.ReadUInt32(),
                groupID = bin.ReadUInt32(),
                flags2 = (MOGPFlags2)bin.ReadUInt32(),
                unused = bin.ReadUInt32()
            };

            using (var stream = new MemoryStream(bin.ReadBytes((int)size)))
            using (var subbin = new BinaryReader(stream))
            {
                long position = 0;
                var MOTVi = 0;

                if (mogp.flags.HasFlag(MOGPFlags.mogp_has_uv3))
                {
                    mogp.textureCoords = new MOTV[4][];
                }
                else
                {
                    mogp.textureCoords = new MOTV[4][];
                }

                while (position < stream.Length)
                {
                    stream.Position = position;

                    var subChunkName = (WMOChunks)subbin.ReadUInt32();
                    var subChunkSize = subbin.ReadUInt32();

                    position = stream.Position + subChunkSize;

                    switch (subChunkName)
                    {
                        case WMOChunks.MOHD:
                            throw new NotSupportedException("Trying to parse root WMO as group WMO!");
                        case WMOChunks.MOVI: // MOVI Vertex indices for triangles
                            mogp.indices = ReadMOVIChunk(subChunkSize, subbin);
                            break;
                        case WMOChunks.MOVT: // MOVT Vertices chunk
                            mogp.vertices = ReadMOVTChunk(subChunkSize, subbin);
                            break;
                        case WMOChunks.MOTV: // MOTV Texture coordinates
                            mogp.textureCoords[MOTVi++] = ReadMOTVChunk(subChunkSize, subbin);
                            break;
                        case WMOChunks.MONR: // MONR Normals
                            mogp.normals = ReadMONRChunk(subChunkSize, subbin);
                            break;
                        case WMOChunks.MOBA: // MOBA Render batches
                            mogp.renderBatches = ReadMOBAChunk(subChunkSize, subbin);
                            break;
                        case WMOChunks.MOPY: // MOPY Material info for triangles, two bytes per triangle.
                            mogp.materialInfo = ReadMOPYChunk(subChunkSize, subbin, 1);
                            break;
                        case WMOChunks.MPY2: // Material info for triangles, two bytes per triangle.
                            mogp.materialInfo = ReadMOPYChunk(subChunkSize, subbin, 2);
                            break;
                        case WMOChunks.MOBS: // MOBS Shadow batches
                            mogp.shadowBatches = ReadMOBSChunk(subChunkSize, subbin);
                            break;
                        case WMOChunks.MODR: // MODR Doodad references
                        case WMOChunks.MOBN: // MOBN Array of t_BSP_NODE
                        case WMOChunks.MOBR: // MOBR Face indices
                        case WMOChunks.MOLR: // MOLR Override light references
                        case WMOChunks.MOCV: // MOCV Vertex colors
                        case WMOChunks.MDAL: // MDAL Replacement for header color
                        case WMOChunks.MLIQ: // MLIQ Liquids
                        case WMOChunks.MOTA: // MOTA Tangent Array
                        case WMOChunks.MOPL: // MOPL Terrain Cutting PLanes
                        case WMOChunks.MOLP: // MOLP Points Lights
                        case WMOChunks.MOLS: // MOLS Spot Lights
                        case WMOChunks.MOPB: // MOPB Prepass Batches
                        case WMOChunks.MNLR: // MNLR Light New References
                        case WMOChunks.MLSP: // MLSP Light Set Pointlights
                        case WMOChunks.MLSS: // MLSS Light Set Spotlights
                        case WMOChunks.MLSK: // MLSK Point Light animsets
                        case WMOChunks.MLSO: // MLSO Spot Light animsets
                        case WMOChunks.MOP2: // MOP2 Map Object Pointlight Anims
                        case WMOChunks.MFVR: // MVFR Volume Refs
                        case WMOChunks.MAVR: // MAVR Ambient Volume Refs
                        case WMOChunks.MPVR: // MPVR Particulate Volume Refs
                        case WMOChunks.MBVR: // MBVR Box Volume Refs
                        case WMOChunks.MOGX: // ?
                        case WMOChunks.MOQG: // ?
                        case WMOChunks.MOC2: // ?
                            continue;
                        default:
#if DEBUG
                            Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", subChunkName.ToString("X"), position.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)subChunkName))));
                            break;
#else
                            CASCLib.Logger.WriteLine("Found unknown header at offset {1} \"{0}\" while we should've already read them all!", subChunkName.ToString("X"), position.ToString());
                            break;
#endif
                    }
                }
            }

            return mogp;
        }

        private static MOBS[] ReadMOBSChunk(uint size, BinaryReader bin)
        {
            var numBatches = size / 24;
            var batches = new MOBS[numBatches];
            for (var i = 0; i < numBatches; i++)
            {
                batches[i] = bin.Read<MOBS>();
            }
            return batches;
        }

        private static MONR[] ReadMONRChunk(uint size, BinaryReader bin)
        {
            var numNormals = size / 12;
            var normals = new MONR[numNormals];
            for (var i = 0; i < numNormals; i++)
            {
                normals[i].normal = bin.Read<Vector3>();
            }
            return normals;
        }
        private static MOVT[] ReadMOVTChunk(uint size, BinaryReader bin)
        {
            var numVerts = size / 12;
            var vertices = new MOVT[numVerts];
            for (var i = 0; i < numVerts; i++)
            {
                vertices[i].vector = bin.Read<Vector3>();
            }
            return vertices;
        }
        private static MOBA[] ReadMOBAChunk(uint size, BinaryReader bin)
        {
            var numBatches = size / 24;
            var batches = new MOBA[numBatches];
            for (var i = 0; i < numBatches; i++)
            {
                batches[i] = bin.Read<MOBA>();
            }
            return batches;
        }
        private static MOPY[] ReadMOPYChunk(uint size, BinaryReader bin, byte version = 1)
        {
            var numMaterials = size / 2;
            if(version == 2)
                numMaterials = size / 4;

            var materials = new MOPY[numMaterials];
            for (var i = 0; i < numMaterials; i++)
            {
                if(version == 1)
                {
                    materials[i].flags = (MOPYFlags)(ushort)bin.ReadByte();
                    materials[i].materialID = bin.ReadByte();
                }
                else
                {
                    materials[i] = bin.Read<MOPY>();
                }
            }
            return materials;
        }
        private static MOTV[] ReadMOTVChunk(uint size, BinaryReader bin)
        {
            var numCoords = size / 8;
            var textureCoords = new MOTV[numCoords];
            for (var i = 0; i < numCoords; i++)
            {
                textureCoords[i].X = bin.ReadSingle();
                textureCoords[i].Y = bin.ReadSingle();
            }
            return textureCoords;
        }
        private static MOVI[] ReadMOVIChunk(uint size, BinaryReader bin)
        {
            var numIndices = size / 2;
            var indices = new MOVI[numIndices];
            for (var i = 0; i < numIndices; i++)
            {
                indices[i].indice = bin.ReadUInt16();
            }
            return indices;
        }
    }
}
