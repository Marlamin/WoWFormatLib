namespace WoWFormatLib.Structs.TEX
{
    public enum TEXChunks
    {
        TXVR = 'T' << 24 | 'X' << 16 | 'V' << 8 | 'R' << 0, // Version
        TXFN = 'T' << 24 | 'X' << 16 | 'F' << 8 | 'N' << 0, // File Names
        TXBT = 'T' << 24 | 'X' << 16 | 'B' << 8 | 'T' << 0, // Blob Texture
        TXMD = 'T' << 24 | 'X' << 16 | 'M' << 8 | 'D' << 0, // Mipmap Data
    }

    public readonly struct TEXFile
    {
        public readonly uint version { get; init; }
        public readonly BlobTexture[] blobTextures { get; init; }
        public readonly byte[] mipMapData { get; init; }
    }

    public readonly struct BlobTexture
    {
        public readonly uint fileDataID { get; init; }
        public readonly uint txmdOffset { get; init; }
        public readonly byte sizeX { get; init; }
        public readonly byte sizeY { get; init; }
        public readonly byte numMipMaps { get; init; }
        public readonly byte dxtFormat { get; init; }
    }
}
