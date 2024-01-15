﻿using System.Numerics;

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

    public struct TextureUnit
    {
        public ushort flags;
        public ushort shading;
        public ushort submeshIndex;
        public ushort submeshIndex2;
        public ushort colorIndex;
        public ushort renderFlags;
        public ushort texUnitNumber;
        public ushort mode;
        public ushort texture;
        public ushort texUnitNumber2;
        public ushort transparency;
        public ushort textureAnim;
    }
}
