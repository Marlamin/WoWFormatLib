using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.Serialization;

namespace WoWFormatLib.Structs.WMO
{
    public enum WMOChunks
    {
        // Root WMO
        MVER = 'M' << 24 | 'V' << 16 | 'E' << 8 | 'R' << 0,
        MOHD = 'M' << 24 | 'O' << 16 | 'H' << 8 | 'D' << 0,
        MOTX = 'M' << 24 | 'O' << 16 | 'T' << 8 | 'X' << 0,
        MOMT = 'M' << 24 | 'O' << 16 | 'M' << 8 | 'T' << 0,
        MOGN = 'M' << 24 | 'O' << 16 | 'G' << 8 | 'N' << 0,
        MOGI = 'M' << 24 | 'O' << 16 | 'G' << 8 | 'I' << 0,
        MODS = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'S' << 0,
        MODN = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'N' << 0,
        MODD = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'D' << 0,
        MOSB = 'M' << 24 | 'O' << 16 | 'S' << 8 | 'B' << 0,
        GFID = 'G' << 24 | 'F' << 16 | 'I' << 8 | 'D' << 0,
        MOPV = 'M' << 24 | 'O' << 16 | 'P' << 8 | 'V' << 0,
        MOPR = 'M' << 24 | 'O' << 16 | 'P' << 8 | 'R' << 0,
        MOPT = 'M' << 24 | 'O' << 16 | 'P' << 8 | 'T' << 0,
        MOVV = 'M' << 24 | 'O' << 16 | 'V' << 8 | 'V' << 0,
        MOVB = 'M' << 24 | 'O' << 16 | 'V' << 8 | 'B' << 0,
        MOLT = 'M' << 24 | 'O' << 16 | 'L' << 8 | 'T' << 0,
        MFOG = 'M' << 24 | 'F' << 16 | 'O' << 8 | 'G' << 0,
        MCVP = 'M' << 24 | 'C' << 16 | 'V' << 8 | 'P' << 0,
        MOUV = 'M' << 24 | 'O' << 16 | 'U' << 8 | 'V' << 0,
        MOSI = 'M' << 24 | 'O' << 16 | 'S' << 8 | 'I' << 0,
        MDDL = 'M' << 24 | 'D' << 16 | 'D' << 8 | 'L' << 0,
        MGI2 = 'M' << 24 | 'G' << 16 | 'I' << 8 | '2' << 0,
        MDDI = 'M' << 24 | 'D' << 16 | 'D' << 8 | 'I' << 0,
        MNLD = 'M' << 24 | 'N' << 16 | 'L' << 8 | 'D' << 0,
        MFED = 'M' << 24 | 'F' << 16 | 'E' << 8 | 'D' << 0,
        MAVG = 'M' << 24 | 'A' << 16 | 'V' << 8 | 'G' << 0,
        MFVR = 'M' << 24 | 'F' << 16 | 'V' << 8 | 'R' << 0,
        MAVD = 'M' << 24 | 'A' << 16 | 'V' << 8 | 'D' << 0,
        MPVD = 'M' << 24 | 'P' << 16 | 'V' << 8 | 'D' << 0,
        MBVD = 'M' << 24 | 'B' << 16 | 'V' << 8 | 'D' << 0,
        MOMX = 'M' << 24 | 'O' << 16 | 'M' << 8 | 'X' << 0,

