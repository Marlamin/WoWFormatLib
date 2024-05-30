using System;
using System.ComponentModel;
using System.Numerics;
namespace WoWFormatLib.Structs.ADT
{
    public enum ADTChunks
    {
        // Root ADT
        MVER = 'M' << 24 | 'V' << 16 | 'E' << 8 | 'R' << 0,
        MCNK = 'M' << 24 | 'C' << 16 | 'N' << 8 | 'K' << 0,
        MHDR = 'M' << 24 | 'H' << 16 | 'D' << 8 | 'R' << 0,
        MH2O = 'M' << 24 | 'H' << 16 | '2' << 8 | 'O' << 0,
        MFBO = 'M' << 24 | 'F' << 16 | 'B' << 8 | 'O' << 0,
        MBMH = 'M' << 24 | 'B' << 16 | 'M' << 8 | 'H' << 0,
        MBBB = 'M' << 24 | 'B' << 16 | 'B' << 8 | 'B' << 0,
        MBMI = 'M' << 24 | 'B' << 16 | 'M' << 8 | 'I' << 0,
        MBNV = 'M' << 24 | 'B' << 16 | 'N' << 8 | 'V' << 0,
        MCIN = 'M' << 24 | 'C' << 16 | 'I' << 8 | 'N' << 0, // Removed in Cataclysm.

        // Root MCNK
        MCVT = 'M' << 24 | 'C' << 16 | 'V' << 8 | 'T' << 0,
        MCCV = 'M' << 24 | 'C' << 16 | 'C' << 8 | 'V' << 0,
        MCNR = 'M' << 24 | 'C' << 16 | 'N' << 8 | 'R' << 0,
        MCSE = 'M' << 24 | 'C' << 16 | 'S' << 8 | 'E' << 0,
        MCBB = 'M' << 24 | 'C' << 16 | 'B' << 8 | 'B' << 0,
        MCLQ = 'M' << 24 | 'C' << 16 | 'L' << 8 | 'Q' << 0,
        MCLV = 'M' << 24 | 'C' << 16 | 'L' << 8 | 'V' << 0,

        // OBJ
        MMDX = 'M' << 24 | 'M' << 16 | 'D' << 8 | 'X' << 0,
        MMID = 'M' << 24 | 'M' << 16 | 'I' << 8 | 'D' << 0,
        MWMO = 'M' << 24 | 'W' << 16 | 'M' << 8 | 'O' << 0,
        MWID = 'M' << 24 | 'W' << 16 | 'I' << 8 | 'D' << 0,
        MDDF = 'M' << 24 | 'D' << 16 | 'D' << 8 | 'F' << 0,
        MODF = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'F' << 0,
        MLMB = 'M' << 24 | 'L' << 16 | 'M' << 8 | 'B' << 0,
        MWDS = 'M' << 24 | 'W' << 16 | 'D' << 8 | 'S' << 0,
        MWDR = 'M' << 24 | 'W' << 16 | 'D' << 8 | 'R' << 0,

        // TEX

        [Obsolete("Replaced by MDID/MHID FileDataID chunks in 8.1.")]
        [Description("Array of texture filenames")]
        MTEX = 'M' << 24 | 'T' << 16 | 'E' << 8 | 'X' << 0, // Removed in 8.1.

        MTXF = 'M' << 24 | 'T' << 16 | 'X' << 8 | 'F' << 0, // Added in 3.x
        MAMP = 'M' << 24 | 'A' << 16 | 'M' << 8 | 'P' << 0, // Added in 4.x.
        MTXP = 'M' << 24 | 'T' << 16 | 'X' << 8 | 'P' << 0, // Added in 5.x.
        MDID = 'M' << 24 | 'D' << 16 | 'I' << 8 | 'D' << 0, // Added in 8.1.
        MHID = 'M' << 24 | 'H' << 16 | 'I' << 8 | 'D' << 0, // Added in 8.1.
        MTCG = 'M' << 24 | 'T' << 16 | 'C' << 8 | 'G' << 0, // Added in SL

