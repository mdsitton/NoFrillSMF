using System;
using System.Text;
using System.IO;

namespace NoFrillSMF
{
    internal static class StreamParseUtils
    {
        public static UInt32 ReadUInt32(this Stream data, byte[] scratch, bool flipEndianness)
        {
            data.Read(scratch, 0, 4);

            if (flipEndianness)
                Array.Reverse(scratch);

            return BitConverter.ToUInt32(scratch, 0);
        }

        public static UInt32 ReadUInt24(this Stream data, byte[] scratch, bool flipEndianness)
        {
            data.Read(scratch, flipEndianness ? 1 : 0, 3);

            if (flipEndianness)
            {
                scratch[0] = 0;
                Array.Reverse(scratch);
            }
            else
            {
                scratch[3] = 0;
            }

            return BitConverter.ToUInt32(scratch, 0);
        }

        public static UInt16 ReadUInt16(this Stream data, byte[] scratch, bool flipEndianness)
        {
            data.Read(scratch, 2, 2);

            if (flipEndianness)
                Array.Reverse(scratch);

            return BitConverter.ToUInt16(scratch, 0);
        }

        public static string ReadString(this Stream data, byte[] scratch, int size, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;
            data.Read(scratch, 0, size);
            return Encoding.ASCII.GetString(scratch);
        }
    }
}