        // Group WMO
        MOGP = 'M' << 24 | 'O' << 16 | 'G' << 8 | 'P' << 0,
        MOVI = 'M' << 24 | 'O' << 16 | 'V' << 8 | 'I' << 0,
        MOVT = 'M' << 24 | 'O' << 16 | 'V' << 8 | 'T' << 0,
        MOTV = 'M' << 24 | 'O' << 16 | 'T' << 8 | 'V' << 0,
        MONR = 'M' << 24 | 'O' << 16 | 'N' << 8 | 'R' << 0,
        MOBA = 'M' << 24 | 'O' << 16 | 'B' << 8 | 'A' << 0,
        MOPY = 'M' << 24 | 'O' << 16 | 'P' << 8 | 'Y' << 0,
        MOBS = 'M' << 24 | 'O' << 16 | 'B' << 8 | 'S' << 0,
        MODR = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'R' << 0,
        MOBN = 'M' << 24 | 'O' << 16 | 'B' << 8 | 'N' << 0,
        MOBR = 'M' << 24 | 'O' << 16 | 'B' << 8 | 'R' << 0,
        MOLR = 'M' << 24 | 'O' << 16 | 'L' << 8 | 'R' << 0,
        MOCV = 'M' << 24 | 'O' << 16 | 'C' << 8 | 'V' << 0,
        MDAL = 'M' << 24 | 'D' << 16 | 'A' << 8 | 'L' << 0,
        MLIQ = 'M' << 24 | 'L' << 16 | 'I' << 8 | 'Q' << 0,
        MOTA = 'M' << 24 | 'O' << 16 | 'T' << 8 | 'A' << 0,
        MOPL = 'M' << 24 | 'O' << 16 | 'P' << 8 | 'L' << 0,
        MOLP = 'M' << 24 | 'O' << 16 | 'L' << 8 | 'P' << 0,
        MOLS = 'M' << 24 | 'O' << 16 | 'L' << 8 | 'S' << 0,
        MOPB = 'M' << 24 | 'O' << 16 | 'P' << 8 | 'B' << 0,
        MLSP = 'M' << 24 | 'L' << 16 | 'S' << 8 | 'P' << 0,
        MODI = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'I' << 0,
        MLSS = 'M' << 24 | 'L' << 16 | 'S' << 8 | 'S' << 0,
        MLSK = 'M' << 24 | 'L' << 16 | 'S' << 8 | 'K' << 0,
        MOP2 = 'M' << 24 | 'O' << 16 | 'P' << 8 | '2' << 0,
        MNLR = 'M' << 24 | 'N' << 16 | 'L' << 8 | 'R' << 0,
        MAVR = 'M' << 24 | 'A' << 16 | 'V' << 8 | 'R' << 0,
        MPVR = 'M' << 24 | 'P' << 16 | 'V' << 8 | 'R' << 0,
        MBVR = 'M' << 24 | 'B' << 16 | 'V' << 8 | 'R' << 0,
        MOLV = 'M' << 24 | 'O' << 16 | 'L' << 8 | 'V' << 0,
        MOGX = 'M' << 24 | 'O' << 16 | 'G' << 8 | 'X' << 0,
        MPY2 = 'M' << 24 | 'P' << 16 | 'Y' << 8 | '2' << 0,
        MOQG = 'M' << 24 | 'O' << 16 | 'Q' << 8 | 'G' << 0,
        MOC2 = 'M' << 24 | 'O' << 16 | 'C' << 8 | '2' << 0,
        MLSO = 'M' << 24 | 'L' << 16 | 'S' << 8 | 'O' << 0,

    }

    public struct WMO
    {
        public MOHD header;
        public uint version;
        public MOTX[] textures;
        public MOMT[] materials;
        public MODN[] doodadNames;
        public uint[] doodadIds;
        public MODD[] doodadDefinitions;
        public MODS[] doodadSets;
        public MOGN[] groupNames;
        public MOGI[] groupInfo;
        public WMOGroupFile[] group;
        public uint[] groupFileDataIDs;
        public string skybox;
        public uint skyboxFileDataID;
        public MGI2[] groupInfo2;
    }

    public enum MOHDFlags : short
    {
        mohd_do_not_attenuate_vertices_based_on_distance_to_portal = 0x1,
        mohd_use_unified_render_path = 0x2,
        mohd_use_liquid_type_dbc_id = 0x4,
        mohd_do_not_fix_vertex_color_alpha = 0x8,
        mohd_lod = 0x10,
        mohd_default_max_lod = 0x20,
        Flag_0x40 = 0x40,
        Flag_0x80 = 0x80,
        Flag_0x100 = 0x100,
        Flag_0x200 = 0x200,
        Flag_0x400 = 0x400,
        Flag_0x800 = 0x800
    }

    public struct MOHD
    {
        public uint nMaterials;
        public uint nGroups;
        public uint nPortals;
        public uint nLights;
        public uint nModels;
        public uint nDoodads;
        public uint nSets;
        public uint ambientColor;
        public uint areaTableID;
        public Vector3 boundingBox1;
        public Vector3 boundingBox2;
        public MOHDFlags flags;
        public short nLod;
    }

    public struct MODN
    {
        public string filename;
        public uint startOffset;
    }

    [Flags]
    public enum MODDFlags : byte
    {
        modd_accept_proj_texture = 0x1,
        modd_use_interior_lighting = 0x2,
        modd_flag_0x4 = 0x4,
        modd_flag_0x8 = 0x8,
        modd_flag_0x10 = 0x10,
        modd_flag_0x20 = 0x20,
        modd_flag_0x40 = 0x40,
        modd_flag_0x80 = 0x80
    }

    public struct MODD
    {
        public uint offset;
        public byte flags;
        public Vector3 position;
        public Quaternion rotation;
        public float scale;
        public byte[] color;
    }

