using System.Collections.Generic;
using System.Numerics;

namespace WoWFormatLib.Structs.WDL
{
    public enum WDLChunks
    {
        MVER = 'M' << 24 | 'V' << 16 | 'E' << 8 | 'R' << 0,
        MWMO = 'M' << 24 | 'W' << 16 | 'M' << 8 | 'O' << 0,
        MWID = 'M' << 24 | 'W' << 16 | 'I' << 8 | 'D' << 0,
        MODF = 'M' << 24 | 'O' << 16 | 'D' << 8 | 'F' << 0,
        MAOF = 'M' << 24 | 'A' << 16 | 'O' << 8 | 'F' << 0,
        MARE = 'M' << 24 | 'A' << 16 | 'R' << 8 | 'E' << 0,
        MAOC = 'M' << 24 | 'A' << 16 | 'O' << 8 | 'C' << 0,
        MAHO = 'M' << 24 | 'A' << 16 | 'H' << 8 | 'O' << 0,
        MLDD = 'M' << 24 | 'L' << 16 | 'D' << 8 | 'D' << 0,
        MLDX = 'M' << 24 | 'L' << 16 | 'D' << 8 | 'X' << 0,
        MLDL = 'M' << 24 | 'L' << 16 | 'D' << 8 | 'L' << 0,
        MLMD = 'M' << 24 | 'L' << 16 | 'M' << 8 | 'D' << 0,
        MLMX = 'M' << 24 | 'L' << 16 | 'M' << 8 | 'X' << 0,
        MLMB = 'M' << 24 | 'L' << 16 | 'M' << 8 | 'B' << 0,
        MSSN = 'M' << 24 | 'S' << 16 | 'S' << 8 | 'N' << 0,
        MSSC = 'M' << 24 | 'S' << 16 | 'S' << 8 | 'C' << 0,
        MSSO = 'M' << 24 | 'S' << 16 | 'S' << 8 | 'O' << 0,
        MSSF = 'M' << 24 | 'S' << 16 | 'S' << 8 | 'F' << 0,
        MAOE = 'M' << 24 | 'A' << 16 | 'O' << 8 | 'E' << 0,
        MSLD = 'M' << 24 | 'S' << 16 | 'L' << 8 | 'D' << 0,
        MSLI = 'M' << 24 | 'S' << 16 | 'L' << 8 | 'I' << 0,
    }

    public struct WDL
    {
        public uint version;
        public List<string> chunks;
        public Structs.ADT.MWMO mwmo; //WMO filenames (zero terminated)
        public Structs.ADT.MWID mwid; //Indexes to MWMO chunk
        public Structs.ADT.MODF modf; //Placement info for WMOs
        public MAOF maof; //Map Area Offset 64x64
        public MSSN[] skyScenes;
        public MSSC[] skySceneConditions;
        public MSSO[] skySceneObjects;
        public MSSF[] mssf;
        public MSLD[] skySceneLivingWorldDefs;
        public int[] skySceneLivingWorldIndices;
    }

    public struct MAOF
    {
        public uint[] areaLowOffsets; //4096 entries, make manually
    }

    public struct MSSN
    {
        public uint skySceneID;
        public uint unk1;
        public short msscIndex;
        public short msscRecordsNum;
        public ushort mssoIndex;
        public short mssoRecordsNum;
        public uint unk4;
        public uint unk5;
        public uint unk6;
        public uint unk7;
    }

    public struct MSSC
    {
        public uint unk0;
        public uint unk1;
        public uint conditionType;// 1 - conditionValue is id into AreaTable.db2 (current player's area should be parent to this one)
                                  // 2 - conditionValue is id into LightParams.db2
                                  // 3 - conditionValue is id into LightSkybox.db2 (target skybox should be assigned as current one)
                                  // 5 - conditionValue is id into ZoneLight.db2 
        public uint unk3;
        public uint unk4;
        public uint unk5;
        public uint conditionValue;
    }

    public struct MSSO
    {
        public uint unk0;
        public uint flags;
        public uint fileDataID;
        public Vector3 translateVec;
        public Vector3 rotateInRads;
        public float scale;
        public uint mssfIndex;
        public uint unk11;
    }

    public struct MSLD
    {
        public uint unk0;
        public uint unk1;
        public uint unk2;
        public uint unk3;
        public uint timeStart0;
        public uint timeStart1;
        public uint timeEnd0;
        public uint timeEnd1;
    }

    public struct MSSF
    {
        public float unk0;
        public float unk1;
    }
}
