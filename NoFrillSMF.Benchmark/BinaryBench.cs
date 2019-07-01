using BenchmarkDotNet.Attributes;
using System;
using System.Buffers.Binary;
using System.IO;

namespace NoFrillSMF.Benchmark
{

    public static class BinaryUtilities
    {
        public static void WriteBytesUInt32(this Stream stream, UInt32 data, bool flipEndianness)
        {
            byte[] bytesOut = BitConverter.GetBytes(data);

            if (flipEndianness)
                Array.Reverse(bytesOut);

            stream.Write(bytesOut, 0, bytesOut.Length);
        }

        public static UInt32 ReadUInt32(this Stream data, byte[] scratch, bool flipEndianness)
        {
            data.Read(scratch, 0, 4);

            if (flipEndianness)
                Array.Reverse(scratch);

            return BitConverter.ToUInt32(scratch, 0);
        }

    }

    public class DataTest
    {
        UInt32[] values;//(UInt32)UnityEngine.Random.Range(0, Int32.MaxValue);

        public void SetData(UInt32[] data) => values = data;

        public void Serialize(Stream data, bool flipEndianess)
        {
            foreach (var value in values)
            {
                data.WriteBytesUInt32(value, flipEndianess);
            }
        }
    }

    [MonoJob, CoreJob]
    public class BinaryBench
    {
        private MemoryStream ms;
        private int itemCount;

        public UInt32[] GenerateRandomValues(int count)
        {
            UInt32[] outData = new UInt32[count];
            System.Random rnd = new System.Random();

            for (int i = 0; i < count; ++i)
            {
                outData[i] = (UInt32)rnd.Next(0, int.MaxValue);
            }
            return outData;
        }


        [GlobalSetup]
        public void SetupData()
        {
            itemCount = 100;
            DataTest test = new DataTest();
            test.SetData(GenerateRandomValues(itemCount));
            ms = new MemoryStream();
            test.Serialize(ms, true);
        }

        [GlobalCleanup]
        public void DisposeData()
        {
            ms.Dispose();
        }

        UInt32[] ReadDataBitConverter(Stream data, int itemCount)
        {
            byte[] scratch = new byte[4];

            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = data.ReadUInt32(scratch, true);
            }
            return outData;
        }

        UInt32[] ReadDataBinaryPrimitives(Span<byte> data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(i * 4, 4));
            }
            return outData;
        }

        [Benchmark]
        public void BitConverterPerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataBitConverter(ms, itemCount);
        }

        [Benchmark]
        public void BinaryPrimitivesPerformanceTest()
        {
            Span<byte> sp = new Span<byte>(ms.GetBuffer(), 0, (int)ms.Length);
            UInt32[] values2 = ReadDataBinaryPrimitives(sp, itemCount);
        }

    }
}