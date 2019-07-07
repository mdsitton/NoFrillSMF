using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace NoFrillSMF.Benchmark
{
    public class CopyBench
    {
        byte[] bytes = Encoding.UTF8.GetBytes("MyProperty1");
        byte[] to;

        public CopyBench()
        {
            to = new byte[bytes.Length];
        }

        [Benchmark]
        public unsafe void MarshalCopyFixed()
        {
            fixed (byte* cPtr = to)
            {
                IntPtr dstPtr = new IntPtr(cPtr);
                Marshal.Copy(bytes, 0, dstPtr, bytes.Length);
            }
        }

        [Benchmark]
        public unsafe void MarshalCopy()
        {

            IntPtr dstPtr = new IntPtr(Unsafe.AsPointer(ref to[0]));
            Marshal.Copy(bytes, 0, dstPtr, bytes.Length);
        }

        [Benchmark]
        public void ArrayCopy()
        {
            Array.Copy(bytes, 0, to, 0, to.Length);
        }

        [Benchmark]
        public unsafe void CopyBlockUnaligned()
        {
            fixed (void* s = &bytes[0])
            fixed (void* p = &to[0])
            {
                Unsafe.CopyBlockUnaligned(p, s, (uint)bytes.Length);
            }
        }

        [Benchmark]
        public unsafe void CopyBlockUnaligned2()
        {
            void* s = Unsafe.AsPointer(ref bytes[0]);
            void* p = Unsafe.AsPointer(ref to[0]);
            Unsafe.CopyBlockUnaligned(p, s, (uint)bytes.Length);
        }

        [Benchmark]
        public void CopyBlockUnalignedSafe()
        {
            Unsafe.CopyBlockUnaligned(ref to[0], ref bytes[0], (uint)bytes.Length);
        }

        [Benchmark(Baseline = true)]
        public void BlockCopy()
        {
            Buffer.BlockCopy(bytes, 0, to, 0, to.Length);
        }
    }
}