using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWFormatLib.Structs
{
    public struct GFAT
    {
        public Dictionary<string, byte[]> rawBLSPerGFX;
        public Dictionary<string, BLS.BLS> shaderPerGFX;
    }
}