        // TEX MCNK
        MCLY = 'M' << 24 | 'C' << 16 | 'L' << 8 | 'Y' << 0,
        MCAL = 'M' << 24 | 'C' << 16 | 'A' << 8 | 'L' << 0,
        MCSH = 'M' << 24 | 'C' << 16 | 'S' << 8 | 'H' << 0,
        MCMT = 'M' << 24 | 'C' << 16 | 'M' << 8 | 'T' << 0,

        // LOD
        MLHD = 'M' << 24 | 'L' << 16 | 'H' << 8 | 'D' << 0,
        MLVH = 'M' << 24 | 'L' << 16 | 'V' << 8 | 'H' << 0,
        MLVI = 'M' << 24 | 'L' << 16 | 'V' << 8 | 'I' << 0,
        MLLL = 'M' << 24 | 'L' << 16 | 'L' << 8 | 'L' << 0,
        MLND = 'M' << 24 | 'L' << 16 | 'N' << 8 | 'D' << 0,
        MLSI = 'M' << 24 | 'L' << 16 | 'S' << 8 | 'I' << 0,
        MBMB = 'M' << 24 | 'B' << 16 | 'M' << 8 | 'B' << 0,
        MLLD = 'M' << 24 | 'L' << 16 | 'L' << 8 | 'D' << 0,
        MLLN = 'M' << 24 | 'L' << 16 | 'L' << 8 | 'N' << 0,
        MLLI = 'M' << 24 | 'L' << 16 | 'L' << 8 | 'I' << 0,
        MLLV = 'M' << 24 | 'L' << 16 | 'L' << 8 | 'V' << 0,
    }

    public struct ADT
    {
        public uint version;
        public byte x;
        public byte y;
        public MHDR header;
        public MTEX textures;
        public MTXP[] texParams;
        public MTXF texFlags;
        public MCNK[] chunks;
        public MH2O mh2o;
        public Obj objects;
        public uint[] diffuseTextureFileDataIDs;
        public uint[] heightTextureFileDataIDs;
        public MTCG[] textureColorGradings;
    }

    public struct MTCG
    {
        public uint startDistance;
        public uint unknown;
        public uint colorGradingFDID;
        public uint colorGradingRampFDID;
    }

    public struct MCIN
    {
        public uint offset;
        public uint size;
        public uint flags;
        public uint asyncID;
    }

    public enum MHDRFlags
    {
        mhdr_MFBO = 1,                // contains a MFBO chunk.
        mhdr_northrend = 2,           // is set for some northrend ones.
    }

    public struct MHDR
    {
        public uint flags;
        public uint ofsMCIN;
        public uint ofsMTEX;
        public uint ofsMMDX;
        public uint ofsMMID;
        public uint ofsMWMO;
        public uint ofsMWID;
        public uint ofsMDDF;
        public uint ofsMODF;
        public uint ofsMFBO;
        public uint ofsMH2O;
        public uint ofsMTXF;
        public uint unk1;
        public uint unk2;
        public uint unk3;
        public uint unk4;
    }

    [Flags]
    public enum MCNKFlags : uint
    {
        mcnk_has_mcsh = 0x1,
        mcnk_impass   = 0x2,
        mcnk_lq_river = 0x4,
        mcnk_lq_ocean = 0x8,
        mcnk_lq_magma = 0x10,
        mcnk_lg_slime = 0x20,
        mcnk_has_mccv = 0x40,
        mcnk_unk_0x80 = 0x80,
        mcnk_unk_0x100 = 0x100,
        mcnk_unk_0x200 = 0x200,
        mcnk_unk_0x400 = 0x400,
        mcnk_unk_0x800 = 0x800,
        mcnk_unk_0x1000 = 0x1000,
        mcnk_unk_0x2000 = 0x2000,
        mcnk_unk_0x4000 = 0x4000,
        mcnk_dont_fix_alpha_map = 0x8000,
        mcnk_high_res_holes = 0x10000
    }

