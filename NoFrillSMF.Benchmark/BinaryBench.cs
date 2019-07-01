using System.Diagnostics;
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

        public static UInt32 ReadUInt32_2(this Stream data, byte[] scratch, bool flipEndianness)
        {
            data.Read(scratch, 0, 4);
            var val = BitConverter.ToUInt32(scratch, 0);

            return flipEndianness ? BinaryPrimitives.ReverseEndianness(val) : val;
        }

        public static uint SwapUInt32(uint i)
        {
            return ((i & 0xFF000000) >> 24) | ((i & 0x00FF0000) >> 8) | ((i & 0x0000FF00) << 8) | ((i & 0x000000FF) << 24);
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
        private BinaryReader br;
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
            br = new BinaryReader(ms);
            test.Serialize(ms, true);
        }

        [GlobalCleanup]
        public void DisposeData()
        {
            ms.Dispose();
        }

        UInt32[] ReadDataBinaryReader(BinaryReader data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = BinaryUtilities.SwapUInt32(data.ReadUInt32());
            }
            return outData;
        }

        UInt32[] ReadDataBitConverter2(Stream data, int itemCount)
        {
            byte[] scratch = new byte[4];

            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = data.ReadUInt32_2(scratch, true);
            }
            return outData;
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

        UInt32[] ReadDataBinaryPrimitives(ReadOnlySpan<byte> data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(i * 4, 4));
            }
            return outData;
        }

        UInt32[] ReadDataBinaryPrimitivesFromMS(MemoryStream data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            ReadOnlySpan<byte> sp = data.GetBuffer().AsSpan(0, (int)data.Length);

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = BinaryPrimitives.ReadUInt32BigEndian(sp.Slice(i * 4, 4));
            }
            return outData;
        }

        UInt32[] ReadDataUnsafeFrom(MemoryStream data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            byte[] bytes = data.GetBuffer();

            unsafe
            {
                UInt32 tmp = 0;

                fixed (byte* start = bytes)
                {
                    byte* current = start;
                    byte* tmpPtr = (byte*)&tmp;
                    for (int i = 0; i < itemCount; ++i)
                    {
                        for (int x = 0; x < 4; ++x)
                        {
                            tmpPtr[x] = current[3 - x];
                        }

                        outData[i] = tmp;

                        current += 4;
                    }
                }
            }
            return outData;
        }

        [Benchmark]
        public void BinaryReaderBitSwapPerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataBinaryReader(br, itemCount);
        }

        [Benchmark]
        public void BitConverterPerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataBitConverter(ms, itemCount);
        }

        [Benchmark]
        public void BitConverter2PerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataBitConverter2(ms, itemCount);
        }

        [Benchmark]
        public void BinaryPrimitivesPerformanceTest()
        {
            ReadOnlySpan<byte> sp = ms.GetBuffer().AsSpan(0, (int)ms.Length);
            UInt32[] values2 = ReadDataBinaryPrimitives(sp, itemCount);
        }

        [Benchmark]
        public void BinaryPrimitivesMSPerformanceTest()
        {
            UInt32[] values2 = ReadDataBinaryPrimitivesFromMS(ms, itemCount);
        }

        [Benchmark]
        public void BinaryUnsafePerformanceTest()
        {
            UInt32[] values2 = ReadDataUnsafeFrom(ms, itemCount);
        }
    }
}