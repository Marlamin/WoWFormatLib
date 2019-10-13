using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WoWFormatLib.Structs.MDX;

namespace WoWFormatLib.FileReaders
{
    public class MDXReader
    {
        public MDXModel model;

        public void LoadModel(Stream stream)
        {
            BinaryReader data = new BinaryReader(stream);

            // Magic check.
            MDXChunks magic = (MDXChunks)data.ReadUInt32();
            if (magic != MDXChunks.MDLX)
                throw new Exception(string.Format("Invalid MDX header? Got {0}, expected {1}", magic.ToString("X"), MDXChunks.MDLX.ToString("X")));

            long pos = 4; // Start just ahead of magic.
            while (pos < stream.Length)
            {
                stream.Position = pos;

                MDXChunks chunkID = (MDXChunks)data.ReadUInt32();
                uint chunkLength = data.ReadUInt32();

                // Ensure that reading of the next chunk will start in the right place
                // regardless of where we stop parsing this one.
                pos = stream.Position + chunkLength;

                switch (chunkID)
                {
                    case MDXChunks.VERS: ParseChunk_VERS(data); break;
                    case MDXChunks.MODL: ParseChunk_MODL(data); break;
                    case MDXChunks.GEOS: ParseChunk_GEOS(data, stream, chunkLength); break;
                    case MDXChunks.MTLS: ParseChunk_MTLS(data, stream, chunkLength); break;
                    case MDXChunks.TEXS: ParseChunk_TEXS(data, chunkLength); break;
                    default:
                        Console.WriteLine("Skipping unknown MDX chunk: {0}", chunkID.ToString("X"));
                        break;
                }
            }
        }

        private void ParseChunk_VERS(BinaryReader data)
        {
            model.version = data.ReadUInt32();
        }

        private void ParseChunk_MODL(BinaryReader data)
        {
            // Model names are always a fixed length of 50 bytes.
            byte[] nameBytes = data.ReadBytes(50);
            model.name = Encoding.UTF8.GetString(nameBytes).Replace("\0", string.Empty);
        }
        
        private void ParseChunk_MTLS(BinaryReader data, Stream stream, uint chunkLength)
        {
            long chunkEnd = stream.Position + chunkLength;

            // Look ahead to pre-calculate material count.
            uint materialCount = 0;
            while (stream.Position < chunkEnd)
            {
                materialCount++;
                data.Skip((int)data.ReadUInt32() - 4); // materialSize is inclusive.
            }

            stream.Seek(chunkEnd - chunkLength, SeekOrigin.Begin);
            Material[] materials = new Material[materialCount];

            uint materialIndex = 0;
            while (stream.Position < chunkEnd)
            {
                long matStartIndex = stream.Position;
                uint materialSize = data.ReadUInt32();
                data.Skip(4 + 4); // Priority plane + Material flags
                data.Skip(80); // Shader name?

                data.Skip(4); // LAYS header
                data.Skip(4); // Number of layers
                data.Skip(4); // Layer size (inclusive)

                data.Skip(4 + 4); // Blend mode + Shading flags

                materials[materialIndex++] = new Material { textureID = data.ReadUInt32() };

                stream.Position = matStartIndex + materialSize; // materialSize is inclusive
            }

            model.materials = materials;
        }

        private void ParseChunk_GEOS(BinaryReader data, Stream stream, uint chunkLength)
        {
            long chunkEnd = stream.Position + chunkLength;

            // Pre-calculate how many geosets we have to save memory.
            uint geosetCount = 0;
            while (stream.Position < chunkEnd)
            {
                geosetCount++;
                stream.Seek(data.ReadUInt32() - 4, SeekOrigin.Current);
            }

            stream.Position = chunkEnd - chunkLength;
            Geoset[] geosets = new Geoset[geosetCount];

            uint geosetIndex = 0;
            while (stream.Position < chunkEnd)
            {
                long geosetStart = stream.Position;
                uint geosetSize = data.ReadUInt32();
                long geosetEnd = geosetStart + geosetSize;

                geosets[geosetIndex++] = ParseGeoset(data, stream, geosetEnd);
                stream.Position = geosetEnd;
            }

            model.geosets = geosets;
        }

        private Geoset ParseGeoset(BinaryReader data, Stream stream, long end)
        {
            Geoset geoset = new Geoset{};
            while (stream.Position < end)
            {
                MDXChunks chunkID = (MDXChunks)data.ReadUInt32();
                switch (chunkID)
                {
                    case MDXChunks.VRTX: ParseChunk_VRTX(data, ref geoset); break;
                    case MDXChunks.NRMS: ParseChunk_NRMS(data, ref geoset); break;
                    case MDXChunks.PTYP: ParseChunk_PTYP(data); break;
                    case MDXChunks.PCNT: ParseChunk_PCNT(data); break;
                    case MDXChunks.PVTX: ParseChunk_PVTX(data, ref geoset); break;
                    case MDXChunks.GNDX: ParseChunk_GNDX(data); break;
                    case MDXChunks.MTGC: ParseChunk_MTGC(data); break;
                    case MDXChunks.MATS: ParseChunk_MATS(data, ref geoset); break;
                    case MDXChunks.TANG: ParseChunk_TANG(data); break;
                    case MDXChunks.SKIN: ParseChunk_SKIN(data); break;
                    case MDXChunks.UVAS: ParseChunk_UVAS(data, stream, ref geoset); break;
                    default:
                        // Unknown/unimplemented geoset data. Breakpoint here to evaluate.
                        stream.Position = end;
                        break;
                }
            }
            return geoset;
        }