    public struct MCNKheader
    {
        public MCNKFlags flags;
        public uint indexX;
        public uint indexY;
        public uint nLayers;
        public uint nDoodadRefs;
        public byte holesHighRes_0;
        public byte holesHighRes_1;
        public byte holesHighRes_2;
        public byte holesHighRes_3;
        public byte holesHighRes_4;
        public byte holesHighRes_5;
        public byte holesHighRes_6;
        public byte holesHighRes_7;
        public uint ofsMCLY;
        public uint ofsMCRF;
        public uint ofsMCAL;
        public uint sizeAlpha;
        public uint ofsMCSH;
        public uint sizeShadows;
        public uint areaID;
        public uint nMapObjRefs;
        public ushort holesLowRes;
        public ushort unknownPad;
        public short lowQualityTexturingMap_0;
        public short lowQualityTexturingMap_1;
        public short lowQualityTexturingMap_2;
        public short lowQualityTexturingMap_3;
        public short lowQualityTexturingMap_4;
        public short lowQualityTexturingMap_5;
        public short lowQualityTexturingMap_6;
        public short lowQualityTexturingMap_7;
        public long noEffectDoodad;
        public uint ofsMCSE;
        public uint numMCSE;
        public uint ofsMCLQ;
        public uint sizeMCLQ;
        public Vector3 position;
        public uint ofsMCCV;
        public uint ofsMCLV;
        public uint unused;
    }

    public struct MCNK
    {
        public MCNKheader header;
        public MCVT vertices;
        public MCNR normals;
        public MCLV colors;
        public MCCV vertexShading;
        public MCSE soundEmitters;
        public MCBB[] blendBatches;
        public MCLY[] layers;
        public MCAL[] alphaLayer;
    }

    public struct Obj
    {
        public MDDF models;
        public MMDX m2Names;
        public MMID m2NameOffsets;

        public MODF worldModels;
        public MWMO wmoNames;
        public MWID wmoNameOffsets;

        public MWDR[] worldModelDoodadRefs;
        public uint[] worldModelDoodadSets;

    }

    //WMO placement
    public struct MODF
    {
        public MODFEntry[] entries;
    }

    public struct MODFEntry
    {
        public uint mwidEntry;
        public uint uniqueId;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 lowerBounds;
        public Vector3 upperBounds;
        public MODFFlags flags;
        public ushort doodadSet;
        public ushort nameSet;
        public ushort scale;
    }

    [Flags]
    public enum MODFFlags : ushort
    {
        modf_destroyable = 0x1,
        modf_use_lod = 0x2,
        modf_0x4_unk = 0x4,
        modf_entry_is_filedataid = 0x8,
        modf_use_sets_from_mwds = 0x80,
    }

    public struct MWDR
    {
        public uint begin;
        public uint end;
    }

    //M2 placement
    public struct MDDF
    {
        public MDDFEntry[] entries;
    }

    public struct MDDFEntry
    {
        public uint mmidEntry;
        public uint uniqueId;
        public Vector3 position;
        public Vector3 rotation;
        public ushort scale;
        public MDDFFlags flags;
    }

    [Flags]
    public enum MDDFFlags : ushort
    {
        mddf_biodome = 0x1,
        mddf_shrubbery = 0x2, //probably deprecated < 18179
        mddf_0x4 = 0x4,
        mddf_0x8 = 0x8,
        mddf_0x10 = 0x10,
        mddf_liquid_known = 0x20,
        mddf_entry_is_filedataid = 0x40,
        mddf_0x80 = 0x80,
        mddf_0x100 = 0x100,
        mddf_0x200 = 0x200,
    }

    //List of filenames for M2 models that appear in this map tile.
    public struct MMDX
    {
        public string[] filenames; // zero-terminated strings with complete paths to models. Referenced in MMID.
        public uint[] offsets; //not part of official struct, filled manually during parsing with where the string started
    }

