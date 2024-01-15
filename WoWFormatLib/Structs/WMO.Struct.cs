﻿using System;
using System.Numerics;

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
    }

    public enum MOHDFlags : short
    {
        Flag_0x1 = 0x1,
        Flag_0x2 = 0x2,
        Flag_0x4 = 0x4,
        Flag_0x8 = 0x8,
        Flag_0x10 = 0x10,
        Flag_0x20 = 0x20,
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
        public uint shader;
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
        Flag_0x1 = 0x1,
        Flag_0x2 = 0x2,
        Flag_0x4 = 0x4,
        Flag_0x8 = 0x8,
        Flag_0x10 = 0x10,
        Flag_0x20 = 0x20,
        Flag_0x40 = 0x40,
        Flag_0x80 = 0x80,
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

    //Group names
    public struct MOGN
    {
        public string name;
        public int offset;
    }

    //Group information
    public struct MOGI
    {
        public uint flags;
        public Vector3 boundingBox1;
        public Vector3 boundingBox2;
        public int nameIndex; //something else
    }

    [Flags]
    public enum MOGPFlags
    {
        Flag_0x1_HasMOBN_MOBR = 0x1, //Has MOBN and MOBR chunk.
        Flag_0x2 = 0x2,
        Flag_0x4_HasMOCV = 0x4, //Has vertex colors (MOCV chunk)
        Flag_0x8_Outdoor = 0x8, //Outdoor
        Flag_0x10 = 0x10,
        Flag_0x20 = 0x20,
        Flag_0x40 = 0x40,
        Flag_0x80 = 0x80,
        Flag_0x100 = 0x100,
        Flag_0x200_HasMOLR = 0x200, //Has lights  (MOLR chunk)
        Flag_0x400_HasMPBV_MPBP_MPBI_MPBG = 0x400, //Has MPBV, MPBP, MPBI, MPBG chunks.
        Flag_0x800_HasMODR = 0x800, //Has doodads (MODR chunk)
        Flag_0x1000_HasMLIQ = 0x1000, //Has water   (MLIQ chunk)
        Flag_0x2000_Indoor = 0x2000, //Indoor
        Flag_0x8000 = 0x8000,
        Flag_0x10000 = 0x10000,
        Flag_0x20000_HasMORI_MORB = 0x20000, //Has MORI and MORB chunks.
        Flag_0x40000_Skybox = 0x40000, //Show skybox
        Flag_0x80000_isNotOcean = 0x80000, //isNotOcean, LiquidType related, see below in the MLIQ chunk.
        Flag_0x100000 = 0x100000,
        Flag_0x200000 = 0x200000,
        Flag_0x400000 = 0x400000,
        Flag_0x800000 = 0x800000,
        Flag_0x1000000 = 0x1000000, //SMOGroup::CVERTS2: Has two MOCV chunks: Just add two or don't set 0x4 to only use cverts2.
        Flag_0x2000000 = 0x2000000, //SMOGroup::TVERTS2: Has two MOTV chunks: Just add two.
        Flag_0x40000000 = 0x40000000, // SMOGroup::TVERTS3: Has three MOTV chunks, eg. for MOMT with shader 18.
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
        public uint numBatchesC; //WoWDev: For the "Number of batches" fields, A + B + C == the total number of batches in the WMO/v17 group (in the MOBA chunk).
        public byte fogIndices_0;
        public byte fogIndices_1;
        public byte fogIndices_2;
        public byte fogIndices_3;
        public uint liquidType;
        public uint groupID;
        public uint unused;
        public uint unk0;
        public uint unk1;
        //public MOBR[] faceIndices;
        public MOPY[] materialInfo;
        public MOVI[] indices;
        public MOVT[] vertices;
        public MONR[] normals;
        public MOTV[][] textureCoords;
        public MOBA[] renderBatches;
        public MOBS[] shadowBatches;
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
        public byte flags;
        public byte materialID;
    }

    public enum MOPYFlags
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