    public struct MODS
    {
        public string setName;
        public uint firstInstanceIndex;
        public uint numDoodads;
        public uint unused;
    }

    //Texture filenames
    public struct MOTX
    {
        public string filename;
        public uint startOffset;
    }

    public struct MOMT
    {
        public MOMTFlags flags;
        public MOMTShader shader;
        public uint blendMode;
        public uint texture1;
        public uint color1;
        public uint color1b;
        public uint texture2;
        public uint color2;
        public uint groundType;
        public uint texture3;
        public uint color3;
        public uint flags3;
        public uint runtimeData0;
        public uint runtimeData1;
        public uint runtimeData2;
        public uint runtimeData3;
    }


    [Flags]
    public enum MOMTFlags : uint
    {
        momt_unlit = 0x1,
        momt_unfogged = 0x2,
        momt_unculled = 0x4,
        momt_extlight = 0x8,
        momt_sidn = 0x10,
        momt_window = 0x20,
        momt_clamp_s = 0x40,
        momt_clamp_t = 0x80,
        Flag_0x100 = 0x100,
        Flag_0x200 = 0x200,
        Flag_0x400 = 0x400,
        Flag_0x800 = 0x800,
        Flag_0x1000 = 0x1000,
        Flag_0x2000 = 0x2000,
        Flag_0x4000 = 0x4000,
        Flag_0x8000 = 0x8000,
        Flag_0x10000 = 0x10000,
        Flag_0x20000 = 0x20000,
        Flag_0x40000 = 0x40000,
        Flag_0x80000 = 0x80000,
        Flag_0x100000 = 0x100000,
        Flag_0x200000 = 0x200000,
        Flag_0x400000 = 0x400000,
        Flag_0x800000 = 0x800000
    }

    public enum MOMTShader : uint
    {
        [EnumMember(Value = "0: Diffuse (VS: MapObjDiffuse_T1, PS: MapObjDiffuse)")]
        Diffuse = 0,
        [EnumMember(Value = "1: Specular (VS: MapObjSpecular_T1, PS: MapObjSpecular)")]
        Specular = 1,
        [EnumMember(Value = "2: Metal (VS: MapObjSpecular_T1, PS: MapObjMetal)")]
        Metal = 2,
        [EnumMember(Value = "3: Env (VS: MapObjDiffuse_T1_Refl, PS: MapObjEnv)")]
        Env = 3,
        [EnumMember(Value = "4: Opaque (VS: MapObjDiffuse_T1, PS: MapObjOpaque)")]
        Opaque = 4,
        [EnumMember(Value = "5: EnvMetal (VS: MapObjDiffuse_T1_Refl, PS: MapObjEnvMetal)")]
        EnvMetal = 5,
        [EnumMember(Value = "6: TwoLayerDiffuse (VS: MapObjDiffuse_Comp, PS: MapObjTwoLayerDiffuse)")]
        TwoLayerDiffuse = 6,
        [EnumMember(Value = "7: TwoLayerEnvMetal (VS: MapObjDiffuse_T1, PS: MapObjTwoLayerEnvMetal)")]
        TwoLayerEnvMetal = 7,
        [EnumMember(Value = "8: TwoLayerTerrain (VS: MapObjDiffuse_Comp_Terrain, PS: MapObjTwoLayerTerrain)")]
        TwoLayerTerrain = 8,
        [EnumMember(Value = "9: DiffuseEmissive (VS: MapObjDiffuse_Comp, PS: MapObjDiffuseEmissive)")]
        DiffuseEmissive = 9,
        [EnumMember(Value = "10: WaterWindow (VS: FFXWaterWindow, PS: FFXWaterWindow)")]
        WaterWindow = 10,
        [EnumMember(Value = "11: MaskedEnvMetal (VS: MapObjDiffuse_T1_Env_T2, PS: MapObjMaskedEnvMetal)")]
        MaskedEnvMetal = 11,
        [EnumMember(Value = "12: MaskedEnvMetalEmissive (VS: MapObjDiffuse_T1_Env_T2, PS: MapObjEnvMetalEmissive)")]
        EnvMetalEmissive = 12,
        [EnumMember(Value = "13: TwoLayerDiffuseOpaque (VS: MapObjDiffuse_Comp, PS: MapObjTwoLayerDiffuseOpaque)")]
        TwoLayerDiffuseOpaque = 13,
        [EnumMember(Value = "14: SubmarineWindow (VS: FFXSubmarineWindow, PS: FFXSubmarineWindow)")]
        SubmarineWindow = 14,
        [EnumMember(Value = "15: TwoLayerDiffuseEmissive (VS: MapObjDiffuse_Comp, PS: MapObjTwoLayerDiffuseEmissive)")]
        TwoLayerDiffuseEmissive = 15,
        [EnumMember(Value = "16: DiffuseTerrain (VS: MapObjDiffuse_T1, PS: MapObjDiffuse)")]
        DiffuseTerrain = 16,
        [EnumMember(Value = "17: AdditiveMaskedEnvMetal (VS: MapObjDiffuse_T1_Env_T2, PS: MapObjAdditiveMaskedEnvMetal)")]
        AdditiveMaskedEnvMetal = 17,
        [EnumMember(Value = "18: TwoLayerDiffuseMod2x (VS: MapObjDiffuse_CompAlpha, PS: MapObjTwoLayerDiffuseMod2x)")]
        TwoLayerDiffuseMod2x = 18,
        [EnumMember(Value = "19: TwoLayerDiffuseMod2xNA (VS: MapObjDiffuse_Comp, PS: MapObjTwoLayerDiffuseMod2xNA)")]
        TwoLayerDiffuseMod2xNA = 19,
        [EnumMember(Value = "20: TwoLayerDiffuseAlpha (VS: MapObjDiffuse_CompAlpha, PS: MapObjTwoLayerDiffuseAlpha)")]
        TwoLayerDiffuseAlpha = 20,
        [EnumMember(Value = "21: Lod (VS: MapObjDiffuse_T1, PS: MapObjLod)")]
        Lod = 21,
        [EnumMember(Value = "22: Parallax (VS: MapObjParallax, PS: MapObjParallax)")]
        Parallax = 22,
        [EnumMember(Value = "23: DF_MoreTexture_Unknown (VS: MapObjDiffuse_T1, PS: MapObjUnkDFMoreTextureShader)")]
        DF_MoreTexture_Unknown = 23
    }

