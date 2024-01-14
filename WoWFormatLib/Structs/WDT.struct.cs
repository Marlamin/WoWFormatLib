﻿using System;
using System.Collections.Generic;
using System.Numerics;
using WoWFormatLib.Utils;

namespace WoWFormatLib.Structs.WDT
{
    public enum WDTChunks
    {
        MVER = 'M' << 24 | 'V' << 16 | 'E' << 8 | 'R' << 0,
        MAIN = 'M' << 24 | 'A' << 16 | 'I' << 8 | 'N' << 0,
        MWMO = 'M' << 24 | 'W' << 16 | 'M' << 8 | 'O' << 0,
        MPHD = 'M' << 24 | 'P' << 16 | 'H' << 8 | 'D' << 0,
        MPLT = 'M' << 24 | 'P' << 16 | 'L' << 8 | 'T' << 0,
        MODF = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'F' << 0,
        MAID = 'M' << 24 | 'A' << 16 | 'I' << 8 | 'D' << 0,
        MANM = 'M' << 24 | 'A' << 16 | 'N' << 8 | 'M' << 0
    }

    public struct WDT
    {
        public MPHD mphd;
        public MODF modf;
        public MANM manm;
        public MapFileDataIDs[] filedataids;
        public Dictionary<uint, string> filenames;
        public List<(byte, byte)> tiles;
        public Dictionary<(byte, byte), MapFileDataIDs> tileFiles;
        public Dictionary<string, MapFileDataIDs> stringTileFiles;
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

    public struct MODF
    {
        public uint id;
        public uint uuid;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 upperExtents;
        public Vector3 lowerExtents;
        public short flags;
        public short doodadSetID;
        public short nameSet;
        public short padding;
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

    public struct MPHD
    {
        public MPHDFlags flags;
        public uint lgtFDID;
        public uint occFDID;
        public uint fogsFDID;
        public uint mpvFDID;
        public uint texFDID;
        public uint wdlFDID;
        public uint unused;
    }

    [Flags]
    public enum MPHDFlags : uint
    {
        wdt_uses_global_map_obj                 = 0x1,
        adt_has_mccv                            = 0x2,
        adt_has_big_alpha                       = 0x4,
        adt_has_doodadrefs_sorted_by_size_cat   = 0x8,
        adt_has_mclv                            = 0x10,
        adt_has_upside_down_ground              = 0x20,
        unk_0x40                                = 0x40,
        adt_has_height_texturing                = 0x80,
        unk_0x100                               = 0x100,
        unk_0x200                               = 0x200,
        unk_0x400                               = 0x400,
        unk_0x800                               = 0x800,
        unk_0x1000                              = 0x1000,
        unk_0x2000                              = 0x2000,
        unk_0x4000                              = 0x4000,
        unk_0x8000                              = 0x8000
    }
}
