using System.Numerics;

namespace WoWFormatLib.Structs.SKIN
{
    public struct SKIN
    {
        public uint version;
        public string filename;
        public Indice[] indices;
        public Triangle[] triangles;
        public Property[] properties;
        public Submesh[] submeshes;
        public TextureUnit[] textureunit;
        public uint bones;
    }

    public struct Indice
    {
        public ushort vertex;
    }

    public struct Triangle
    {
        public ushort pt1;
        public ushort pt2;
        public ushort pt3;
    }

    public struct Property
    {
        public byte properties_0;
        public byte properties_1;
        public byte properties_2;
        public byte properties_3;
    }

    public struct Submesh
    {
        public ushort submeshID;
        public ushort level;
        public uint startVertex;
        public ushort nVertices;
        public uint startTriangle;
        public ushort nTriangles;
        public ushort nBones;
        public ushort startBones;
        public ushort boneInfluences;
        public ushort centerBoneIndex;
        public Vector3 centerPosition;
        public Vector3 sortCenterPosition;
        public float sortRadius;
    }

    public enum TextureUnitFlags : byte
    {
        Flag_0x1 = 0x1, // "materials invert something" -deamon
        Transform = 0x2,
        ProjectedTexture = 0x4,
        Flag_0x8 = 0x8, // Something batch compatible
        Flag_0x10 = 0x10,
        Flag_0x20 = 0x20, // Project texture 2?
        Flag_0x40 = 0x40 // Use TextureWeights
    }
    public struct TextureUnit
    {
        public TextureUnitFlags flags;
        public sbyte priorityPlane;
        public ushort shaderID;
        public ushort submeshIndex;
        public ushort flags2;
        public ushort colorIndex;
        public ushort renderFlagsIndex;
        public ushort materialLayer;
        public ushort textureCount;
        public ushort texture;
        public ushort texUnitNumber2;
        public ushort transparency;
        public ushort textureAnim;
    }
}
