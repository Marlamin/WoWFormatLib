using System;
using System.Collections.Generic;
using System.Numerics;

namespace WoWFormatLib.Structs.WDT
{
    public enum WDTChunks
    {
        // Root
        MVER = 'M' << 24 | 'V' << 16 | 'E' << 8 | 'R' << 0,
        MAIN = 'M' << 24 | 'A' << 16 | 'I' << 8 | 'N' << 0,
        MWMO = 'M' << 24 | 'W' << 16 | 'M' << 8 | 'O' << 0,
        MPHD = 'M' << 24 | 'P' << 16 | 'H' << 8 | 'D' << 0,
        MODF = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'F' << 0,
        MAID = 'M' << 24 | 'A' << 16 | 'I' << 8 | 'D' << 0,
        MAI2 = 'M' << 24 | 'A' << 16 | 'I' << 8 | '2' << 0,
        MANM = 'M' << 24 | 'A' << 16 | 'N' << 8 | 'M' << 0,

        // Occlusion
        MAOI = 'M' << 24 | 'A' << 16 | 'O' << 8 | 'I' << 0,
        MAOH = 'M' << 24 | 'A' << 16 | 'O' << 8 | 'H' << 0,

        // Lights
        MPLT = 'M' << 24 | 'P' << 16 | 'L' << 8 | 'T' << 0,
        MPL2 = 'M' << 24 | 'P' << 16 | 'L' << 8 | '2' << 0,
        MPL3 = 'M' << 24 | 'P' << 16 | 'L' << 8 | '3' << 0,
        MSLT = 'M' << 24 | 'S' << 16 | 'L' << 8 | 'T' << 0,
        MTEX = 'M' << 24 | 'T' << 16 | 'E' << 8 | 'X' << 0,
        MLTA = 'M' << 24 | 'L' << 16 | 'T' << 8 | 'A' << 0,

        // Fogs
        VFOG = 'V' << 24 | 'F' << 16 | 'O' << 8 | 'G' << 0,
        VFEX = 'V' << 24 | 'F' << 16 | 'E' << 8 | 'X' << 0,

        // MapParticulateVolumes
        PVPD = 'P' << 24 | 'V' << 16 | 'P' << 8 | 'D' << 0,
        PVMI = 'P' << 24 | 'V' << 16 | 'M' << 8 | 'I' << 0,
        PVBD = 'P' << 24 | 'V' << 16 | 'B' << 8 | 'D' << 0,
    }

    public struct WDT
    {
        public uint version;

        // Root
        public MPHD mphd;
        public ADT.MODF modf;
        public MANM manm;
        public MapFileDataIDs[] filedataids;
        public Dictionary<uint, string> filenames;
        public List<(byte, byte)> tiles;
        public Dictionary<(byte, byte), MapFileDataIDs> tileFiles;
        public Dictionary<(byte, byte), MapFileDataIDs2> tileFiles2;
        public Dictionary<string, MapFileDataIDs> stringTileFiles;

        // TODO: Other types
    }

    /* Names in MANM stuff are very much placeholder and taken from 010 template*/
    public struct MANM
    {
        public uint version;
        public uint countB;
        public MANM_B[] entriesB;

    }

    public struct MANM_B
    {
        public uint c; // ID?
        public byte[] d; // 544 bytes of still unk
        public uint type;
        public uint s;
        public uint posPlusNormalCount;
        public MANMPosPlusNormal[] posPlusNormal;
    }

    public struct MANMPosPlusNormal
    {
        public Vector3 position;
        public Vector3 normal;
    }

    public struct MapFileDataIDs
    {
        public uint rootADT;
        public uint obj0ADT;
        public uint obj1ADT;
        public uint tex0ADT;
        public uint lodADT;
        public uint mapTexture;
        public uint mapTextureN;
        public uint minimapTexture;
    }

    public struct MapFileDataIDs2
    {
        public uint unknown0;
        public uint unknown1;
        public uint unknown2;
        public uint unknown3;
        public uint unknown4;
        public uint unknown5;
        public uint unknown6;
        public uint unknown7;
    }

    public struct MPHD
    {
        public MPHDFlags flags;
        public uint lgtFDID;
        public uint occFDID;
        public uint fogsFDID;
        public uint mpvFDID;
        public uint texFDID;
        public uint wdlFDID;
        public uint pd4FDID;
    }

    [Flags]
    public enum MPHDFlags : uint
    {
        wdt_uses_global_map_obj = 0x1,
        adt_has_mccv = 0x2,
        adt_has_big_alpha = 0x4,
        adt_has_doodadrefs_sorted_by_size_cat = 0x8,
        adt_has_mclv = 0x10,
        adt_has_upside_down_ground = 0x20,
        unk_0x40 = 0x40,
        adt_has_height_texturing = 0x80,
        unk_0x100 = 0x100,
        wdt_has_maid = 0x200,
        unk_0x400 = 0x400,
        unk_0x800 = 0x800,
        unk_0x1000 = 0x1000,
        unk_0x2000 = 0x2000,
        unk_0x4000 = 0x4000,
        unk_0x8000 = 0x8000
    }
}
