namespace WoWFormatLib.Utils
{
    public struct BGRAColor
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;

        public BGRAColor(byte b, byte g, byte r, byte a)
            : this()
        {
            B = b;
            G = g;
            R = r;
            A = a;
        }
    }
}