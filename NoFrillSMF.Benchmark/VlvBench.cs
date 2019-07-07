using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;

using NoFrill.Common;
using BenchmarkDotNet.Attributes;
using System.Text;

namespace NoFrillSMF.Benchmark
{
    public static class VlvImp
    {
        public static void WriteVlv(this byte[] data, UInt32 value, ref int offset)
        {
            Debug.Assert(value <= 0xfffffff);

            uint byteCount = 0;
            UInt32 store = value & 0x7F;

            while ((value >>= 7) != 0)
            {
                store <<= 8;
                store |= (value & 0x7F) | 0x80;
                byteCount++;
            }

            BinUtils.SwapEndianess(store);
            Unsafe.CopyBlockUnaligned(ref Unsafe.As<UInt32, byte>(ref store), ref data[offset], byteCount);
            offset += (int)byteCount;
        }

        public static void WriteVlvStandard(this byte[] data, UInt32 value, ref int offset)
        {
            Debug.Assert(value <= 0xfffffff);

            UInt32 store = value & 0x7F;

            while ((value >>= 7) != 0)
            {
                store <<= 8;
                store |= (value & 0x7F) | 0x80;
            }

            do
            {
                data[offset++] = (byte)(store);
            }
            while (((store >>= 8) & 0x80) != 0);
        }

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

        public static UInt32 ReadVlvAlternate(this byte[] data, ref int offset)
        {
            UInt32 value = 0;

            for (int i = 0; i < 4; ++i)
            {
                byte c = data[offset++];

                value <<= 7;
                value |= (UInt32)(c & 0x7f);

                if ((c & 0x80) == 0)
                    break;
            }
            return value;
        }
    }

    [MonoJob, CoreJob]
    public class VlvBench
    {

        UInt32[] srcData;
        byte[] data;
        int itemCount;

        public UInt32[] GenerateRandomValues(int count)
        {
            UInt32[] outData = new UInt32[count];
            System.Random rnd = new System.Random();

            for (int i = 0; i < count; ++i)
            {
                outData[i] = (UInt32)rnd.Next(0, 0xFFFFFFF);
            }
            return outData;
        }

        [GlobalSetup]
        public void SetupData()
        {
            itemCount = 100;
            data = new byte[itemCount * 4];
            srcData = GenerateRandomValues(itemCount);

            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVlvStandard(val, ref pos);
            }
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
        public void WriteVlvPerformance()
        {
            int pos = 0;
            foreach (var val in srcData)
            {
                data.WriteVlv(val, ref pos);
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
    }
}