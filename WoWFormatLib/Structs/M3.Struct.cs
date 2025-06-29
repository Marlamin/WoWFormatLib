﻿using System;
using System.Collections.Generic;
using System.Numerics;
using WoWFormatLib.Structs.M2;

namespace WoWFormatLib.Structs.M3
{
    public enum M3Chunks
    {
        M3DT = 'M' << 0 | '3' << 8 | 'D' << 16 | 'T' << 24,
        MES3 = 'M' << 0 | 'E' << 8 | 'S' << 16 | '3' << 24,
        M3VR = 'M' << 0 | '3' << 8 | 'V' << 16 | 'R' << 24,
        VPOS = 'V' << 0 | 'P' << 8 | 'O' << 16 | 'S' << 24,
        VNML = 'V' << 0 | 'N' << 8 | 'M' << 16 | 'L' << 24,
        VUV0 = 'V' << 0 | 'U' << 8 | 'V' << 16 | '0' << 24,
        VUV1 = 'V' << 0 | 'U' << 8 | 'V' << 16 | '1' << 24,
        VUV2 = 'V' << 0 | 'U' << 8 | 'V' << 16 | '2' << 24,
        VUV3 = 'V' << 0 | 'U' << 8 | 'V' << 16 | '3' << 24,
        VUV4 = 'V' << 0 | 'U' << 8 | 'V' << 16 | '4' << 24,
        VUV5 = 'V' << 0 | 'U' << 8 | 'V' << 16 | '5' << 24,
        VTAN = 'V' << 0 | 'T' << 8 | 'A' << 16 | 'N' << 24,
        VSTR = 'V' << 0 | 'S' << 8 | 'T' << 16 | 'R' << 24,
        VINX = 'V' << 0 | 'I' << 8 | 'N' << 16 | 'X' << 24,
        VGEO = 'V' << 0 | 'G' << 8 | 'E' << 16 | 'O' << 24,
        LODS = 'L' << 0 | 'O' << 8 | 'D' << 16 | 'S' << 24,
        RBAT = 'R' << 0 | 'B' << 8 | 'A' << 16 | 'T' << 24,
        VWTS = 'V' << 0 | 'W' << 8 | 'T' << 16 | 'S' << 24,
        VIBP = 'V' << 0 | 'I' << 8 | 'B' << 16 | 'P' << 24,
        VCL0 = 'V' << 0 | 'C' << 8 | 'L' << 16 | '0' << 24,
        VCL1 = 'V' << 0 | 'C' << 8 | 'L' << 16 | '1' << 24,
        M3CL = 'M' << 0 | '3' << 8 | 'C' << 16 | 'L' << 24,
        CPOS = 'C' << 0 | 'P' << 8 | 'O' << 16 | 'S' << 24,
        CNML = 'C' << 0 | 'N' << 8 | 'M' << 16 | 'L' << 24,
        CINX = 'C' << 0 | 'I' << 8 | 'N' << 16 | 'X' << 24,
        M3SI = 'M' << 0 | '3' << 8 | 'S' << 16 | 'I' << 24,
        M3ST = 'M' << 0 | '3' << 8 | 'S' << 16 | 'T' << 24,
        M3VS = 'M' << 0 | '3' << 8 | 'V' << 16 | 'S' << 24,
        M3PT = 'M' << 0 | '3' << 8 | 'P' << 16 | 'T' << 24,
        M3XF = 'M' << 0 | '3' << 8 | 'X' << 16 | 'F' << 24
    }

    public struct M3Model
    {
        public M3DT Header;
        public MES3 Mesh;
        public M3CL Collision;
        public M3SI Instances;
        public M3ST StringTable;
        public M3VS m3vs;
        public M3XF TransformMatrix;
        public M3PT m3pt;
    }

    #region MES3 Mesh
    public struct M3DT
    {
        public uint Version;
        public uint PropertyB;

        public uint Unknown0;
        public ushort Unknown1_a;
        public ushort Unknown1_b;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public int Flags;

        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;
        public float Radius;

        public Vector3 BoundingBox2Min;
        public Vector3 BoundingBox2Max;
        public float Radius2;

        public byte ParticleCount;
        public byte UnkByte0;
        public byte UnkByte1;
        public byte UnkByte2;
    }

