using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NoFrillSMF
{
    public static class ArrayParseUtils
    {

        [ThreadStatic] static byte[] scratchData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] EnsureScratch(int size)
        {
            if (scratchData == null || scratchData.Length < size)
            {
                scratchData = new byte[size];
            }

            return scratchData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this byte[] data, ref int offset, int size, Encoding encoding = null)
        {
            byte[] scratch = EnsureScratch(size);
            encoding = encoding ?? Encoding.ASCII;

            Unsafe.CopyBlockUnaligned(ref scratch[0], ref data[offset], (uint)size);
            offset += size;
            return encoding.GetString(scratch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int WriteString(this byte[] data, ref int offset, string str, Encoding encoding = null)
        {
            byte[] scratch = EnsureScratch(str.Length);
            encoding = encoding ?? Encoding.ASCII;
            int count = 0;
            fixed (char* chr = str)
            {
                count = encoding.GetBytes(chr, 0, (byte*)Unsafe.AsPointer(ref scratch[0]), scratch.Length);
            }

            Unsafe.CopyBlockUnaligned(ref data[offset], ref scratch[0], (uint)count);
            offset += count;
            return count;
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
        public static void WriteVlv(this byte[] data, ref int offset, UInt32 value)
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
