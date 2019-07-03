using System.Linq;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        public static UInt32 ReadUInt32_3(this Stream data, byte[] scratch, bool flipEndianness)
        {
            data.Read(scratch, 0, 4);
            var val = BitConverter.ToUInt32(scratch, 0);

            return flipEndianness ? SwapUInt32(val) : val;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReadUInt32Unsafe(this byte[] buff, int offset)
        {
            unsafe
            {
                fixed (byte* start = buff)
                {
                    UInt32* current = (UInt32*)(start + offset);
                    return BinaryUtilities.SwapUInt32(*current);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 SwapUInt32(UInt32 i)
        {
            UInt32 tmp = (i << 16) | ((i >> 16) & 0x00ffff);
            return ((tmp >> 8) & 0x00ff00ff) | ((tmp & 0x00ff00ff) << 8);
        }

        public static T SwapBytes<T>(T pv) where T : struct
        {

            int size = Marshal.SizeOf(pv);
            IntPtr arrPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(pv, arrPtr, true);

            unsafe
            {
                int lo, hi;
                byte* p = (byte*)&arrPtr;
                for (lo = 0, hi = size - 1; hi > lo; lo++, hi--)
                {
                    byte tmp = p[lo];
                    p[lo] = p[hi];
                    p[hi] = tmp;
                }
            }
            return pv;
        }

        public static UInt32 ShiftyReadUInt32BE(this Stream data, byte[] scratch)
        {
            data.Read(scratch, 0, 4);
            return (UInt32)scratch[0] | (UInt32)scratch[1] << 8 | (UInt32)scratch[2] << 16 | (UInt32)scratch[3] << 24;
        }

    }

    public class DataTest
    {
        public UInt32[] values;//(UInt32)UnityEngine.Random.Range(0, Int32.MaxValue);

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
        DataTest test;
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
            test = new DataTest();
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

        UInt32[] ReadDataBitConverter3(Stream data, int itemCount)
        {
            byte[] scratch = new byte[4];

            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = data.ReadUInt32_3(scratch, true);
            }
            return outData;
        }

        UInt32[] ReadDataBitConverter_noswap(Stream data, int itemCount)
        {
            byte[] scratch = new byte[4];

            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                data.Read(scratch, 0, 4);
                outData[i] = BitConverter.ToUInt32(scratch, 0);
            }
            return outData;
        }

        UInt32[] ReadDataShifty(Stream data, int itemCount)
        {
            byte[] scratch = new byte[4];

            UInt32[] outData = new UInt32[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = data.ShiftyReadUInt32BE(scratch);
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

        UInt32[] ReadDataUnsafeSwap(MemoryStream data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            byte[] bytes = data.GetBuffer();

            unsafe
            {
                fixed (byte* start = bytes)
                {
                    UInt32* current = (UInt32*)start;
                    for (int i = 0; i < itemCount; ++i)
                    {

                        outData[i] = BinaryPrimitives.ReverseEndianness(*current);

                        current++;
                    }
                }
            }
            return outData;
        }

        UInt32[] ReadDataUnsafeSwapStandalone(MemoryStream data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            byte[] bytes = data.GetBuffer();

            unsafe
            {
                fixed (byte* start = bytes)
                {
                    UInt32* current = (UInt32*)start;
                    for (int i = 0; i < itemCount; ++i)
                    {

                        outData[i] = BinaryUtilities.SwapUInt32(*current);

                        current++;
                    }
                }
            }
            return outData;
        }

        UInt32[] ReadDataUnsafeSwapStandaloneFunctioned(MemoryStream data, int itemCount)
        {
            UInt32[] outData = new UInt32[itemCount];

            var buff = data.GetBuffer();
            int start = (int)data.Position;
            for (int i = 0; i < itemCount; ++i)
            {
                outData[i] = buff.ReadUInt32Unsafe(start + (i * 4));
            }
            data.Position += (itemCount * 4);
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
        public void BitConverter3PerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataBitConverter3(ms, itemCount);
        }

        [Benchmark]
        public void BitConverterNoSwapPerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataBitConverter_noswap(ms, itemCount);
        }

        [Benchmark]
        public void ShiftyPerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values1 = ReadDataShifty(ms, itemCount);
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
        public void BinaryUnsafeSwapStandalonePerformanceTest()
        {
            UInt32[] values2 = ReadDataUnsafeSwapStandalone(ms, itemCount);
        }

        [Benchmark]
        public void BinaryUnsafeSwapStandaloneFunctionedPerformanceTest()
        {
            ms.Position = 0;
            UInt32[] values2 = ReadDataUnsafeSwapStandaloneFunctioned(ms, itemCount);
            //Console.Write(values2.SequenceEqual(test.values));
        }

        [Benchmark]
        public void BinaryUnsafeSwapPerformanceTest()
        {
            UInt32[] values2 = ReadDataUnsafeSwap(ms, itemCount);
        }

        [Benchmark]
        public void BinaryUnsafePerformanceTest()
        {
            UInt32[] values2 = ReadDataUnsafeFrom(ms, itemCount);
        }
    }
}