    //Group names
    public struct MOGN
    {
        public string name;
        public int offset;
    }

    //Group information
    public struct MOGI
    {
        public MOGPFlags flags;
        public Vector3 boundingBox1;
        public Vector3 boundingBox2;
        public int nameIndex; //something else
    }

    public struct MGI2
    {
        public MOGPFlags2 flags2;
        public uint lodIndex; // probably not this
    }

    [Flags]
    public enum MOGPFlags : uint
    {
        mogp_bsp_tree = 0x1, //Has MOBN and MOBR chunk.
        mogp_light_map = 0x2,
        mogp_has_vertex_colors = 0x4, //Has vertex colors (MOCV chunk)
        moigp_exterior = 0x8, //Outdoor
        mogp_unk_0x10 = 0x10,
        mogp_unk_0x20 = 0x20,
        mogp_exterior_lit = 0x40,
        mogp_unreachable = 0x80,
        mogp_show_exterior_sky_indoors = 0x100,
        mogp_has_lights = 0x200, //Has lights  (MOLR chunk)
        mogp_lod = 0x400, //Has MPBV, MPBP, MPBI, MPBG chunks.
        mogp_has_doodads = 0x800, //Has doodads (MODR chunk)
        mogp_has_water = 0x1000, //Has water   (MLIQ chunk)
        mogp_interior = 0x2000, //Indoor
        mogp_unk_0x4000 = 0x4000,
        mogp_unk_0x8000 = 0x8000, // probably unused now?
        mogp_alwaysdraw = 0x10000,
        mogp_unk_0x20000 = 0x20000, //Has MORI and MORB chunks, unused now
        mogp_show_skybox = 0x40000, //Show skybox
        mogp_is_not_water_but_ocean = 0x80000, //isNotOcean, LiquidType related, see below in the MLIQ chunk.
        mogp_unk_0x100000 = 0x100000,
        mogp_is_mount_allowed = 0x200000,
        mogp_unk_0x400000 = 0x400000,
        mogp_unk_0x800000 = 0x800000,
        mogp_has_vertex_colors2 = 0x1000000, //SMOGroup::CVERTS2: Has two MOCV chunks: Just add two or don't set 0x4 to only use cverts2.
        mogp_has_uv2 = 0x2000000, //SMOGroup::TVERTS2: Has two MOTV chunks: Just add two.
        mogp_antiportal = 0x4000000,
        mogp_unk_0x8000000 = 0x8000000,
        mogp_unk_0x10000000 = 0x10000000,
        mogp_unk_0x20000000 = 0x20000000,
        mogp_has_uv3 = 0x40000000, // SMOGroup::TVERTS3: Has three MOTV chunks, eg. for MOMT with shader 18.
        mogp_unk_0x80000000 = 0x80000000
    }

