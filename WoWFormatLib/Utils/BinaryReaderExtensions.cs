using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WoWFormatLib
{
    public static class Extensions
    {
        public static void Skip(this BinaryReader reader, int bytes)
        {
            reader.BaseStream.Position += bytes;
        }

        public static T Read<T>(this BinaryReader reader) where T : struct
        {
            Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<T>()];
            reader.ReadExactly(buffer);
            return MemoryMarshal.Read<T>(buffer);
        }
    }

    public static class CStringExtensions
    {
        /// <summary> Reads the NULL terminated string from 
        /// the current stream and advances the current position of the stream by string length + 1.
        /// <seealso cref="BinaryReader.ReadString"/>
        /// </summary>
        public static string ReadCString(this BinaryReader reader)
        {
            return reader.ReadCString(Encoding.UTF8);
        }

        /// <summary> Reads the NULL terminated string from 
        /// the current stream and advances the current position of the stream by string length + 1.
        /// <seealso cref="BinaryReader.ReadString"/>
        /// </summary>
        public static string ReadCString(this BinaryReader reader, Encoding encoding)
        {
            using var ms = new MemoryStream();

            byte b;
            while ((b = reader.ReadByte()) != 0)
                ms.WriteByte(b);

            return encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }
    }
}
