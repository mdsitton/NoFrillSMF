using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using BinaryEx;

namespace NoFrillSMF
{
    public static class ArrayParseUtils
    {

        [ThreadStatic] static byte[] scratchData;

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        private static byte[] EnsureScratch(int size)
        {
            if (scratchData == null || scratchData.Length < size)
            {
                scratchData = new byte[size];
            }

            return scratchData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static string ReadString(this byte[] data, ref int offset, int size, Encoding encoding = null)
        {
            byte[] scratch = EnsureScratch(size);
            encoding = encoding ?? Encoding.ASCII;

            Unsafe.CopyBlockUnaligned(ref scratch[0], ref data[offset], (uint)size);
            offset += size;
            return encoding.GetString(scratch, 0, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public unsafe static int WriteString(this byte[] data, ref int offset, string str, Encoding encoding = null)
        {
            byte[] scratch = EnsureScratch(str.Length);
            encoding = encoding ?? Encoding.ASCII;
            int count = encoding.GetBytes(str, 0, str.Length, scratch, 0);

            Unsafe.CopyBlockUnaligned(ref data[offset], ref scratch[0], (uint)count);
            offset += count;
            return count;
        }

        /// <summary>
        /// Reads a MIDI variable length value from the data array.
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlv(this byte[] data, ref int offset)
        {
            byte val1 = data[offset];
            if (val1 < 0x80)
            {
                ++offset;
                return val1;
            }
            byte val2 = data[++offset];
            if (val2 < 0x80)
            {
                ++offset;
                return ((val1 & 0x7FU) << 7) |
                        ((val2 & 0x7FU));
            }

            byte val3 = data[++offset];
            if (val3 < 0x80)
            {
                ++offset;
                return ((val1 & 0x7FU) << 14) |
                        ((val2 & 0x7FU) << 7) |
                        ((val3 & 0x7FU));
            }

            byte val4 = data[++offset];
            ++offset;
            return ((val1 & 0x7FU) << 21) |
                    ((val2 & 0x7FU) << 14) |
                    ((val3 & 0x7FU) << 7) |
                    ((val4 & 0x7FU));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static void WriteVlv(this byte[] data, ref int offset, int value)
        {
            Debug.Assert(value <= 0xfffffff);

            int store = value & 0x7F;
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
