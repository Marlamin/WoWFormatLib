using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.WDL;

namespace WoWFormatLib.FileReaders
{
    public class WDLReader
    {
        public WDL wdlfile;

        public void LoadWDL(string filename)
        {
            if (FileProvider.FileExists(filename))
            {
                using (Stream wdl = FileProvider.OpenFile(filename))
                {
                    ReadWDL(wdl);
                }
            }
            else
            {
                throw new FileNotFoundException("WDL " + filename + " does not exist");
            }
        }

        public void LoadWDL(uint filedataid)
        {
            if (FileProvider.FileExists(filedataid))
            {
                using (Stream wdl = FileProvider.OpenFile(filedataid))
                {
                    ReadWDL(wdl);
                }
            }
            else
            {
                throw new FileNotFoundException("WDL " + filedataid + " does not exist");
            }
        }

        private void ReadWDL(Stream wdl)
        {
            wdlfile = new WDL();
            var bin = new BinaryReader(wdl);
            long position = 0;
            while (position < wdl.Length)
            {
                wdl.Position = position;

                var chunkName = (WDLChunks)bin.ReadUInt32();
                var chunkSize = bin.ReadUInt32();

                position = wdl.Position + chunkSize;

                switch (chunkName)
                {
                    case WDLChunks.MVER:
                        wdlfile.version = bin.ReadUInt32();
                        if (wdlfile.version != 18)
                            throw new Exception("Unsupported WDL version!");
                        break;
                    case WDLChunks.MWMO:
                        wdlfile.mwmo = ADTReader.ReadMWMOChunk(chunkSize, bin);
                        break;
                    case WDLChunks.MWID:
                        wdlfile.mwid = ADTReader.ReadMWIDChunk(chunkSize, bin);
                        break;
                    case WDLChunks.MODF:
                        wdlfile.modf = ADTReader.ReadMODFChunk(chunkSize, bin);
                        break;
                    case WDLChunks.MSSN: // SkyScenes
                        var mssnCount = chunkSize / 32;
                        wdlfile.skyScenes = new MSSN[mssnCount];
                        for (int i = 0; i < mssnCount; i++)
                            wdlfile.skyScenes[i] = bin.Read<MSSN>();
                        break;
                    case WDLChunks.MSSC: // SkySceneConditions
                        var msscCount = chunkSize / 28;
                        wdlfile.skySceneConditions = new MSSC[msscCount];
                        for (int i = 0; i < msscCount; i++)
                            wdlfile.skySceneConditions[i] = bin.Read<MSSC>();
                        break;
                    case WDLChunks.MSSO: // SkySceneObjects
                        var mssoCount = chunkSize / 48;
                        wdlfile.skySceneObjects = new MSSO[mssoCount];
                        for (int i = 0; i < mssoCount; i++)
                            wdlfile.skySceneObjects[i] = bin.Read<MSSO>();
                        break;
                    case WDLChunks.MSSF: // SkySceneUnk
                        var mssfCount = chunkSize / 8;
                        wdlfile.mssf = new MSSF[mssfCount];
                        for (int i = 0; i < mssfCount; i++)
                            wdlfile.mssf[i] = bin.Read<MSSF>();
                        break;
                    case WDLChunks.MSLI: // SkySceneLivingWorldIndices, Only on Khaz Algar, index into MSLD
                        var msliCount = chunkSize / 4;
                        wdlfile.skySceneLivingWorldIndices = new int[msliCount];
                        for (int i = 0; i < msliCount; i++)
                            wdlfile.skySceneLivingWorldIndices[i] = bin.ReadInt32();
                        break;
                    case WDLChunks.MSLD: // SkySceneLivingWorldDefs, Only on Khaz Algar, related to Beledar
                        var msldCount = chunkSize / 32;
                        wdlfile.skySceneLivingWorldDefs = new MSLD[msldCount];
                        for (int i = 0; i < msldCount; i++)
                            wdlfile.skySceneLivingWorldDefs[i] = bin.Read<MSLD>();
                        break;
                    case WDLChunks.MAOF: // Map Area Offset
                    case WDLChunks.MARE: // Map Area Heightmap
                    case WDLChunks.MAOC: // Map Area Occlusion
                    case WDLChunks.MAOE: // Map Area Ocean
                    case WDLChunks.MAHO: // Map Area Holes
                    case WDLChunks.MLDD: // Same as OBJ1 ADT MLDD
                    case WDLChunks.MLDX: // Same as OBJ1 ADT MLDX
                    case WDLChunks.MLDL: // Same as OBJ1 ADT MLDL
                    case WDLChunks.MLMD: // Same as OBJ1 ADT MLMD
                    case WDLChunks.MLMX: // Same as OBJ1 ADT MLMX
                    case WDLChunks.MLMB: // Same as OBJ0/OBJ1 ADT MLMB
                        break;
                    default:
                        var chunkNameStr = Encoding.ASCII.GetString(BitConverter.GetBytes((uint)chunkName).Reverse().ToArray());
                        Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkNameStr, position.ToString()));
                        break;
                }
            }
        }
    }
}