    [Flags]
    public enum MOGPFlags2 : uint
    { 
        mogp2_can_cut_terrain = 0x1,
        mogp2_unk_0x2 = 0x2,
        mogp2_unk_0x4 = 0x4,
        mogp2_unk_0x8 = 0x8,
        mogp2_unk_0x10 = 0x10,
        mogp2_unk_0x20 = 0x20,
        mogp2_is_split_group_parent = 0x40,
        mogp2_is_split_group_child = 0x80,
        mogp2_attachment_mesh = 0x100,
    }

    public struct WMOGroupFile
    {
        public uint version;
        public MOGP mogp;
    }
    public struct MOGP
    {
        public uint nameOffset;
        public uint descriptiveNameOffset;
        public MOGPFlags flags;
        public Vector3 boundingBox1;
        public Vector3 boundingBox2;
        public ushort ofsPortals; //Index of portal in MOPR chunk
        public ushort numPortals;
        public ushort numBatchesA;
        public ushort numBatchesB;
        public ushort numBatchesC; //WoWDev: For the "Number of batches" fields, A + B + C == the total number of batches in the WMO/v17 group (in the MOBA chunk).
        public ushort numBatchesD;
        public byte fogIndices_0;
        public byte fogIndices_1;
        public byte fogIndices_2;
        public byte fogIndices_3;
        public uint liquidType;
        public uint groupID; // WMOAreaTableRec::groupID
        public MOGPFlags2 flags2;
        public short parentOrFirstChildSplitGroupindex;
        public short nextSplitChildGroupIndex;
        public uint unused;
        //public MOBR[] faceIndices;
        public MOPY[] materialInfo;
        public ushort[] indices;
        public MOVT[] vertices;
        public MONR[] normals;
        public MOTV[][] textureCoords;
        public MOBA[] renderBatches;
        public MOBS[] shadowBatches;
        public MOBN[] bspNodes;
        public ushort[] bspIndices;
    }

    public struct MOBN
    {
        public ushort flags;
        public short negChild;
        public short posChild;
        public ushort nFaces;
        public uint faceStart;
        public float planeDist;
    }

    public struct MOBS
    {
        public byte unk0;
        public byte unk1;
        public byte unk2;
        public byte unk3;
        public byte unk4;
        public byte unk5;
        public byte unk6;
        public byte unk7;
        public byte unk8;
        public byte unk9;
        public short materialIDBig;
        public uint unk11;
        public short unk12;
        public byte unk13;
        public byte unk14;
        public byte unk15;
        public byte unk16;
        public byte flags;
        public byte materialIDSmall;
    }

    public struct MOVI
    {
        public ushort indice;
    }

    public struct MOVT
    {
        public Vector3 vector;
    }

    public struct MONR
    {
        public Vector3 normal;
    }

    public struct MOTV
    {
        public float X;
        public float Y;
    }

    public struct MOPY
    {
        public MOPYFlags flags;
        public ushort materialID;
    }

    [Flags]
    public enum MOPYFlags : ushort
    {
        /*
        bool isNoCamCollide (uint8 flags) { return flags & 2; }
        bool isDetailFace (uint8 flags) { return flags & 4; }
        bool isCollisionFace (uint8 flags) { return flags & 8; }
        bool isColor (uint8 flags) { return !(flags & 8); }
        bool isRenderFace (uint8 flags) { return (flags & 0x24) == 0x20; }
        bool isTransFace (uint8 flags) { return (flags & 1) && (flags & 0x24); }
        bool isCollidable (uint8 flags) { return isCollisionFace (flags) || isRenderFace (flags); }
         */
        Flag_0x1 = 0x1,
        Flag_0x2_NoCamCollide = 0x2,
        Flag_0x4_NoCollide = 0x4,
        Flag_0x8_IsCollisionFace = 0x8, //If it's not set it's isColor apparently
        mopy_unk_0x10 = 0x10,
        mopy_unk_0x20 = 0x20,
        mopy_unk_0x40 = 0x40,
        mopy_unk_0x80 = 0x80
    }

    public struct MOBA
    {
        public short possibleBox1_1;
        public short possibleBox1_2;
        public short possibleBox1_3;
        public short possibleBox2_1;
        public short possibleBox2_2;
        public short possibleBox2_3;
        public uint firstFace;
        public ushort numFaces;
        public ushort firstVertex;
        public ushort lastVertex;
        public byte flags;
        public byte materialID;
    }


}
