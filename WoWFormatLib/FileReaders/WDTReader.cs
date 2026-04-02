using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.WDT;

namespace WoWFormatLib.FileReaders
{
    public class WDTReader
    {
        public enum WDTType
        {
            Unknown,
            Root,
            Occlusion,
            Lights,
            Fogs,
            MapParticuleVolumes,
            Preload
        }

        public WDT wdtfile;
        public WDTType type;

        public void LoadWDT(string filename)
        {
            if (FileProvider.FileExists(filename))
            {
                LoadWDT(FileProvider.GetFileDataIdByName(filename));
            }
            else
            {
                throw new FileNotFoundException("WDT " + filename + " does not exist");
            }
        }

        public void LoadWDT(uint filedataid)
        {
            using (var stream = FileProvider.OpenFile(filedataid))
            {
                ReadWDT(stream);
            }

            wdtfile.stringTileFiles = new Dictionary<string, MapFileDataIDs>();

            if (wdtfile.tileFiles == null)
                wdtfile.tileFiles = new Dictionary<(byte, byte), MapFileDataIDs>();

            foreach (var entry in wdtfile.tileFiles)
            {
                wdtfile.stringTileFiles.Add(entry.Key.Item1 + "," + entry.Key.Item2, entry.Value);
            }
        }

        public void LoadWDT(Stream wdt)
        {
            wdtfile = new WDT();
            ReadWDT(wdt);
        }

        private static List<(byte, byte)> ReadMAINChunk(BinaryReader bin)
        {
            var tileList = new List<(byte, byte)>();
            for (byte x = 0; x < 64; x++)
            {
                for (byte y = 0; y < 64; y++)
                {
                    var flags = bin.ReadUInt32();
                    bin.ReadUInt32();
                    if (flags == 1)
                    {
                        tileList.Add((y, x));
                    }
                }
            }
            return tileList;
        }

        private static void ReadMWMOChunk(BinaryReader bin, uint size)
        {
            if (size == 0)
                return;

            if (bin.ReadByte() != 0)
            {
                bin.BaseStream.Position = bin.BaseStream.Position - 1;

                var str = new StringBuilder();
                char c;
                while ((c = bin.ReadChar()) != '\0')
                {
                    str.Append(c);
                }
                var wmofilename = str.ToString();
                //var wmoreader = new WMOReader();
                //wmoreader.LoadWMO(wmofilename);
            }
        }

        private static MPHD ReadMPHDChunk(BinaryReader bin)
        {
            return bin.Read<MPHD>();
        }

        private static Dictionary<(byte, byte), MapFileDataIDs> ReadMAIDChunk(BinaryReader bin)
        {
            var tileFiles = new Dictionary<(byte, byte), MapFileDataIDs>();
            for (byte x = 0; x < 64; x++)
            {
                for (byte y = 0; y < 64; y++)
                {
                    tileFiles.Add((y, x), bin.Read<MapFileDataIDs>());
                }
            }
            return tileFiles;
        }

        private static Dictionary<(byte, byte), MapFileDataIDs2> ReadMAI2Chunk(BinaryReader bin)
        {
            var tileFiles = new Dictionary<(byte, byte), MapFileDataIDs2>();
            for (byte x = 0; x < 64; x++)
            {
                for (byte y = 0; y < 64; y++)
                {
                    tileFiles.Add((y, x), bin.Read<MapFileDataIDs2>());

                    if(tileFiles[(y, x)].unknown0 != 0 || tileFiles[(y, x)].unknown1 != 0 || tileFiles[(y, x)].unknown2 != 0 || tileFiles[(y, x)].unknown3 != 0 ||
                       tileFiles[(y, x)].unknown4 != 0 || tileFiles[(y, x)].unknown5 != 0 || tileFiles[(y, x)].unknown6 != 0 || tileFiles[(y, x)].unknown7 != 0)
                    {
                        Console.WriteLine(string.Format("WDT: Found non-zero data in MAI2 chunk for tile {0},{1} at offset {2}!", y, x, bin.BaseStream.Position.ToString()));
                        Console.WriteLine("\t unknown0: " + tileFiles[(y, x)].unknown0);
                        Console.WriteLine("\t unknown1: " + tileFiles[(y, x)].unknown1);
                        Console.WriteLine("\t unknown2: " + tileFiles[(y, x)].unknown2);
                        Console.WriteLine("\t unknown3: " + tileFiles[(y, x)].unknown3);
                        Console.WriteLine("\t unknown4: " + tileFiles[(y, x)].unknown4);
                        Console.WriteLine("\t unknown5: " + tileFiles[(y, x)].unknown5);
                        Console.WriteLine("\t unknown6: " + tileFiles[(y, x)].unknown6);
                        Console.WriteLine("\t unknown7: " + tileFiles[(y, x)].unknown7);
                    }
                }
            }
            return tileFiles;
        }

        private static Structs.ADT.MODF ReadMODFChunk(BinaryReader bin)
        {
            var modfEntry = new Structs.ADT.MODF();
            modfEntry.entries = new Structs.ADT.MODFEntry[1];
            modfEntry.entries[0] = bin.Read<Structs.ADT.MODFEntry>();
            return modfEntry;
        }

