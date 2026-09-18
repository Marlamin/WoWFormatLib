using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WoWFormatLib.Structs
{
    public enum DATChunks
    {
        MVER = 'M' << 24 | 'V' << 16 | 'E' << 8 | 'R' << 0,
        AHDR = 'A' << 24 | 'H' << 16 | 'D' << 8 | 'R' << 0,
        ALOC = 'A' << 24 | 'L' << 16 | 'O' << 8 | 'C' << 0,
        AOCH = 'A' << 24 | 'O' << 16 | 'C' << 8 | 'H' << 0,
        AVTX = 'A' << 24 | 'V' << 16 | 'T' << 8 | 'X' << 0,
        ANRM = 'A' << 24 | 'N' << 16 | 'R' << 8 | 'M' << 0,
        ACNK = 'A' << 24 | 'C' << 16 | 'N' << 8 | 'K' << 0,
        ADOO = 'A' << 24 | 'D' << 16 | 'O' << 8 | 'O' << 0,
        ACVT = 'A' << 24 | 'C' << 16 | 'V' << 8 | 'T' << 0,
        AFBO = 'A' << 24 | 'F' << 16 | 'B' << 8 | 'O' << 0,
        ATEX = 'A' << 24 | 'T' << 16 | 'E' << 8 | 'X' << 0,
        ALYR = 'A' << 24 | 'L' << 16 | 'Y' << 8 | 'R' << 0,
        ASHD = 'A' << 24 | 'S' << 16 | 'H' << 8 | 'D' << 0,
    }

    public struct DAT
    {
        public uint version { get; set; }
        public DATHeader header { get; set; }
        public DATLocation location { get; set; }
        public List<string> doodads { get; set; }
        public List<string> textures { get; set; }
        public List<DATChunk> chunks { get; set; }
    }

    public struct DATLocation
    {
        public uint mapID { get; set; }
        public uint adtX0 { get; set; }
        public uint adtY0 { get; set; }
        public uint adtX1 { get; set; }
        public uint adtY1 { get; set; }
    }

    public struct DATHeader
    {
        public uint version { get; set; }
        public uint verticesX { get; set; }
        public uint verticesY { get; set; }
        public uint chunksX { get; set; }
        public uint chunksY { get; set; }

        public uint unk0 { get; set; }
        public uint unk1 { get; set; }
        public uint unk2 { get; set; }
        public uint unk3 { get; set; }
        public uint unk4 { get; set; }
        public uint unk5 { get; set; }
        public uint unk6 { get; set; }
        public uint unk7 { get; set; }
        public uint unk8 { get; set; }
        public uint unk9 { get; set; }
        public uint unk10 { get; set; }
    }

    public struct DATChunk
    {
        public DATMCNKHeader header { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DATMCNKHeader
    {
        public uint indexX;
        public uint indexY;
        public uint flags;
        public uint areaID;
        public ushort holesLowRes;
        public uint lowDetailTexturingMap0;
        public uint lowDetailTexturingMap1;
        public uint lowDetailTexturingMap2;
        public uint lowDetailTexturingMap3;

        public ushort MCDD;

        public ushort unk0;
        public ushort unk1;
        public ushort unk2;

        public ushort unk3;
        public ushort unk4;
        public ushort unk5;

        public ushort unk6;

        public ulong holesHighRes;

        public ushort unk7;
        public ushort unk8;
        public ushort unk9;
    }
}
