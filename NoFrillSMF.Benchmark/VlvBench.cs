using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;

using BinaryEx;
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
            Endian.SwapEndianess(store);
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<UInt32, byte>(ref store), ref data[offset], byteCount);
            Console.WriteLine(byteCount);
            offset += (int)byteCount;
        }

        public static uint ReadVarIntNAudioBr(this BinaryReader reader)
        {
            uint value = 0;
            byte b;
            for (uint n = 0; n < 4; n++)
            {
                b = reader.ReadByte();
                value <<= 7;
                value += (uint)(b & 0x7F);
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

            // This takes advantage of the fact that when you subtract 1 from a bit it sets the bits under it all to 1
            // Which when & with itself clears both the original bit and all lower bits and exposes the next bit to count.

            while (bits != 0)
            {
                count++;
                bits &= (uint)(bits - 1);
            }

            return count;
        }

        // bad this doesn't work
        // [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        // public static uint ReadVlvBranchless(this byte[] data, ref int offset)
        // {
        //     uint fullData = Unsafe.ReadUnaligned<uint>(ref data[offset]);

        //     // get each continuation bit, and use it to mask the next one
        //     // this ensures that once the continuation bit is 0 the masks
        //     // generated later will also mask any data that is not
        //     // part of the vlv
        //     uint v1c = fullData & 0x80;
        //     uint v2c = fullData & (v1c << 8);
        //     uint v3c = fullData & (v2c << 8);

        //     // count the number of bytes that are used
        //     uint bitCount = 1 + (v1c >> 7) + (v2c >> 15) + (v3c >> 23);

        //     // build masks for each bit that is set
        //     uint v0mask = 0x7FU;
        //     uint v1mask = ((v1c << 8) - 1) & 0xFF00U;
        //     uint v2mask = ((v2c << 8) - 1) & 0xFF0000U;
        //     uint v3mask = ((v3c << 8) - 1) & 0xFF000000U;

        //     uint v0 = (fullData & v0mask);
        //     uint v1 = (fullData & v1mask) >> 1;
        //     uint v2 = (fullData & v2mask) >> 2;
        //     uint v3 = (fullData & v3mask) >> 3;

        //     offset += (int)bitCount;
        //     return v0 | v1 | v2 | v3;
        // }


        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvFastBranchOld(this byte[] data, ref int offset)
        {
            uint fullData = Unsafe.As<byte, uint>(ref data[offset]);

            uint v1c = fullData & 0x80;
            uint v2c = fullData & (v1c << 8);
            uint v3c = fullData & (v2c << 8);

            if (v3c != 0)
            {
                uint value0 = (fullData & 0x7FU) << 21;
                uint value1 = (fullData & 0x7F00U) << 6;
                uint value2 = (fullData & 0x7F0000U) >> 9;
                uint value3 = (fullData & 0x7F000000U) >> 24;
                offset += 4;
                return value0 | value1 | value2 | value3;
            }
            else if (v2c != 0)
            {
                uint value0 = (fullData & 0x7FU) << 14;
                uint value1 = (fullData & 0x7F00U) >> 1;
                uint value2 = (fullData & 0x7F0000U) >> 16;
                offset += 3;
                return value0 | value1 | value2;
            }
            else if (v1c != 0)
            {
                uint value0 = (fullData & 0x7FU) << 7;
                uint value1 = (fullData & 0x7F00U) >> 8;
                offset += 2;
                return value0 | value1;
            }

            offset += 1;
            return fullData & 0x7FU;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvFastBranch(this byte[] data, ref int offset)
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
                // offset += 2;
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


        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvStandard3(this byte[] data, ref int offset)
        {
            var localOffset = offset;
            byte c = data[localOffset];
            UInt32 value = c & 0x7FU;

            while (c >= 0x80)
            {
                c = data[++localOffset];
                value <<= 7;
                value |= c & 0x7FU;
            }
            ++localOffset;
            offset = localOffset;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvSonic(this byte[] data, ref int offset)
        {
            var localOffset = offset;
            uint value = data[localOffset];
            if (value >= 0x80)
            {
                value &= 0x7FU;
                do
                {
                    value <<= 7;
                    value |= data[++localOffset] & 0x7FU;
                } while (data[localOffset] >= 0x80);
            }
            ++localOffset;
            offset = localOffset;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static uint ReadVlvDryWetMidi(this byte[] data, ref int offset)
        {
            uint value = 0;
            byte b;
            do
            {
                b = data[offset++];
                value = (value << 7) + (b & 127U);
            }
            while (b >> 7 != 0);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
        public static UInt32 ReadVlvStandard2(this byte[] data, ref int offset)
        {
            byte c = data[offset++];
            uint value = c & 0x7FU;

            while (c >= 0x80)
            {
                c = data[offset++];
                value <<= 7;
                value |= c & 0x7FU;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), TargetedPatchingOptOut("Inline across assemplies")]
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
    [SimpleJob(RuntimeMoniker.Net70)]
    // [SimpleJob(RuntimeMoniker.Mono)]
    public class VlvBench
    {

        UInt32[] srcData;
        byte[] data;
        uint itemCount;
        MemoryStream ms;
        BinaryReader br;

        public UInt32[] GenerateValues(uint count)
        {
            UInt32[] outData = new UInt32[count];

            uint eachCount = count / 4;
            uint increment1 = 127 / eachCount;
            uint offset1 = 0;

            uint increment2 = 32640 / eachCount;
            uint offset2 = 0x80;

            uint increment3 = 8355840 / eachCount;
            uint offset3 = 0x8000;

            uint increment4 = 2139095040 / eachCount;
            uint offset4 = 0x800000;

            for (uint i = 0; i < eachCount; ++i)
            {
                outData[i * 4] = offset1 + (i * increment1);
            }
            for (uint i = 0; i < eachCount; ++i)
            {
                outData[(i * 4) + 1] = offset2 + (i * increment2);
            }
            for (uint i = 0; i < eachCount; ++i)
            {
                outData[(i * 4) + 2] = offset3 + (i * increment3);
            }
            for (uint i = 0; i < eachCount; ++i)
            {
                outData[(i * 4) + 3] = (offset4 + (i * increment4)) & 0xfffffff;
            }
            return outData;
        }


        public UInt32[] GenerateSmallValues(uint count)
        {
            UInt32[] outData = new UInt32[count];

            for (uint i = 0; i < count; ++i)
            {
                outData[i] = i;
            }
            return outData;
        }

        public UInt32[] GenerateMediumValues(uint count)
        {
            UInt32[] outData = new UInt32[count];

            for (uint i = 0; i < count; ++i)
            {
                outData[i] = 0x7f + (i * 10);
            }
            return outData;
        }

        [GlobalSetup]
        public void SetupData()
        {
            itemCount = 512;
            data = new byte[itemCount * 4];
            srcData = GenerateMediumValues(itemCount);

            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVlvStandard(val, ref pos);
            }
            ms = new MemoryStream(data);
            br = new BinaryReader(ms);
        }

        // [Benchmark]
        // public void ReadInt8Performance()
        // {
        //     byte[] dataRead = new byte[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadUInt8(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        [Benchmark]
        public void ReadVlvPerformanceFastBranch()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlvFastBranch(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception($"Bad val:{dataRead[itemCount - 1]} good val: {srcData[itemCount - 1]}");
            // }
        }

        // [Benchmark]
        // public void ReadVlvPerformanceFastBranchOld()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvFastBranchOld(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception($"Bad val:{dataRead[itemCount - 1]} good val: {srcData[itemCount - 1]}");
        //     // }
        // }

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
        public void ReadVlvPerformance3()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlvStandard3(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception("Bad");
            // }
        }

        // [Benchmark]
        // public void ReadVlvPerformance2()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvStandard2(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvSonicPerformance()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvSonic(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvDryWetMidiPerformance()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvDryWetMidi(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvPerformanceUnroll()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvStandardUnroll(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvNAudioPerformanceBr()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];
        //     ms.Position = 0;

        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = br.ReadVarIntNAudioBr();
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvNAudioPerformance()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVarIntNAudio(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvAltPerformance()
        // {
        //     UInt32[] dataRead = new UInt32[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvAlternate(ref pos);
        //     }
        //     // if (!dataRead.SequenceEqual(srcData))
        //     // {
        //     //     throw new Exception("Bad");
        //     // }
        // }

        // [Benchmark]
        // public void ReadVlvBranchlessPerformance()
        // {
        //     uint[] dataRead = new uint[itemCount];

        //     int pos = 0;
        //     for (int i = 0; i < itemCount; ++i)
        //     {
        //         dataRead[i] = data.ReadVlvBranchless(ref pos);
        //     }
        //     if (!dataRead.SequenceEqual(srcData))
        //     {
        //         throw new Exception("Bad");
        //     }
        // }

        // [Benchmark]
        // public void WriteVlvStdPerformance()
        // {
        //     int pos = 0;
        //     foreach (var val in srcData)
        //     {
        //         data.WriteVlvStandard(val, ref pos);
        //     }
        // }

        // [Benchmark]
        // public void WriteVlvNAudioPerformance()
        // {
        //     int pos = 0;
        //     foreach (var val in srcData)
        //     {
        //         data.WriteVarIntNAudio(val, ref pos);
        //     }
        // }

        // [Benchmark]
        // public void WriteVlvNAudioOriginalPerformance()
        // {
        //     int pos = 0;
        //     foreach (var val in srcData)
        //     {
        //         data.WriteVarIntNAudioOriginal(val, ref pos);
        //     }
        // }


        // [Benchmark]
        // public void WriteVlvPerformance()
        // {
        //     int pos = 0;
        //     foreach (var val in srcData)
        //     {
        //         data.WriteVlv(val, ref pos);
        //     }
        // }

    }
}