        private static MANM ReadMANMChunk(BinaryReader bin)
        {
            var manm = new MANM();
            manm.version = bin.ReadUInt32();
            manm.countB = bin.ReadUInt32();
            manm.entriesB = new MANM_B[manm.countB];
            for (var i = 0; i < manm.countB; i++)
            {
                manm.entriesB[i] = new MANM_B();
                manm.entriesB[i].c = bin.ReadUInt32();
                manm.entriesB[i].d = bin.ReadBytes(1480);
                //manm.entriesB[i].type = bin.ReadUInt32();
                //manm.entriesB[i].s = bin.ReadUInt32();
                manm.entriesB[i].posPlusNormalCount = bin.ReadUInt32();
                manm.entriesB[i].posPlusNormal = new MANMPosPlusNormal[manm.entriesB[i].posPlusNormalCount];
                for (var j = 0; j < manm.entriesB[i].posPlusNormalCount; j++)
                {
                    manm.entriesB[i].posPlusNormal[j] = bin.Read<MANMPosPlusNormal>();
                }

                bin.ReadBytes(28 * ((int)manm.entriesB[i].posPlusNormalCount - 3));

            }
            return manm;
        }

        private void ReadWDT(Stream wdt)
        {
            if (wdt == null)
                return;

            var bin = new BinaryReader(wdt);
            long position = 0;

            // We make a list of chunks first to detect the WDT sub-file type
            var chunkList = new List<string>();
            while (position < wdt.Length)
            {
                wdt.Position = position;

                var chunkName = new string(bin.ReadChars(4).Reverse().ToArray());
                var chunkSize = bin.ReadUInt32();

                position = wdt.Position + chunkSize;

                if (chunkName == "MVER")
                {
                    var version = bin.ReadUInt32();
                    wdtfile.version = version;
                }

                chunkList.Add(chunkName);
            }

            wdt.Position = 0;
            position = 0;

            foreach (var chunk in chunkList)
            {
                if (chunk == "MAIN" || chunk == "MAID" || chunk == "MWMO" || chunk == "MPHD" || chunk == "MODF" || chunk == "MANM")
                {
                    type = WDTType.Root;
                    break;
                }
                else if (chunk == "MAOI" || chunk == "MAOH")
                {
                    type = WDTType.Occlusion;
                    break;
                }
                else if (chunk == "MPLT" || chunk == "MPL2" || chunk == "MPL3" || chunk == "MSLT" || chunk == "MTEX" || chunk == "MLTA")
                {
                    type = WDTType.Lights;
                    break;
                }
                else if (chunk == "VFOG" || chunk == "VFEX")
                {
                    type = WDTType.Fogs;
                    break;
                }
                else if (chunk == "PVPD" || chunk == "PVMI" || chunk == "PVBD")
                {
                    type = WDTType.MapParticuleVolumes;
                    break;
                }
                else if (chunk == "MMFE")
                {
                    type = WDTType.Preload;
                    break;
                }
                else if (chunk != "MVER")
                {
                    Console.WriteLine(string.Format("WDT: Found unknown chunk at offset {1} \"{0}\"/ while trying to detect WDT type!", chunk, position.ToString()));
                }
            }

            if (type == WDTType.Unknown)
            {
                // We can kinda guess type by version while they don't overlap for some types
                switch (wdtfile.version)
                {
                    case 20:
                        type = WDTType.Lights;
                        break;
                    case 2:
                        type = WDTType.Fogs;
                        break;
                    case 3:
                        type = WDTType.MapParticuleVolumes;
                        break;
                    case 18: // Root or Occlusion
                    default:
                        Console.WriteLine("Couldn't detect WDT type by chunk names, and version " + wdtfile.version + " is not recognized, so WDT type remains unknown.");
                        break;
                }
            }

            while (position < wdt.Length)
            {
                wdt.Position = position;

                var chunkName = (WDTChunks)bin.ReadUInt32();
                var chunkSize = bin.ReadUInt32();

                position = wdt.Position + chunkSize;

                if (type == WDTType.Root)
                {
                    if (wdtfile.version != 18)
                        throw new Exception("Unsupported root WDT version " + wdtfile.version + "!");

                    switch (chunkName)
                    {
                        case WDTChunks.MVER:
                            break;
                        case WDTChunks.MAIN:
                            wdtfile.tiles = ReadMAINChunk(bin);
                            break;
                        case WDTChunks.MWMO:
                            ReadMWMOChunk(bin, chunkSize);
                            break;
                        case WDTChunks.MPHD:
                            wdtfile.mphd = ReadMPHDChunk(bin);
                            break;
                        case WDTChunks.MAID:
                            wdtfile.tileFiles = ReadMAIDChunk(bin);
                            break;
                        case WDTChunks.MAI2:
                            wdtfile.tileFiles2 = ReadMAI2Chunk(bin);
                            break;
                        case WDTChunks.MODF:
                            wdtfile.modf = ReadMODFChunk(bin);
                            break;
                        case WDTChunks.MANM:
                            wdtfile.manm = ReadMANMChunk(bin);
                            break;
                        default:
                            Console.WriteLine(string.Format("WDT: Found unknown header at offset {1} \"{0}\"/\"{2}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString(), Encoding.UTF8.GetString(BitConverter.GetBytes((uint)chunkName))));
                            break;
                    }
                }
                else
                {
                    throw new Exception("WDT type " + Enum.GetName(type) + " is not supported!");
                }
            }
        }
    }
}
