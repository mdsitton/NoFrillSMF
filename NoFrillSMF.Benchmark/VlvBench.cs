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


    [MonoJob, CoreJob]
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

            for (int i = 0; i < count; ++i)
            {
                outData[i] = (UInt32)(i * 2684354);
            }
            return outData;
        }

        [GlobalSetup]
        public void SetupData()
        {
            itemCount = 100;
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
        public void ReadInt8UnsafePerformance()
        {
            byte[] dataRead = new byte[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadUInt8Unsafe(ref pos);
            }
            // if (!dataRead.SequenceEqual(srcData))
            // {
            //     throw new Exception("Bad");
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
        public void ReadVlvPerformanceNFSMF()
        {
            UInt32[] dataRead = new UInt32[itemCount];

            int pos = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                dataRead[i] = data.ReadVlv(ref pos);
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