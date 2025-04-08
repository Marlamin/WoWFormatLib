using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WoWFormatLib.FileProviders;
using WoWFormatLib.Structs.WWF;

namespace WoWFormatLib.FileReaders
{
    public class WWFReader
    {
        public WWF Load(uint fileDataID)
        {
            var wwf = new WWF();

            using (var stream = FileProvider.OpenFile(fileDataID))
            using (var bin = new BinaryReader(stream))
            {
                wwf.type = (WWFType)bin.ReadUInt32();
                if (wwf.type != WWFType.JSON && wwf.type != WWFType.Binary)
                    throw new Exception("Unsupported WWF type " + wwf.type);

                wwf.magic = bin.ReadUInt32();

                if (wwf.magic != 0x932c64b4)
                    throw new NotImplementedException("Encountered unknown WWF magic " + wwf.magic);
                else
                    wwf.name = "WWFParticulateGroup";

                if(wwf.type == WWFType.JSON)
                {
                    var asString = new string(bin.ReadChars((int)bin.BaseStream.Length - (int)bin.BaseStream.Position));

                    JsonSerializerOptions jsonSerializerOptions = new()
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString,
                        PropertyNameCaseInsensitive = true
                    };

                    wwf.data = JsonSerializer.Deserialize<WWFData>(asString, jsonSerializerOptions);
                }
                else
                {
                    wwf.data = bin.Read<WWFData>();
                }
            }

            return wwf;
        }
    }
}