    //List of offsets of M2 filenames in the MMDX chunk.
    public struct MMID
    {
        public uint[] offsets; // filename starting position in MMDX chunk. These entries are getting referenced in the MDDF chunk.
    }

    //List of offsets of WMO filenames in the MWMO chunk.
    public struct MWID
    {
        public uint[] offsets; // filename starting position in MWMO chunk. These entries are getting referenced in the MODF chunk.
    }

    //List of filenames for WMOs (world map objects) that appear in this map tile.
    public struct MWMO
    {
        public string[] filenames;
        public uint[] offsets; //not part of official struct, filled manually during parsing with where the string started
    }

    public struct MCVT
    {
        public float[] vertices; //make manually, 145
    }

    public struct MCLV
    {
        public ushort[] color; //make manually, 145
    }

    public struct MCLY
    {
        public uint textureId;
        public MCLYFlags flags;
        public uint offsetInMCAL;
        public int effectId;
    }

    public struct MCSE
    {
        public byte[] raw; //TODO
    }

    public struct MCBB
    {
        public uint mbmhIndex;
        public uint indexCount;
        public uint indexFirst;
        public uint vertexCount;
        public uint vertexFirst;
    }

    public struct MH2O
    {
        public MH2OHeader[] headers;
        public MH2OAttribute[][] attributes;
        public MH2OInstance[][] instances;
        public MH2OVertexData[][] vertexData;
    }

    public struct MH2OHeader
    {
        public uint offsetInstances;
        public uint layerCount;
        public uint offsetAttributes;
    }

    public struct MH2OAttribute
    {
        public ulong fishable;
        public ulong deep;
    }

    public struct MH2OInstance
    {
        public ushort liquidType;
        public ushort liquidObjectOrLVF;
        public float min_height_level;
        public float max_height_level;
        public byte xOffset;
        public byte yOffset;
        public byte width;
        public byte height;
        public uint offsetExistsBitmap;
        public uint offsetVertexData;
    }

    public struct MH2OVertexData
    {
        public byte liquidVertexFormat; // "case 0-3" as per wiki
        public byte[] vertexData;
    }

    [Flags]
    public enum MCLYFlags : uint
    {
        mcly_animate_rot_45 = 0x1,
        mcly_animate_rot_90 = 0x2,
        mcly_animate_rot_180 = 0x4,
        mcly_animate_speed_fast = 0x8,
        mcly_animate_speed_faster = 0x10,
        mcly_animate_speed_fastest = 0x20,
        mcly_animation_enabled = 0x40,
        mcly_overbright = 0x80,
        mcly_use_alpha_map = 0x100,
        mcly_alpha_map_compressed = 0x200,
        mcly_use_cubemap_reflection = 0x400,
        mcly_unk_apply_scale1 = 0x800,
        mcly_unk_apply_scale2 = 0x1000
    }

    public struct MCAL
    {
        public byte[] layer;
    }

    public struct MCCV
    {
        public byte[] red;
        public byte[] green;
        public byte[] blue;
        public byte[] alpha;
    }

    public struct MCNR
    {
        public short[] normal_0;
        public short[] normal_1;
        public short[] normal_2;
    }

    public struct MTEX
    {
        public string[] filenames;
    }

    public struct MTXF
    {
        public uint[] flags;
    }

    public struct MTXP
    {
        public uint flags;
        public float height;
        public float offset;
        public uint unk3;
    }

    /* _lod */
    public struct LODADT
    {
        public float[] heights;
        public short[] indices;
        public MLLLEntry[] lodLevels;
        public MLNDEntry[] quadTree;
        public short[] skirtIndices;
    }

    public struct MLLLEntry
    {
        public float lod;
        public uint heightLength;
        public uint heightIndex;
        public uint mapAreaLowLength;
        public uint mapAreaLowIndex;
    }

    public struct MLNDEntry
    {
        public uint index;
        public uint length;
        public uint unk0;
        public uint unk1;
        public short indice_1;
        public short indice_2;
        public short indice_3;
        public short indice_4;
    }
}
