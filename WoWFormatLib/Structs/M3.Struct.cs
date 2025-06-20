using System;
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
    }

    public struct M3Model
    {
        public M3DT Header;
        public MES3 Mesh;
        public M3CL Collision;
        public M3SI Instances;
        public M3ST StringTable;
        public M3VS m3vs;
        public M3XF m3xf;
        public M3PT m3pt;
    }

    #region MES3 Mesh
    public struct M3DT
    {
        public uint PropertyA;
        public uint PropertyB;

        public uint Unknown0;
        public uint Unknown1;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint Unknown6;

        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;
        public float Radius;

        public Vector3 BoundingBox2Min;
        public Vector3 BoundingBox2Max;
        public float Radius2;

        public uint Unknown7;
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
        public VSTR MaterialString;
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

        public string MaterialName;
    }

    public struct VGEO
    {
        public uint PropertyA;
        public uint PropertyB;

        public M3Geoset[] Geosets;
    }

    public struct M3Geoset
    {
        public uint Unknown0;
        public uint Unknown1;
        public uint Unknown2;
        public uint IndicesIndex;
        public uint IndicesCount;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint Unknown6;
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
        public uint Unknown0;
        public uint Unknown1;
        public uint Unknown2;
        public uint Unknown3;
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
        public byte[] MD5;
        public uint Unknown0;
        public uint Unknown1;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint Unknown6;
        public uint Unknown7;
        public uint Unknown8;
        public uint Unknown9;
        public uint Unknown10;
        public uint Unknown11;
        public uint Unknown12;
        public uint Unknown13;
        public uint Unknown14;
        public uint Unknown15;
        public uint Unknown16;
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

    }

    public struct M3PT
    {
    }
}
