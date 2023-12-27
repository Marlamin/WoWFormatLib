using System.Collections.Generic;
using WoWFormatLib.Utils;

namespace WoWFormatLib.DBC
{
    public static class LiquidVertexFormatLookup
    {
        private static Dictionary<uint, ushort> liquidObjectToLVFDict = new Dictionary<uint, ushort>();
        private static bool loaded = false;

        private static void Load()
        {
            liquidObjectToLVFDict = new Dictionary<uint, ushort>();

            var dbcd = new DBCD.DBCD(new CASCDBCProvider(), new DBCD.Providers.GithubDBDProvider());
            var loStorage = dbcd.Load("LiquidObject", CASC.BuildNameFormatted);
            var ltStorage = dbcd.Load("LiquidType", CASC.BuildNameFormatted);
            var lmStorage = dbcd.Load("LiquidMaterial", CASC.BuildNameFormatted);

            foreach (dynamic loEntry in loStorage.Values)
            {
                foreach (dynamic ltEntry in ltStorage.Values)
                {
                    if (loEntry.LiquidTypeID == ltEntry.ID)
                    {
                        foreach (dynamic lmEntry in lmStorage.Values)
                        {
                            if (ltEntry.MaterialID == lmEntry.ID)
                            {
                                liquidObjectToLVFDict.Add((uint)loEntry.ID, (ushort)lmEntry.LVF);
                            }
                        }
                    }
                }
            }

            loaded = true;
        }

        public static void TryGetLVF(uint liquidObjectID, out ushort LVF)
        {
            if (!loaded) Load();

            liquidObjectToLVFDict.TryGetValue(liquidObjectID, out LVF);
        }
    }
}
