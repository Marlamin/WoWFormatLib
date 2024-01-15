namespace WoWFormatLib.Utils
{
    public static class VersionManager
    {
        public enum FileVersion
        {
            PreRelease = 0,
            Vanilla = 1,
            TBC = 2,
            WotLK = 3,
            Cata = 4,
            MoP = 5,
            WoD = 6,
            Legion = 7,
            BfA = 8,
            SL = 9,
            DF = 10
        }

        public static FileVersion CurrentVersion = FileVersion.DF;
        public static FileVersion TargetVersion = FileVersion.DF;
    }
}
