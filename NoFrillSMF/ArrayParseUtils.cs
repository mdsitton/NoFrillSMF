using System;
using System.Text;

namespace NoFrillSMF
{
    internal static class ArrayParseUtils
    {
        public static UInt32 ReadUInt32(this byte[] data, ref int position, byte[] scratch, bool flipEndianness)
        {
            Array.Copy(data, position, scratch, 0, 4);
            position += 4;

            if (flipEndianness)
                Array.Reverse(scratch);

            return BitConverter.ToUInt32(scratch, 0);
        }

        public static UInt32 ReadUInt24(this byte[] data, ref int position, byte[] scratch, bool flipEndianness)
        {
            Array.Copy(data, position, scratch, flipEndianness ? 1 : 0, 3);
            position += 3;

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

        public static UInt16 ReadUInt16(this byte[] data, ref int position, byte[] scratch, bool flipEndianness)
        {
            Array.Copy(data, position, scratch, 2, 2);
            position += 2;

            if (flipEndianness)
                Array.Reverse(scratch);

            return BitConverter.ToUInt16(scratch, 0);
        }

        public static string ReadString(this byte[] data, ref int position, byte[] scratch, int size, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;

            Array.Copy(data, position, scratch, 0, size);
            return encoding.GetString(scratch);
        }
    }
}
