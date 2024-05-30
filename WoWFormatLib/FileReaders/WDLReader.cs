﻿using System;
using System.IO;
using System.Text;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.WDL;

namespace WoWFormatLib.FileReaders
{
    public class WDLReader
    {
        public void LoadWDL(string filename)
        {
            if (FileProvider.FileExists(filename))
            {
                using (Stream tex = FileProvider.OpenFile(filename))
                {
                    ReadWDL(filename, tex);
                }
            }
            else
            {
                throw new FileNotFoundException("WDL " + filename + " does not exist");
            }
        }
        private void ReadMVERChunk(BinaryReader bin)
        {
            if (bin.ReadUInt32() != 18)
            {
                throw new Exception("Unsupported WDL version!");
            }
        }
        private void ReadMWMOChunk(BinaryReader bin, uint size)
        {
            var wmoFilesChunk = bin.ReadBytes((int)size);

            var str = new StringBuilder();

            for (var i = 0; i < wmoFilesChunk.Length; i++)
            {
                if (wmoFilesChunk[i] == '\0')
                {
                    if (str.Length > 1)
                    {
                        if (!FileProvider.FileExists(str.ToString()))
                        {
                            Console.WriteLine("WMO file does not exist!!! {0}", str.ToString());
                        }
                    }
                    str = new StringBuilder();
                }
                else
                {
                    str.Append((char)wmoFilesChunk[i]);
                }
            }
        }
        private void ReadWDL(string filename, Stream wdl)
        {
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
                        ReadMVERChunk(bin);
                        continue;
                    case WDLChunks.MWMO:
                        ReadMWMOChunk(bin, chunkSize);
                        continue;
                    case WDLChunks.MWID:
                    case WDLChunks.MODF:
                    case WDLChunks.MAOF: //contains MARE and MAHO subchunks
                    case WDLChunks.MARE:
                    case WDLChunks.MAOC: //New in WoD
                    case WDLChunks.MAHO:
                        continue;
                    default:
                        Console.WriteLine(string.Format("{2} Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkName, position.ToString(), filename));
                        break;
                }
            }
        }
    }
}
