using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoFrillSMF
{
    internal static class ArrayParseUtils
    {
        public static string ReadString(this byte[] data, ref int position, byte[] scratch, int size, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;

            Array.Copy(data, position, scratch, 0, size);
            return encoding.GetString(scratch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReadVlvStandard(this byte[] data, ref int offset)
        {
            UInt32 value = 0;

            byte c;

            do
            {
                c = data[offset++];
                value = (value << 7) | (UInt32)(c & 0x7f);
            }
            while ((c & 0x80) != 0);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteVlv(this byte[] data, UInt32 value, ref int offset)
        {
            Debug.Assert(value <= 0xfffffff);

            UInt32 store = value & 0x7F;
            value >>= 7;

            while (value != 0)
            {
                store <<= 8;
                store |= (value & 0x7F) | 0x80;
                value >>= 7;
            }

            while (true)
            {
                data[offset++] = (byte)(store);

                if ((store & 0x80) != 0)
                    store >>= 8;
                else
                    break;
            }
        }
    }
}
