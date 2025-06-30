using System;
using System.IO;
using System.Numerics;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.M3;

namespace WoWFormatLib.FileReaders
{
    public class M3Reader
    {
        public M3Model model;

        public void LoadM3(string filename, bool loadSkins = true)
        {
            if (FileProvider.FileExists(filename))
            {
                LoadM3(FileProvider.GetFileDataIdByName(Path.ChangeExtension(filename, "M3")), loadSkins);
            }
            else
            {
                throw new FileNotFoundException("M3 " + filename + " not found");
            }
        }

        public void LoadM3(uint fileDataID, bool loadSkins = true)
        {
#if DEBUG
            using (var stream = FileProvider.OpenFile(fileDataID))
            {
                LoadM3(stream, loadSkins);
            }
#else
                try
                {
                    LoadM3(FileProvider.OpenFile(fileDataID));
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error during reading file: {0}", e.Message);
                    return;
                }
#endif
        }

        public void LoadM3(Stream m3, bool loadSkins = true)
        {
            using (var bin = new BinaryReader(m3))
            {
                while (bin.BaseStream.Position < m3.Length)
                {
                    var chunkName = (M3Chunks)bin.ReadUInt32();
                    if (chunkName == 0)
                        throw new Exception("M3 is likely encrypted");

                    var debugChunkName = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)chunkName));

                    var chunkSize = bin.ReadUInt32();

                    var propertyA = bin.ReadUInt32();
                    var propertyB = bin.ReadUInt32();

                    var prevPos = bin.BaseStream.Position;

                    Console.WriteLine("M3 chunk " + debugChunkName + " of size " + chunkSize);
                    switch (chunkName)
                    {
                        case M3Chunks.M3DT:
                            model.Header = new M3DT()
                            {
                                Version = propertyA,
                                PropertyB = propertyB,

                                Unknown0 = bin.ReadUInt32(),
                                Unknown1_a = bin.ReadUInt16(),
                                Unknown1_b = bin.ReadUInt16(),
                                Unknown2 = bin.ReadUInt32(),
                                Unknown3 = bin.ReadUInt32(),
                                Unknown4 = bin.ReadUInt32(),
                                Unknown5 = bin.ReadUInt32(),
                                Flags = bin.ReadInt32(),

                                BoundingBoxMin = new Vector3(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle()),
                                BoundingBoxMax = new Vector3(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle()),
                                Radius = bin.ReadSingle(),

                                BoundingBox2Min = new Vector3(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle()),
                                BoundingBox2Max = new Vector3(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle()),
                                Radius2 = bin.ReadSingle(),

                                ParticleCount = bin.ReadByte(),
                                UnkByte0 = bin.ReadByte(),
                                UnkByte1 = bin.ReadByte(),
                                UnkByte2 = bin.ReadByte()
                            };
                            break;
                        case M3Chunks.M3SI:
                            model.Instances = ReadM3SIChunk(bin, chunkSize, propertyA, propertyB);
                            break;
                        case M3Chunks.MES3:
                            model.Mesh = ReadMES3Chunk(bin, chunkSize, propertyA, propertyB);
                            break;
                        case M3Chunks.M3CL:
                            model.Collision = ReadM3CLChunk(bin, chunkSize, propertyA, propertyB);
                            break;
                        case M3Chunks.M3XF:
                            model.TransformMatrix = new M3XF()
                            {
                                Transform = bin.Read<Matrix4x4>()
                            };
                            break;
                        default:
                            Console.WriteLine("Unknown M3 chunk: {0} of size {1} at position {2}", debugChunkName, chunkSize, bin.BaseStream.Position);
                            bin.BaseStream.Position += chunkSize;
                            break;
                    }

