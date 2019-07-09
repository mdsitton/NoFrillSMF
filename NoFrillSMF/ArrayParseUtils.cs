using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoFrillSMF
{
    public static class ArrayParseUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this byte[] data, ref int offset, byte[] scratch, int size, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;

            Unsafe.CopyBlockUnaligned(ref scratch[0], ref data[offset], (uint)size);
            offset += size;
            return encoding.GetString(scratch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReadVlv(this byte[] data, ref int offset)
        {
            byte c = data[offset++];
            UInt32 value = c & 0x7FU;


            while ((c & 0x80) != 0)
            {
                c = data[offset++];
                value <<= 7;
                value |= c & 0x7FU;
            }

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
