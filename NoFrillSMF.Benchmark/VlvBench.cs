using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;

using NoFrill.Common;
using BenchmarkDotNet.Attributes;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime;
using NoFrillSMF;
using BenchmarkDotNet.Jobs;

namespace NoFrillSMF.Benchmark
{
    public static class VlvImp
    {

        public static void WriteVarIntNAudioOriginal(this byte[] data, uint value, ref int offset)
        {
            Debug.Assert(value <= 0xfffffff);

            int n = 0;
            byte[] buffer = new byte[4];
            do
            {
                buffer[n++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            } while (value > 0);

            buffer[0] &= 0x7F;

            while (n > 0)
            {
                n--;
                data[offset++] = buffer[n];
            }
        }

        public static unsafe void WriteVarIntNAudio(this byte[] data, uint value, ref int offset)
        {
            Debug.Assert(value <= 0xfffffff);

            int n = 1;
            UInt32 store = value & 0x7F;
            byte* buffer = (byte*)Unsafe.AsPointer(ref store);
            value >>= 7;

            while (value > 0)
            {
                buffer[n++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }

            while (n > 0)
            {
                data[offset++] = buffer[--n];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteVlvStandard(this byte[] data, UInt32 value, ref int offset)
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

        public static void WriteVlv(this byte[] data, UInt32 value, ref int offset)
        {
            Debug.Assert(value <= 0xfffffff);

            uint byteCount = 1;
            UInt32 store = value & 0x7F;
            value >>= 7;

            while (value != 0)
            {
                store <<= 8;
                store |= (value & 0x7F) | 0x80;
                value >>= 7;
                byteCount++;
            }

            BinUtils.SwapEndianess(store);
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<UInt32, byte>(ref store), ref data[offset], byteCount);
            Console.WriteLine(byteCount);
            offset += (int)byteCount;
        }

        public static UInt32 ReadVarIntNAudioBr(this BinaryReader reader)
        {
            UInt32 value = 0;
            byte b;
            for (UInt32 n = 0; n < 4; n++)
            {
                b = reader.ReadByte();
                value <<= 7;
                value += (UInt32)(b & 0x7F);
                if ((b & 0x80) == 0)
                {
                    return value;
                }
            }
            throw new FormatException("Invalid Var Int");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReadVarIntNAudio(this byte[] data, ref int offset)
        {
            byte b = data[offset++];
            UInt32 value = b & 0x7FU;

            while (true)
            {

                if ((b & 0x80) == 0)
                    return value;

                b = data[offset++];
                value <<= 7;
                value |= b & 0x7FU;

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static int GetBitCount(uint bits)
        {
            int count = 0;

            // This takes advantage of the fact that when you subtrack 1 from a bit it sets the bits under it all to 1
            // Which when & with itself clears both the original bit and all lower bits and exposes the next bit to count.

            while (bits != 0)
            {
                count++;
                bits &= (uint)(bits - 1);
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvNoBranch(this byte[] data, ref int offset)
        {
            uint fullData = Unsafe.As<byte, UInt32>(ref data[offset]);

            uint nextMask = 0x80;

            uint v1b = (nextMask & fullData);
            nextMask = v1b << 8;

            uint v2b = (nextMask & fullData);
            nextMask = v2b << 8;

            uint v3b = (nextMask & fullData);

            uint value;

            if (v3b != 0)
            {
                value = ((fullData & 0x7FU) << 21) |
                        ((fullData & 0x7F00U) << 6) |
                        ((fullData & 0x7F0000U) >> 9) |
                        ((fullData & 0x7F000000U) >> 24);
                offset += 4;
            }
            else if (v2b != 0)
            {
                value = ((fullData & 0x7FU) << 14) |
                        ((fullData & 0x7F00U) >> 1) |
                        ((fullData & 0x7F0000U) >> 16);
                offset += 3;
            }
            else if (v1b != 0)
            {
                value = (fullData & 0x7FU) << 7 | ((fullData & 0x7F00U) >> 8);
                offset += 2;
            }
            else
            {
                value = (fullData & 0x7FU);
                offset += 1;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvStandardUnroll(this byte[] data, ref int offset)
        {
            uint c = data[offset++];
            UInt32 value = c & 0x7FU;

            // uint c1 = 0, c2 = 0, c3 = 0;
            if ((c & 0x80) != 0)
            {
                c = data[offset++];
                value = (value << 7) | (c & 0x7FU);
            }
            if ((c & 0x80) != 0)
            {
                c = data[offset++];
                value = (value << 7) | (c & 0x7FU);
            }
            if ((c & 0x80) != 0)
            {
                c = data[offset++];
                value = (value << 7) | (c & 0x7FU);
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvStandard(this byte[] data, ref int offset)
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
        public static UInt32 ReadVlvAlternate(this byte[] data, ref int offset)
        {
            unchecked
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadUInt8(this byte[] data, ref int offset)
        {
            return data[offset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static byte ReadUInt8Unsafe(this byte[] data, ref int offset)
        {
            byte* dataPtr = (byte*)Unsafe.AsPointer(ref data);
            return dataPtr[offset++];
        }
    }


    [SimpleJob(RuntimeMoniker.Net48)]
    // [SimpleJob(RuntimeMoniker.Mono)]
    [SimpleJob(RuntimeMoniker.Net50)]
    public class VlvBench
    {

        UInt32[] srcData;
        byte[] data;
        int itemCount;
        MemoryStream ms;
        BinaryReader br;

        public UInt32[] GenerateValues(int count)
        {
            UInt32[] outData = new UInt32[count];

            // for (int i = 0; i <= 127; ++i)
            // {
            //     outData[i] = (UInt32)(i * i * i * i);
            // }
            // for (int i = 128; i <= 255; ++i)
            // {
            //     outData[i] = (UInt32)(i * i);
            // }
            // for (int i = 256; i <= 383; ++i)
            // {
            //     outData[i] = (UInt32)(i * i * i);
            // }
            for (int i = 0; i < count; ++i)
            {
                var newI = i + 15;
                outData[i] = (UInt32)(newI * newI * newI * newI);
            }
            return outData;
        }

        [GlobalSetup]
        public void SetupData()
        {
            itemCount = 128;
            data = new byte[itemCount * 4];
            srcData = GenerateValues(itemCount);

            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVlvStandard(val, ref pos);
            }
            ms = new MemoryStream(data);
            br = new BinaryReader(ms);
        }

        [Benchmark]
        public void ReadInt8Performance()
        {
            byte[] dataRead = new byte[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadUInt8(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception("Bad");
            // }
        }

        [Benchmark]
        public void ReadVlvPerformanceNoBranch()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlvNoBranch(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception($"Bad val:{dataRead[itemCount - 1]} good val: {srcData[itemCount - 1]}");
            // }
        }

        [Benchmark]
        public void ReadVlvPerformance()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlvStandard(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception("Bad");
            // }
        }

        [Benchmark]
        public void ReadVlvPerformanceUnroll()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlvStandardUnroll(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception("Bad");
            // }
        }

        [Benchmark]
        public void ReadVlvNAudioPerformanceBr()
        {
            UInt32[] dataRead = new UInt32[itemCount];
            ms.Position = 0;

            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = br.ReadVarIntNAudioBr();
            }
        }

        [Benchmark]
        public void ReadVlvNAudioPerformance()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVarIntNAudio(ref pos);
            }
        }

        [Benchmark]
        public void ReadVlvAltPerformance()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlvAlternate(ref pos);
            }
        }

        [Benchmark]
        public void WriteVlvStdPerformance()
        {
            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVlvStandard(val, ref pos);
            }
        }

        [Benchmark]
        public void WriteVlvNAudioPerformance()
        {
            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVarIntNAudio(val, ref pos);
            }
        }

        [Benchmark]
        public void WriteVlvNAudioOriginalPerformance()
        {
            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVarIntNAudioOriginal(val, ref pos);
            }
        }


        [Benchmark]
        public void WriteVlvPerformance()
        {
            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVlv(val, ref pos);
            }
        }

    }
}