    public struct MES3
    {
        public M3VR Version;
        public BUFF Vertices;
        public BUFF Normals;
        public BUFF UV0;
        public BUFF UV1;
        public BUFF UV2;
        public BUFF UV3;
        public BUFF UV4;
        public BUFF UV5;
        public BUFF Tangents;
        public VSTR Names;
        public BUFF Indices;
        public VGEO Geosets;
        public LODS LodLevels;
        public RBAT RenderBatches;
        public VWTS BoneWeights;
        public BUFF InverseBindPoses;
        public BUFF Color0;
        public BUFF Color1;
    }

    public struct M3VR
    {
        public uint Version;
        public uint Unknown0;
    }

    // Not a real chunk, but used for storing generic buffer data
    public struct BUFF
    {
        public string Format;
        public uint Unknown0;
        public byte[] Buffer;
    }

    public struct VSTR
    {
        public uint PropertyA;
        public uint PropertyB;

        public string NameBlock;
    }

    public struct VGEO
    {
        public uint PropertyA;
        public uint PropertyB;

        public M3Geoset[] Geosets;
    }

    public struct M3Geoset
    {
        public uint Unknown0;           // Unknown, first geoset of the model has a value. 8 on 5916032 or 12 on 6648661 & 6655655
        public uint NameCharStart;      // Start of name string in VSTR.
        public uint NameCharCount;      // Number of characters in name string in VSTR.
        public uint IndexStart;
        public uint IndexCount;
        public uint VertexStart;
        public uint VertexCount;
        public uint Unknown1;           // Always 0, could be for bones.
        public uint Unknown2;           // Always 0, could be for bones.
    }

    public struct LODS
    {
        public uint LODCount;
        public uint GeosetCount;
     
        public M3LodLevel[] LodLevels;
    }

    public struct M3LodLevel
    {
        public uint VertexCount;
        public uint IndexCount;
    }

    public struct RBAT
    {
        public uint BatchCount;
        public uint PropertyB;

        public M3RenderBatch[] RenderBatches;
    }

    public struct M3RenderBatch
    {
        public uint Unknown0; // Always 0
        public uint Unknown1; // Always 0
        public uint GeosetIndex;
        public uint MaterialIndex;
    }

    public struct VWTS
    {
        public string IndicesFormat;
        public string WeightsFormat;
        public byte[] Buffer;
    }
    #endregion

    #region M3CL Collision
    public struct M3CL
    {
        public uint PropertyA;
        public uint PropertyB;
        public CPOS Vertices;
        public CNML Normals;
        public CINX Indices;
    }

    public struct CPOS
    {
        public Vector3[] Vertices;
    }
    public struct CNML
    {
        public Vector3[] Normals;
    }
    public struct CINX
    {
        public ushort[] Indices;
    }
    #endregion

    #region M3SI Instances
    public struct M3SI
    {
        public uint PropertyA;
        public uint PropertyB;

        public string Magic;
        public uint Version;
        public uint TotalSize;
        public uint Unknown1;
        public uint InstanceCount;
        public M3Instance[] Instances;
    }

    public struct M3Instance
    {
        public uint FileDataID;
        public byte[] GUID;
        public uint Unknown0;
        public uint BlendMode;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public ShaderData shaderData;
    }

    public struct ShaderData
    {
        public int uniformCount;
        public int uniformDataSize;
        public int uniformHashesOffset;
        public int uniformTypesOffset;
        public int uniformLocationsOffset;
        public int uniformDataOffset;
        public int samplerCount;
        public int samplerHashesOffset;
        public int samplerTextureFileIDsOffset;
        public int unkDataCount;
        public int unkDataHashesOffset;
        public int unkDataOffset;

        public List<ulong> UniformHashes;
        public List<ushort> UniformTypes;
        public List<int> UniformLocations;
        public List<object[]> UniformData;

        public List<ulong> SamplerHashes;
        public List<int> SamplerTextureFileIDs;

        public List<ulong> UnkDataHashes;
        public List<UnkData> UnkData;
    }

    public struct UnkData
    {
        public byte unk0;
        public byte unk1;
        public ushort unk2;
    }
    #endregion

    public struct M3ST
    {

    }

    public struct M3VS
    {

    }

    public struct M3XF
    {
        public Matrix4x4 Transform;
    }

    public struct M3PT
    {
    }
}
