using System.IO;

namespace WoWFormatLib
{
    public interface IFileProvider
    {
        // FileDataID-based file methods
        bool FileExists(uint filedataid);
        Stream OpenFile(uint filedataid);
        uint GetFileDataIdByName(string filename);

        // Filename-based file methods
        Stream OpenFile(string filename);
        bool FileExists(string filename);

        // CKey-based file methods
        Stream OpenFile(byte[] cKey);
        bool FileExists(byte[] cKey);

        // Build related methods
        void SetBuild(string build);
    }
}
