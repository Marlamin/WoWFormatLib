using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WoWFormatLib.Structs.WWF
{
    public enum WWFType : uint
    {
        JSON = 0,
        Binary = 4
    }

    public enum WWFParticulateGeoType : uint
    {
        Quads,
        Triangles
    }

    public enum WWFParticulateRenderMode : uint
    {
        VelocityBillboard,
        CameraBillboard
    }

    public enum WWFParticulateConstraintType : uint
    {
        Center,
        Nearest,
        None
    }

    public enum WWFParticulateHeightFadeMode : uint
    {
        Flat,
        Relative,
        RelativeFollow
    }

    public enum WWFParticulateBlendMode : uint
    {
        Additive,
        Blended
    }

    public struct WWF
    {
        public WWFType type;
        public uint magic;
        public WWFData data;
        public string name;
    }

    public unsafe struct WWFData
    {
        public uint placeholder;
        public WWFParticulateGeoType geometryType;
        public WWFParticulateRenderMode renderMode;
        public int particleCount;
        public float strength;                         // Default Particulate Strength
        public char enableEmissionGraph;               // Enable Particulate Strength Graph
        public fixed float graphNodes[15];               // Particulate Strength Graph
        public char occlusion;                         // Enable Occlusion Testing
        public float occlusionFadeDistance;
        public float occlusionIgnoreChance;
        public char enableGroundParticulate;
        public WWFParticulateGeoType groundGeometryType;
        public WWFParticulateRenderMode groundRenderMode;
        public uint groundTextureID;            // fileDataId
        public uint groundSheetWidth;           // Ground Cell X Count
        public uint groundSheetHeight;           // Ground Cell Y Count
        public uint groundAnimationCount;
        public float groundAnimationSpeed;
        public char groundEnableAngularTransition;  // Ground Enable Texture Angle Fading
        public float groundDecayTime;
        public uint groundParticulateSize;
        public uint groundParticulateSizeVariance;
        public char depthFading;                    // Enable Depth Buffer Fading
        public float depthFadeDistance;
        public char useLookDirectionOffset;
        public float groupEdgeFadeDistance;
        public float cameraNearFade;
        public float cameraFarFade;
        public char useForceDirection;
        public Vector3 velocityMean;
        public Vector3 forceDirection;
        public float rotation;
        public float tilt;
        public float speed;
        public Vector3 velocityVariance;
        public char enableCompute;                  // Enable Dynamic Rendering
        public char enableWindEffects;
        public Vector2 massRange;
        public char useWandering;
        public float wanderSpeed;
        public Vector2 wanderAmount;
        public WWFParticulateConstraintType constraint; // Volume Constraint
        public char inclusiveRestraint;             // Use Inclusive Constraint
        public float groupSize;                     // Particulate Group Size
        public Vector3 particulateSize;
        public Vector2 particulateSizeVariance;
        public char randomRotation;                 // Randomize Initial Particle Orientation
        public char useParticleRotation;
        public float rotationSpeedMean;             // Particle Rotation Speed
        public float rotationSpeedVariance;         // Particle Rotation Speed Variance
        public char randomizeRotationDirection;
        public float useParticleFading;
        public float particleFadeTotalTime;         // Fade Loop Length
        public float particleFadeTimeBegin;         // Fade Start Time
        public float particleFadeTimeEnd;           // Fade End Time
        public float particleFadeTimeVariance;
        public char enableHeightFading;
        public WWFParticulateHeightFadeMode heightFadeMode;
        public float heightFadeStartDistance;       // Height Fade Max Height
        public float heightFadeVariance;
        public float heightFadeTransitionDistance;
        public char enableDeltaCulling;
        public float deltaCullingMax;
        public WWFParticulateBlendMode blendMode;
        public char useSkyFogColor;                 // Use Sky Fog Color As Diffuse
        public char useAmbientLighting;
        public uint color;
        public uint textureID;                  // fileDataId
        public char useAnimation;
        public int sheetWidth;                  // Cell X Count
        public int sheetHeight;                 // Cell Y Count
        public int animationCount;
        public float animationSpeed;
        public char enableAngularTransition;        // Enable Texture Angle Fading
    }
}
