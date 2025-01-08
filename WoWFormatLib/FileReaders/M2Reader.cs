﻿using System;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.M2;

namespace WoWFormatLib.FileReaders
{
    public class M2Reader
    {
        public M2Model model;

        public void LoadM2(string filename, bool loadSkins = true)
        {
            if (FileProvider.FileExists(filename))
            {
                LoadM2(FileProvider.GetFileDataIdByName(Path.ChangeExtension(filename, "M2")), loadSkins);
            }
            else
            {
                throw new FileNotFoundException("M2 " + filename + " not found");
            }
        }

        public void LoadM2(uint fileDataID, bool loadSkins = true)
        {
#if DEBUG
            using (var stream = FileProvider.OpenFile(fileDataID))
            {
                LoadM2(stream, loadSkins);
            }
#else
                try
                {
                    LoadM2(FileProvider.OpenFile(fileDataID));
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error during reading file: {0}", e.Message);
                    return;
                }
#endif
        }

        public void LoadM2(Stream m2, bool loadSkins = true)
        {
            var bin = new BinaryReader(m2);
            long position = 0;
            while (position < m2.Length)
            {
                m2.Position = position;

                var chunkName = (M2Chunks)bin.ReadUInt32();
                if (chunkName == 0)
                    throw new Exception("M2 is likely encrypted");

                var chunkSize = bin.ReadUInt32();

                position = m2.Position + chunkSize;

                switch (chunkName)
                {
                    case M2Chunks.MD21:
                        using (Stream m2stream = new MemoryStream(bin.ReadBytes((int)chunkSize)))
                        {
                            ParseYeOldeM2Struct(m2stream);
                        }
                        break;
                    case M2Chunks.AFID: // Animation file IDs
                        var afids = new AFID[chunkSize / 8];
                        for (var a = 0; a < chunkSize / 8; a++)
                        {
                            afids[a].animID = bin.ReadInt16();
                            afids[a].subAnimID = bin.ReadInt16();
                            afids[a].fileDataID = bin.ReadUInt32();
                        }
                        model.animFileDataIDs = afids;
                        break;
                    case M2Chunks.BFID: // Bone file IDs
                        var bfids = new uint[chunkSize / 4];
                        for (var b = 0; b < chunkSize / 4; b++)
                        {
                            bfids[b] = bin.ReadUInt32();
                        }
                        break;
                    case M2Chunks.SFID: // Skin file IDs
                        var sfids = new uint[chunkSize / 4];
                        for (var s = 0; s < chunkSize / 4; s++)
                        {
                            sfids[s] = bin.ReadUInt32();
                        }
                        model.skinFileDataIDs = sfids;
                        break;
                    case M2Chunks.PFID: // Phys file ID
                        model.physFileID = bin.ReadUInt32();
                        break;
                    case M2Chunks.SKID: // Skel file ID
                        model.skelFileID = bin.ReadUInt32();
                        break;
                    case M2Chunks.TXID: // Texture file IDs
                        var txids = new uint[chunkSize / 4];
                        for (var t = 0; t < chunkSize / 4; t++)
                        {
                            txids[t] = bin.ReadUInt32();
                        }
                        model.textureFileDataIDs = txids;
                        break;
                    case M2Chunks.RPID: // Recursive particle file IDs
                        var rpids = new uint[chunkSize / 4];
                        for (var t = 0; t < chunkSize / 4; t++)
                        {
                            rpids[t] = bin.ReadUInt32();
                        }
                        model.recursiveParticleModelFileIDs = rpids;
                        break;
                    case M2Chunks.GPID: // Geometry particle file IDs
                        var gpids = new uint[chunkSize / 4];
                        for (var t = 0; t < chunkSize / 4; t++)
                        {
                            gpids[t] = bin.ReadUInt32();
                        }
                        model.geometryParticleModelFileIDs = gpids;
                        break;
                    case M2Chunks.TXAC: // Texture transforms (?)
                    case M2Chunks.EXPT: // Extended Particles
                    case M2Chunks.EXP2: // Extended Particles 2
                    case M2Chunks.PABC:
                    case M2Chunks.PADC:
                    case M2Chunks.PEDC:
                    case M2Chunks.PSBC:
                    case M2Chunks.PGD1:
                    case M2Chunks.WFV1:
                    case M2Chunks.WFV2:
                    case M2Chunks.LDV1:
                    case M2Chunks.WFV3:
                    case M2Chunks.PFDC:
                    case M2Chunks.EDGF:
                    case M2Chunks.NERF:
                    case M2Chunks.DETL:
                    case M2Chunks.DBOC:
                        break;
                    default:
                        if (chunkName.ToString("X") != "00000000")
                        {
#if DEBUG
                            Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)chunkName))));
                            break;
#else
                            Console.WriteLine(String.Format("M2: Found unknown header at offset {1} \"{0}\"", chunkName, position.ToString()));
#endif
                        }
                        break;
                }
            }

            if (loadSkins && model.skinFileDataIDs != null)
            {
                model.skins = ReadSkins(model.skinFileDataIDs);
            }

            return;
        }

        public void ParseYeOldeM2Struct(Stream m2stream)
        {
            var bin = new BinaryReader(m2stream);
            var header = (M2Chunks)bin.ReadUInt32();
            if (header != M2Chunks.MD20)
            {
                throw new Exception("Invalid M2 file!");
            }

            model.version = bin.ReadUInt32();
            var lenModelname = bin.ReadUInt32();
            var ofsModelname = bin.ReadUInt32();
            model.flags = (GlobalModelFlags)bin.ReadUInt32();
            var nSequences = bin.ReadUInt32();
            var ofsSequences = bin.ReadUInt32();
            var nAnimations = bin.ReadUInt32();
            var ofsAnimations = bin.ReadUInt32();
            var nAnimationLookup = bin.ReadUInt32();
            var ofsAnimationLookup = bin.ReadUInt32();
            var nBones = bin.ReadUInt32();
            var ofsBones = bin.ReadUInt32();
            var nKeyboneLookup = bin.ReadUInt32();
            var ofsKeyboneLookup = bin.ReadUInt32();
            var nVertices = bin.ReadUInt32();
            var ofsVertices = bin.ReadUInt32();
            model.nViews = bin.ReadUInt32();
            var nColors = bin.ReadUInt32();
            var ofsColors = bin.ReadUInt32();
            var nTextures = bin.ReadUInt32();
            var ofsTextures = bin.ReadUInt32();
            var nTransparency = bin.ReadUInt32();
            var ofsTransparency = bin.ReadUInt32();
            var nUVAnimation = bin.ReadUInt32();
            var ofsUVAnimation = bin.ReadUInt32();
            var nTexReplace = bin.ReadUInt32();
            var ofsTexReplace = bin.ReadUInt32();
            var nRenderFlags = bin.ReadUInt32();
            var ofsRenderFlags = bin.ReadUInt32();
            var nBoneLookupTable = bin.ReadUInt32();
            var ofsBoneLookupTable = bin.ReadUInt32();
            var nTexLookup = bin.ReadUInt32();
            var ofsTexLookup = bin.ReadUInt32();
            var nUnk1 = bin.ReadUInt32();
            var ofsUnk1 = bin.ReadUInt32();
            var nTransLookup = bin.ReadUInt32();
            var ofsTranslookup = bin.ReadUInt32();
            var nUVAnimLookup = bin.ReadUInt32();
            var ofsUVAnimLookup = bin.ReadUInt32();
            model.vertexbox = new Vector3[2];
            model.vertexbox[0] = bin.Read<Vector3>();
            model.vertexbox[1] = bin.Read<Vector3>();
            model.vertexradius = bin.ReadSingle();
            model.boundingbox = new Vector3[2];
            model.boundingbox[0] = bin.Read<Vector3>();
            model.boundingbox[1] = bin.Read<Vector3>();
            model.boundingradius = bin.ReadSingle();
            var nBoundingTriangles = bin.ReadUInt32();
            var ofsBoundingTriangles = bin.ReadUInt32();
            var nBoundingVertices = bin.ReadUInt32();
            var ofsBoundingVertices = bin.ReadUInt32();
            var nBoundingNormals = bin.ReadUInt32();
            var ofsBoundingNormals = bin.ReadUInt32();
            var nAttachments = bin.ReadUInt32();
            var ofsAttachments = bin.ReadUInt32();
            var nAttachLookup = bin.ReadUInt32();
            var ofsAttachLookup = bin.ReadUInt32();
            var nEvents = bin.ReadUInt32();
            var ofsEvents = bin.ReadUInt32();
            var nLights = bin.ReadUInt32();
            var ofsLights = bin.ReadUInt32();
            var nCameras = bin.ReadUInt32();
            var ofsCameras = bin.ReadUInt32();
            var nCameraLookup = bin.ReadUInt32();
            var ofsCameraLookup = bin.ReadUInt32();
            var nRibbonEmitters = bin.ReadUInt32();
            var ofsRibbonEmitters = bin.ReadUInt32();
            var nParticleEmitters = bin.ReadUInt32();
            var ofsParticleEmitters = bin.ReadUInt32();

            if (GlobalModelFlags.Flag_UseTextureCombinerCombos != 0) //models with flag 8 have extra field
            {
                var nUnk2 = bin.ReadUInt32();
                var ofsUnk2 = bin.ReadUInt32();
            }

            if (lenModelname != 0)
            {
                bin.BaseStream.Position = ofsModelname;
                model.name = new string(bin.ReadChars((int)lenModelname));
                model.name = model.name.Remove(model.name.Length - 1); //remove last char, empty
            }
            else
            {
                model.name = "";
            }

            model.sequences = ReadSequences(nSequences, ofsSequences, bin);
            model.animations = ReadAnimations(nAnimations, ofsAnimations, bin);
            model.animationlookup = ReadAnimationLookup(nAnimationLookup, ofsAnimationLookup, bin);
            model.bones = ReadBones(nBones, ofsBones, bin);
            model.keybonelookup = ReadKeyboneLookup(nKeyboneLookup, ofsKeyboneLookup, bin);
            model.vertices = ReadVertices(nVertices, ofsVertices, bin);
            model.colors = ReadColors(nColors, ofsColors, bin);
            model.textures = ReadTextures(nTextures, ofsTextures, bin);
            model.transparency = ReadTransparency(nTransparency, ofsTransparency, bin);
            model.uvanimations = ReadUVAnimation(nUVAnimation, ofsUVAnimation, bin);
            model.texreplace = ReadTexReplace(nTexReplace, ofsTexReplace, bin);
            model.renderflags = ReadRenderFlags(nRenderFlags, ofsRenderFlags, bin);
            model.bonelookuptable = ReadBoneLookupTable(nBoneLookupTable, ofsBoneLookupTable, bin);
            model.texlookup = ReadTexLookup(nTexLookup, ofsTexLookup, bin);
            ReadUnk1(nUnk1, ofsUnk1, bin); // tex_unit_lookup_table unused as of cata
            model.translookup = ReadTransLookup(nTransLookup, ofsTranslookup, bin);
            model.uvanimlookup = ReadUVAnimLookup(nUVAnimLookup, ofsUVAnimLookup, bin);
            model.boundingtriangles = ReadBoundingTriangles(nBoundingTriangles, ofsBoundingTriangles, bin);
            model.boundingvertices = ReadBoundingVertices(nBoundingVertices, ofsBoundingVertices, bin);
            model.boundingnormals = ReadBoundingNormals(nBoundingNormals, ofsBoundingNormals, bin);
            model.attachments = ReadAttachments(nAttachments, ofsAttachments, bin);
            model.attachlookup = ReadAttachLookup(nAttachLookup, ofsAttachLookup, bin);
            model.events = ReadEvents(nEvents, ofsEvents, bin);
            model.lights = ReadLights(nLights, ofsLights, bin);
            model.cameras = ReadCameras(nCameras, ofsCameras, bin);
            model.cameralookup = ReadCameraLookup(nCameraLookup, ofsCameraLookup, bin);
            model.ribbonemitters = ReadRibbonEmitters(nRibbonEmitters, ofsRibbonEmitters, bin);
            model.particleemitters = ReadParticleEmitters(nParticleEmitters, ofsParticleEmitters, bin);
        }

        private static AnimationLookup[] ReadAnimationLookup(uint nAnimationLookup, uint ofsAnimationLookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsAnimationLookup;
            var animationlookup = new AnimationLookup[nAnimationLookup];
            for (var i = 0; i < nAnimationLookup; i++)
            {
                animationlookup[i] = bin.Read<AnimationLookup>();
            }
            return animationlookup;
        }
        private Animation[] ReadAnimations(uint nAnimations, uint ofsAnimations, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsAnimations;
            var animations = new Animation[nAnimations];
            for (var i = 0; i < nAnimations; i++)
            {
                animations[i] = bin.Read<Animation>();
                if (((uint)animations[i].flags & 0x130) == 0 && model.animFileDataIDs != null)
                {
                    // Animation in other file
                    foreach (var afid in model.animFileDataIDs)
                    {
                        if (animations[i].animationID == afid.animID && animations[i].subAnimationID == animations[i].subAnimationID)
                        {
                            Console.WriteLine(".anim filedata id " + afid.fileDataID);
                        }
                    }
                }
                else
                {
                    // Animation included in this file
                }
            }
            return animations;
        }
        private static AttachLookup[] ReadAttachLookup(uint nAttachLookup, uint ofsAttachLookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsAttachLookup;
            var attachlookup = new AttachLookup[nAttachLookup];
            for (var i = 0; i < nAttachLookup; i++)
            {
                attachlookup[i] = bin.Read<AttachLookup>();
            }
            return attachlookup;
        }
        private static Attachment[] ReadAttachments(uint nAttachments, uint ofsAttachments, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsAttachments;
            var attachments = new Attachment[nAttachments];
            for (var i = 0; i < nAttachments; i++)
            {
                attachments[i] = bin.Read<Attachment>();
            }
            return attachments;
        }
        private static BoneLookupTable[] ReadBoneLookupTable(uint nBoneLookupTable, uint ofsBoneLookupTable, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsBoneLookupTable;
            var bonelookuptable = new BoneLookupTable[nBoneLookupTable];
            for (var i = 0; i < nBoneLookupTable; i++)
            {
                bonelookuptable[i] = bin.Read<BoneLookupTable>();
            }
            return bonelookuptable;
        }
        private static Bone[] ReadBones(uint nBones, uint ofsBones, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsBones;
            var bones = new Bone[nBones];
            for (var i = 0; i < nBones; i++)
            {
                bones[i] = bin.Read<Bone>();
            }
            return bones;
        }
        private static BoundingNormal[] ReadBoundingNormals(uint nBoundingNormals, uint ofsBoundingNormals, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsBoundingNormals;
            var boundingNormals = new BoundingNormal[nBoundingNormals];
            for (var i = 0; i < nBoundingNormals; i++)
            {
                boundingNormals[i] = bin.Read<BoundingNormal>();
            }
            return boundingNormals;
        }
        private static BoundingVertex[] ReadBoundingVertices(uint nBoundingVertices, uint ofsBoundingVertices, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsBoundingVertices;
            var boundingVertices = new BoundingVertex[nBoundingVertices];
            for (var i = 0; i < nBoundingVertices; i++)
            {
                boundingVertices[i] = bin.Read<BoundingVertex>();
            }
            return boundingVertices;
        }
        private static BoundingTriangle[] ReadBoundingTriangles(uint nBoundingTriangles, uint ofsBoundingTriangles, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsBoundingTriangles;
            var boundingTriangles = new BoundingTriangle[nBoundingTriangles / 3];
            for (var i = 0; i < nBoundingTriangles / 3; i++)
            {
                boundingTriangles[i] = bin.Read<BoundingTriangle>();
            }
            return boundingTriangles;
        }
        private static CameraLookup[] ReadCameraLookup(uint nCameraLookup, uint ofsCameraLookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsCameraLookup;
            var cameraLookup = new CameraLookup[nCameraLookup];
            for (var i = 0; i < nCameraLookup; i++)
            {
                cameraLookup[i] = bin.Read<CameraLookup>();
            }
            return cameraLookup;
        }
        private static Camera[] ReadCameras(uint nCameras, uint ofsCameras, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsCameras;
            var cameras = new Camera[nCameras];
            for (var i = 0; i < nCameras; i++)
            {
                cameras[i] = bin.Read<Camera>();
            }
            return cameras;
        }
        private static Color[] ReadColors(uint nColors, uint ofsColors, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsColors;
            var colors = new Color[nColors];
            for (var i = 0; i < nColors; i++)
            {
                colors[i] = bin.Read<Color>();
            }
            return colors;
        }
        private static Event[] ReadEvents(uint nEvents, uint ofsEvents, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsEvents;
            var events = new Event[nEvents];
            for (var i = 0; i < nEvents; i++)
            {
                events[i] = new Event()
                {
                    identifier = System.Text.Encoding.ASCII.GetString(bin.ReadBytes(4)),
                    data = bin.ReadUInt32(),
                    bone = bin.ReadUInt32(),
                    position = bin.Read<Vector3>(),
                    interpolationType = bin.ReadUInt16(),
                    GlobalSequence = bin.ReadUInt16(),
                    nTimestampEntries = bin.ReadUInt32(),
                    ofsTimestampList = bin.ReadUInt32()
                };
            }
            return events;
        }
        private static KeyBoneLookup[] ReadKeyboneLookup(uint nKeyboneLookup, uint ofsKeyboneLookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsKeyboneLookup;
            var keybonelookup = new KeyBoneLookup[nKeyboneLookup];
            for (var i = 0; i < nKeyboneLookup; i++)
            {
                keybonelookup[i] = bin.Read<KeyBoneLookup>();
            }
            return keybonelookup;
        }
        private static Light[] ReadLights(uint nLights, uint ofsLights, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsLights;
            var lights = new Light[nLights];
            for (var i = 0; i < nLights; i++)
            {
                lights[i] = bin.Read<Light>();
            }
            return lights;
        }
        private static ParticleEmitter[] ReadParticleEmitters(uint nParticleEmitters, uint ofsParticleEmitters, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsParticleEmitters;
            var particleEmitters = new ParticleEmitter[nParticleEmitters];
            for (var i = 0; i < nParticleEmitters; i++)
            {
                //Apparently really wrong. Who needs particles, right?
            }
            return particleEmitters;
        }
        private static RenderFlag[] ReadRenderFlags(uint nRenderFlags, uint ofsRenderFlags, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsRenderFlags;
            var renderflags = new RenderFlag[nRenderFlags];
            for (var i = 0; i < nRenderFlags; i++)
            {
                renderflags[i] = bin.Read<RenderFlag>();
            }
            return renderflags;
        }
        private static RibbonEmitter[] ReadRibbonEmitters(uint nRibbonEmitters, uint ofsRibbonEmitters, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsRibbonEmitters;
            var ribbonEmitters = new RibbonEmitter[nRibbonEmitters];
            for (var i = 0; i < nRibbonEmitters; i++)
            {
                ribbonEmitters[i] = bin.Read<RibbonEmitter>();
            }
            return ribbonEmitters;
        }
        private static Sequence[] ReadSequences(uint nSequences, uint ofsSequences, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsSequences;
            var sequences = new Sequence[nSequences];
            for (var i = 0; i < nSequences; i++)
            {
                sequences[i] = bin.Read<Sequence>();
            }
            return sequences;
        }
        private static Structs.SKIN.SKIN[] ReadSkins(uint[] skinFileDataIDs)
        {
            var skins = new Structs.SKIN.SKIN[skinFileDataIDs.Length];
            for (var i = 0; i < skinFileDataIDs.Length; i++)
            {
                var skinreader = new SKINReader();
                skinreader.LoadSKIN(skinFileDataIDs[i]);
                skins[i] = skinreader.skin;
            }
            return skins;
        }
        private static TexLookup[] ReadTexLookup(uint nTexLookup, uint ofsTexLookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsTexLookup;
            var texlookup = new TexLookup[nTexLookup];
            for (var i = 0; i < nTexLookup; i++)
            {
                texlookup[i] = bin.Read<TexLookup>();
            }
            return texlookup;
        }
        private static TexReplace[] ReadTexReplace(uint nTexReplace, uint ofsTexReplace, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsTexReplace;
            var texreplace = new TexReplace[nTexReplace];
            for (var i = 0; i < nTexReplace; i++)
            {
                texreplace[i] = bin.Read<TexReplace>();
            }
            return texreplace;
        }
        private static Texture[] ReadTextures(uint num, uint offset, BinaryReader bin)
        {
            bin.BaseStream.Position = offset;
            var textures = new Texture[num];
            for (var i = 0; i < num; i++)
            {
                textures[i].type = bin.ReadUInt32();
                textures[i].flags = (TextureFlags)bin.ReadUInt32();
                var lenFilename = bin.ReadUInt32();
                var ofsFilename = bin.ReadUInt32();

                if (textures[i].type == 0)
                {
                    if (ofsFilename < 10)
                    {
                        // Referenced in TXID, no longer in file (rip listfiles)
                        textures[i].filename = @"dungeons\textures\testing\color_01.blp";
                    }
                    else
                    {
                        var preFilenamePosition = bin.BaseStream.Position; // probably a better way to do all this
                        bin.BaseStream.Position = ofsFilename;
                        var filename = new string(bin.ReadChars(int.Parse(lenFilename.ToString())));
                        filename = filename.Replace("\0", "");
                        if (!filename.Equals(""))
                        {
                            textures[i].filename = filename;
                        }
                        else
                        {
                            textures[i].filename = @"dungeons\textures\testing\color_01.blp";
                        }
                        bin.BaseStream.Position = preFilenamePosition;
                    }
                }
                else
                {
                    textures[i].filename = @"dungeons\textures\testing\color_01.blp";
                }
            }
            return textures;
        }
        private static TransLookup[] ReadTransLookup(uint nTransLookup, uint ofsTranslookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsTranslookup;
            var translookup = new TransLookup[nTransLookup];
            for (var i = 0; i < nTransLookup; i++)
            {
                translookup[i] = bin.Read<TransLookup>();
            }
            return translookup;
        }
        private static Transparency[] ReadTransparency(uint nTransparency, uint ofsTransparency, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsTransparency;
            var transparency = new Transparency[nTransparency];
            for (var i = 0; i < nTransparency; i++)
            {
                transparency[i] = bin.Read<Transparency>();
            }
            return transparency;
        }
        private static void ReadUnk1(uint nUnk1, uint ofsUnk1, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsUnk1;
            for (var i = 0; i < nUnk1; i++)
            {
                //wot
            }
        }
        private static UVAnimation[] ReadUVAnimation(uint nUVAnimation, uint ofsUVAnimation, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsUVAnimation;
            var uvanimations = new UVAnimation[nUVAnimation];
            for (var i = 0; i < nUVAnimation; i++)
            {
                uvanimations[i] = bin.Read<UVAnimation>();
            }
            return uvanimations;
        }
        private static UVAnimLookup[] ReadUVAnimLookup(uint nUVAnimLookup, uint ofsUVAnimLookup, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsUVAnimLookup;
            var uvanimlookup = new UVAnimLookup[nUVAnimLookup];
            for (var i = 0; i < nUVAnimLookup; i++)
            {
                uvanimlookup[i] = bin.Read<UVAnimLookup>();
            }
            return uvanimlookup;
        }
        private static Vertice[] ReadVertices(uint nVertices, uint ofsVertices, BinaryReader bin)
        {
            bin.BaseStream.Position = ofsVertices;
            var vertices = new Vertice[nVertices];
            for (var i = 0; i < nVertices; i++)
            {
                vertices[i] = bin.Read<Vertice>();
            }
            return vertices;
        }
    }
}
