namespace WoWFormatLib.Structs.MDX
{
    public enum MDXChunks
    {
        MDLX = 'M' << 0 | 'D' << 8 | 'L' << 16 | 'X' << 24,
        VERS = 'V' << 0 | 'E' << 8 | 'R' << 16 | 'S' << 24,
        MODL = 'M' << 0 | 'O' << 8 | 'D' << 16 | 'L' << 24,
        GEOS = 'G' << 0 | 'E' << 8 | 'O' << 16 | 'S' << 24,
        VRTX = 'V' << 0 | 'R' << 8 | 'T' << 16 | 'X' << 24,
        NRMS = 'N' << 0 | 'R' << 8 | 'M' << 16 | 'S' << 24,
        PTYP = 'P' << 0 | 'T' << 8 | 'Y' << 16 | 'P' << 24,
        PCNT = 'P' << 0 | 'C' << 8 | 'N' << 16 | 'T' << 24,
        PVTX = 'P' << 0 | 'V' << 8 | 'T' << 16 | 'X' << 24,
        UVAS = 'U' << 0 | 'V' << 8 | 'A' << 16 | 'S' << 24,
        UVBS = 'U' << 0 | 'V' << 8 | 'B' << 16 | 'S' << 24,
        GNDX = 'G' << 0 | 'N' << 8 | 'D' << 16 | 'X' << 24,
        MTGC = 'M' << 0 | 'T' << 8 | 'G' << 16 | 'C' << 24,
        MATS = 'M' << 0 | 'A' << 8 | 'T' << 16 | 'S' << 24,
        TANG = 'T' << 0 | 'A' << 8 | 'N' << 16 | 'G' << 24,
        SKIN = 'S' << 0 | 'K' << 8 | 'I' << 16 | 'N' << 24
    }

    public struct MDXModel
    {
        public uint version;
        public string name;
        public Geoset[] geosets;
    }

    public struct Geoset
    {
        public Vert[] verts;
        public Normal[] normals;
        public Primitive[] primitives;
        public UV[] uvs;
        public string name;
    }

    public struct Vert
    {
        public float x;
        public float y;
        public float z;
    }

    public struct Normal
    {
        public float x;
        public float y;
        public float z;
    }

    public struct Primitive
    {
        public uint v1;
        public uint v2;
        public uint v3;
    }

    public struct UV
    {
        public float x;
        public float y;
    }
}