                    // Ensure we've read everything
                    var correctPos = prevPos + chunkSize;
                    if (bin.BaseStream.Position != correctPos)
                    {
                        Console.WriteLine("Ended up at wrong position after reading " + debugChunkName + ": " + bin.BaseStream.Position + " != " + correctPos);
                        bin.BaseStream.Position = correctPos;
                    }
                }
            }
        }

        public M3SI ReadM3SIChunk(BinaryReader bin, uint chunkSize, uint propertyA, uint propertyB)
        {
            var instances = new M3SI();

            instances.PropertyA = propertyA;
            instances.PropertyB = propertyB;

            var baseOffset = bin.BaseStream.Position;

            instances.Magic = new string(bin.ReadChars(4));
            if (instances.Magic != "m3IS")
                throw new Exception("Invalid M3 instances magic!");

            instances.Version = bin.ReadUInt32();
            if (instances.Version != 2)
                throw new Exception("Unsupported M3 instance version: " + instances.Version);

            instances.TotalSize = bin.ReadUInt32() * 4; // Total Chunk size
            instances.Unknown1 = bin.ReadUInt32();

            var instanceSize = bin.ReadUInt32();
            var maybeNotInstanceOffset = bin.ReadUInt32() * 4;

            instances.Instances = new M3Instance[instanceSize];

            for (int i = 0; i < instanceSize; i++)
            {
                instances.Instances[i] = new M3Instance()
                {
                    FileDataID = bin.ReadUInt32(),
                    GUID = bin.ReadBytes(16),
                    Unknown0 = bin.ReadUInt32(),
                    BlendMode = bin.ReadUInt32(),
                    Unknown2 = bin.ReadUInt32(),
                    Unknown3 = bin.ReadUInt32(),
                    Unknown4 = bin.ReadUInt32()
                };

                var shaderData = new ShaderData()
                {
                    uniformCount = bin.ReadInt32(),
                    uniformDataSize = bin.ReadInt32(),
                    uniformHashesOffset = bin.ReadInt32(),
                    uniformTypesOffset = bin.ReadInt32(),
                    uniformLocationsOffset = bin.ReadInt32(),
                    uniformDataOffset = bin.ReadInt32(),
                    samplerCount = bin.ReadInt32(),
                    samplerHashesOffset = bin.ReadInt32(),
                    samplerTextureFileIDsOffset = bin.ReadInt32(),
                    unkDataCount = bin.ReadInt32(),
                    unkDataHashesOffset = bin.ReadInt32(),
                    unkDataOffset = bin.ReadInt32()
                };

                var prevPos = bin.BaseStream.Position;

                if(shaderData.uniformCount > 0)
                {
                    shaderData.UniformHashes = [];
                    shaderData.UniformTypes = [];
                    shaderData.UniformLocations = [];
                    shaderData.UniformData = [];

                    bin.BaseStream.Position = baseOffset + (shaderData.uniformHashesOffset * 4);
                    for (int u = 0; u < shaderData.uniformCount; u++)
                        shaderData.UniformHashes.Add(bin.ReadUInt64());

                    bin.BaseStream.Position = baseOffset + (shaderData.uniformTypesOffset * 4);
                    for (int u = 0; u < shaderData.uniformCount; u++)
                        shaderData.UniformTypes.Add(bin.ReadUInt16());

                    bin.BaseStream.Position = baseOffset + (shaderData.uniformLocationsOffset * 4);
                    for (int u = 0; u < shaderData.uniformCount; u++)
                        shaderData.UniformLocations.Add(bin.ReadInt32());

                    bin.BaseStream.Position = baseOffset + (shaderData.uniformDataOffset * 4);
                    for (int u = 0; u < shaderData.uniformCount; u++)
                    {
                        switch (shaderData.UniformTypes[u])
                        {
                            case 0:
                                shaderData.UniformData.Add([bin.ReadInt32()]);
                                break;
                            case 14:
                                shaderData.UniformData.Add([bin.ReadSingle()]);
                                break;
                            case 15:
                                shaderData.UniformData.Add([bin.ReadSingle(), bin.ReadSingle()]);
                                break;
                            case 16:
                                shaderData.UniformData.Add([bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle()]);
                                break;
                            case 17:
                                shaderData.UniformData.Add([bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle()]);
                                break;
                            default:
                                throw new Exception("Unknown uniform type: " + shaderData.UniformTypes[u]);
                        }
                    }
                }

                if(shaderData.samplerCount > 0)
                {
                    shaderData.SamplerHashes = [];
                    shaderData.SamplerTextureFileIDs = [];

                    bin.BaseStream.Position = baseOffset + (shaderData.samplerHashesOffset * 4);
                    for (int s = 0; s < shaderData.samplerCount; s++)
                        shaderData.SamplerHashes.Add(bin.ReadUInt64());

                    bin.BaseStream.Position = baseOffset + (shaderData.samplerTextureFileIDsOffset * 4);
                    for(int s = 0; s < shaderData.samplerCount; s++)
                        shaderData.SamplerTextureFileIDs.Add(bin.ReadInt32());
                }

                if(shaderData.unkDataCount > 0)
                {
                    shaderData.UnkDataHashes = [];
                    shaderData.UnkData = [];

                    bin.BaseStream.Position = baseOffset + (shaderData.unkDataHashesOffset * 4);
                    for (int u = 0; u < shaderData.unkDataCount; u++)
                        shaderData.UnkDataHashes.Add(bin.ReadUInt64());

                    bin.BaseStream.Position = baseOffset + (shaderData.unkDataOffset * 4);
                    for (int u = 0; u < shaderData.unkDataCount; u++)
                    {
                        var unkData = new UnkData
                        {
                            unk0 = bin.ReadByte(),
                            unk1 = bin.ReadByte(),
                            unk2 = bin.ReadUInt16()
                        };
                        shaderData.UnkData.Add(unkData);
                    }
                }

                bin.BaseStream.Position = prevPos;

                instances.Instances[i].shaderData = shaderData;
            }

            if ((bin.BaseStream.Position - baseOffset) != instances.TotalSize)
            {
                Console.WriteLine("Did not end up at expect position for instances, skipping ahead..");
                bin.BaseStream.Position = baseOffset + instances.TotalSize;
            }

            return instances;
        }

        public MES3 ReadMES3Chunk(BinaryReader bin, uint chunkSize, uint propertyA, uint propertyB)
        {
            var mesh = new MES3();

            using (var ms = new MemoryStream())
            using (var subbin = new BinaryReader(ms))
            {
                ms.Write(bin.ReadBytes((int)chunkSize), 0, (int)chunkSize);
                subbin.BaseStream.Position = 0;
                while (subbin.BaseStream.Position < ms.Length)
                {
                    var subChunkName = (M3Chunks)subbin.ReadUInt32();
                    var debugChunkName = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)subChunkName));
                    var subChunkSize = subbin.ReadUInt32();

                    Console.WriteLine("M3 chunk MES3 subchunk " + debugChunkName);

                    switch (subChunkName)
                    {
                        case M3Chunks.M3VR:
                            mesh.Version.Version = subbin.ReadUInt32();
                            subbin.ReadUInt32(); // Skip unknown value
                            break;
                        case M3Chunks.VPOS:
                            mesh.Vertices.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.Vertices.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VNML:
                            mesh.Normals.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.Normals.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VUV0:
                            mesh.UV0.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.UV0.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VUV1:
                            mesh.UV1.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.UV1.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VUV2:
                            mesh.UV2.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.UV2.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VUV3:
                            mesh.UV3.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.UV3.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VUV4:
                            mesh.UV4.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.UV4.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VUV5:
                            mesh.UV5.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.UV5.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VTAN:
                            mesh.Tangents.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.Tangents.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VINX:
                            mesh.Indices.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.Indices.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VWTS:
                            mesh.BoneWeights.IndicesFormat = new string(subbin.ReadChars(4));
                            mesh.BoneWeights.WeightsFormat = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            break;
                        case M3Chunks.VIBP:
                            mesh.InverseBindPoses.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.InverseBindPoses.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VCL0:
                            mesh.Color0.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.Color0.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VCL1:
                            mesh.Color1.Format = new string(subbin.ReadChars(4));
                            subbin.ReadUInt32(); // Skip unknown value
                            mesh.Color1.Buffer = ReadBuffer(subbin, subChunkSize);
                            break;
                        case M3Chunks.VSTR:
                            mesh.Names.PropertyA = subbin.ReadUInt32();
                            mesh.Names.PropertyB = subbin.ReadUInt32();
                            mesh.Names.NameBlock = new string(subbin.ReadChars((int)subChunkSize)).Replace("\0", "");
                            break;
                        case M3Chunks.VGEO:
                            mesh.Geosets.PropertyA = subbin.ReadUInt32();
                            mesh.Geosets.PropertyB = subbin.ReadUInt32();
                            mesh.Geosets.Geosets = new M3Geoset[mesh.Geosets.PropertyA];
                            for(var i = 0; i < mesh.Geosets.PropertyA; i++)
                            {
                                mesh.Geosets.Geosets[i] = new M3Geoset()
                                {
                                    Unknown0 = subbin.ReadUInt32(),
                                    NameCharStart = subbin.ReadUInt32(),
                                    NameCharCount = subbin.ReadUInt32(),
                                    IndexStart = subbin.ReadUInt32(),
                                    IndexCount = subbin.ReadUInt32(),
                                    VertexStart = subbin.ReadUInt32(),
                                    VertexCount = subbin.ReadUInt32(),
                                    Unknown1 = subbin.ReadUInt32(),
                                    Unknown2 = subbin.ReadUInt32()
                                };
                            }
                            break;
                        case M3Chunks.LODS:
                            mesh.LodLevels.LODCount = subbin.ReadUInt32();
                            mesh.LodLevels.GeosetCount = subbin.ReadUInt32();
                            mesh.LodLevels.LodLevels = new M3LodLevel[mesh.LodLevels.LODCount + 1];
                            for (var i = 0; i < mesh.LodLevels.LODCount + 1; i++)
                            {
                                mesh.LodLevels.LodLevels[i] = new M3LodLevel()
                                {
                                    VertexCount = subbin.ReadUInt32(),
                                    IndexCount = subbin.ReadUInt32()
                                };
                            }
                            break;
                        case M3Chunks.RBAT:
                            mesh.RenderBatches.BatchCount= subbin.ReadUInt32();
                            mesh.RenderBatches.PropertyB = subbin.ReadUInt32();
                            mesh.RenderBatches.RenderBatches = new M3RenderBatch[mesh.RenderBatches.BatchCount];
                            for (var i = 0; i < mesh.RenderBatches.BatchCount; i++)
                            {
                                mesh.RenderBatches.RenderBatches[i] = new M3RenderBatch()
                                {
                                    Unknown0 = subbin.ReadUInt16(),
                                    Unknown1 = subbin.ReadUInt16(),
                                    GeosetIndex = subbin.ReadUInt16(),
                                    MaterialIndex = subbin.ReadUInt16()
                                };
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown M3 MES3 sub-chunk: " + debugChunkName + " @ " + (bin.BaseStream.Position - chunkSize) + subbin.BaseStream.Position);
                            subbin.BaseStream.Position += subChunkSize + 8;
                            break;
                    }
                }
            }

            return mesh;
        }

        public byte[] ReadBuffer(BinaryReader bin, uint chunkSize)
        {
            var buffer = new byte[chunkSize];
            bin.Read(buffer, 0, (int)chunkSize);
            return buffer;
        }

        public M3CL ReadM3CLChunk(BinaryReader bin, uint chunkSize, uint propertyA, uint propertyB)
        {
            var collision = new M3CL();

            bin.BaseStream.Position += chunkSize;

            return collision;
        }
    }
}