        private void ParseChunk_TEXS(BinaryReader data, uint chunkLength)
        {
            uint textureCount = chunkLength / 268; // uint32 (0x0?) + char[] 260 + uint32 flags
            string[] textures = new string[textureCount];
            for (int i = 0; i < textureCount; i++)
            {
                data.Skip(4); // Skip header (0x0?)
                textures[i] = Encoding.UTF8.GetString(data.ReadBytes(260)).Replace("\0", string.Empty);
                data.Skip(4); // Skip flag
            }

            model.textures = textures;
        }

        private void ParseChunk_VRTX(BinaryReader data, ref Geoset geoset)
        {
            uint vertCount = data.ReadUInt32();
            Vert[] verts = new Vert[vertCount];

            for (int i = 0; i < vertCount; i++)
                verts[i] = new Vert { x = data.ReadSingle(), y = data.ReadSingle(), z = data.ReadSingle() };

            geoset.verts = verts;
        }

        private void ParseChunk_NRMS(BinaryReader data, ref Geoset geoset)
        {
            uint normalsCount = data.ReadUInt32();
            Normal[] normals = new Normal[normalsCount];

            for (int i = 0; i < normalsCount; i++)
                normals[i] = new Normal { x = data.ReadSingle(), y = data.ReadSingle(), z = data.ReadSingle() };

            geoset.normals = normals;
        }

        private void ParseChunk_PTYP(BinaryReader data)
        {
            // According to documentation, these are always triangles (0x4)
            // Do we ever need to implement this? Just skip it for now.
            data.Skip((int)data.ReadUInt32() * 4);
        }

        private void ParseChunk_PCNT(BinaryReader data)
        {
            // No point implementing this? Primitives are always triangles (3 points).
            data.Skip((int)data.ReadUInt32() * 4);
        }

        private void ParseChunk_PVTX(BinaryReader data, ref Geoset geoset)
        {
            uint primitiveVertCount = data.ReadUInt32();
            uint primitiveCount = primitiveVertCount / 3; // Technically this 3 maps from the PCNT, but it's always 3 so.

            Primitive[] primitives = new Primitive[primitiveCount];
            for (int i = 0; i < primitiveCount; i++)
                primitives[i] = new Primitive { v1 = data.ReadUInt16(), v2 = data.ReadUInt16(), v3 = data.ReadUInt16() };

            geoset.primitives = primitives;
        }

        private void ParseChunk_GNDX(BinaryReader data)
        {
            // Not Yet Implemented
            data.Skip((int)data.ReadUInt32());
        }

        private void ParseChunk_MTGC(BinaryReader data)
        {
            // Not Yet Implemented
            data.Skip((int)data.ReadUInt32() * 4);
        }

        private void ParseChunk_MATS(BinaryReader data, ref Geoset geoset)
        {
            // Not Yet Implemented
            data.Skip((int)data.ReadUInt32() * 4);

            // This seems version specific (900?), may need additional checks.
            geoset.materialIndex = data.ReadUInt32();
            data.Skip(12);

            byte[] geosetName = data.ReadBytes(112); // 112 name bytes?
            geoset.name = Encoding.UTF8.GetString(geosetName).Replace("\0", string.Empty);
        }

        private void ParseChunk_TANG(BinaryReader data)
        {
            // Not Yet Implemented (No idea what this is)
            data.Skip((int)data.ReadUInt32() * 16);
        }

        private void ParseChunk_SKIN(BinaryReader data)
        {
            // Not Yet Implemented
            data.Skip((int)data.ReadUInt32());
        }

        private void ParseChunk_UVAS(BinaryReader data, Stream stream, ref Geoset geoset)
        {
            uint chunkCount = data.ReadUInt32();

            // 900 UVAS contain UVBS sub-chunks. If it doesn't, treat it like an old UVAS.
            // Note: No idea what the use of auxiliary UVBS chunks are, so we ignore them.
            if ((MDXChunks)data.ReadUInt32() != MDXChunks.UVBS)
                stream.Seek(-8, SeekOrigin.Current);

            ParseChunk_UVBS(data, ref geoset);

            // Even though we don't parse auxiliary UVBS chunks, we need to skip them.
            for (int i = 0; i < chunkCount - 1; i++)
            {
                data.Skip(4); // UVBS header.
                data.Skip((int)data.ReadUInt32() * 8);
            }
        }

        private void ParseChunk_UVBS(BinaryReader data, ref Geoset geoset)
        {
            uint uvCount = data.ReadUInt32();
            UV[] uvs = new UV[uvCount];

            for (int i = 0; i < uvCount; i++)
                uvs[i] = new UV { x = data.ReadSingle(), y = data.ReadSingle() };

            geoset.uvs = uvs;
        }
    }
}
