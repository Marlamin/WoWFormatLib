using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs;
using WoWFormatLib.Structs.BLS;

namespace WoWFormatLib.FileReaders
{
    public class GFATReader
    {
        private uint fdid;
        public GFAT LoadGFAT(uint fileDataID)
        {
            if (!FileProvider.FileExists(fileDataID))
            {
                throw new FileNotFoundException("BLS " + fileDataID + " not found!");
            }

            fdid = fileDataID;

            return LoadGFAT(FileProvider.OpenFile(fileDataID));
        }
        public GFAT LoadGFAT(byte[] cKey)
        {
            return LoadGFAT(FileProvider.OpenFile(cKey));
        }

        public GFAT LoadGFAT(Stream stream)
        {
            var shaderComboFile = new GFAT();
            shaderComboFile.rawBLSPerGFX = new Dictionary<string, byte[]>();
            shaderComboFile.shaderPerGFX = new Dictionary<string, BLS>();

            //var shaderDir = Path.Combine("extract", "gfat", fdid.ToString());

            //if (!Directory.Exists(shaderDir))
            //    Directory.CreateDirectory(shaderDir);

            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                string magic = new string(reader.ReadChars(4));
                if (magic != "TAFG")
                {
                    throw new InvalidDataException("Invalid GFAT magic: " + magic);
                }

                uint version = reader.ReadUInt32();
                if (version != 0x1000B)
                {
                    throw new InvalidDataException("Unsupported GFAT version: " + version);
                }

                int numAPIs = reader.ReadSByte();
                reader.ReadBytes(3); // padding
                for (int i = 0; i < numAPIs; i++)
                {
                    var apiChars = reader.ReadChars(4);
                    Array.Reverse(apiChars);
                    string apiName = new string(apiChars);
                    uint startOffset = reader.ReadUInt32();
                    uint endOffset = reader.ReadUInt32();

                    var prevPos = stream.Position;
                    stream.Seek(startOffset, SeekOrigin.Begin);
                    //shaderComboFile.rawBLSPerGFX[apiName] = reader.ReadBytes((int)(endOffset - startOffset));
                    var blsBytes = reader.ReadBytes((int)(endOffset - startOffset));
                    stream.Position = prevPos;
                    var blsReader = new BLSReader();
                    BLS blsShader = blsReader.LoadBLS(new MemoryStream(blsBytes));
                    shaderComboFile.shaderPerGFX[apiName] = blsShader;

                    //var shaderIndex = 0;
                    //foreach(var decompressedShader in blsShader.decompressedShaders)
                    //{
                    //    File.WriteAllBytes(Path.Combine(shaderDir, "shader_" + apiName + "_" + shaderIndex++ + ".dxbc"), decompressedShader);
                    //}
                }
            }

           // File.WriteAllText(Path.Combine("extract", "gfat", fdid + ".json"), JsonConvert.SerializeObject(shaderComboFile, Newtonsoft.Json.Formatting.Indented));
            return shaderComboFile;
        }
    }
}
