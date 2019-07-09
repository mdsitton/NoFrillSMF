using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using BenchmarkDotNet.Attributes;

namespace NoFrillSMF.Benchmark
{
    [MonoJob]
    public class FileBench
    {
        string fileName = "/home/matthew/development/NoFrillSMF/NoFrillSMF.Tests/midifiles/notes-9k.mid";


        // [Benchmark]
        public void SafeMemoryMappedEndFilePerformance()
        {
            var size = new FileInfo(fileName).Length;
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open))
            using (var access = mmf.CreateViewAccessor(size - 64, 64))
            {
                byte b = access.ReadByte(access.Capacity - 1);
            }
        }

        [Benchmark]
        public unsafe void MemoryMappedFilePerformance()
        {
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open))
            using (var access = mmf.CreateViewAccessor())
            {
                byte* filePtr = null;
                access.SafeMemoryMappedViewHandle.AcquirePointer(ref filePtr);
                byte b = filePtr[access.Capacity - 1];
                access.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }


        [Benchmark]
        public unsafe void MemoryMappedNoViewPerformance()
        {
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open))
            // using (var access = mmf.CreateViewAccessor())
            {
                // byte* filePtr = null;
                // access.SafeMemoryMappedViewHandle.AcquirePointer(ref filePtr);

                // byte b = 0;

                // for (int i = 0; i < access.Capacity; ++i)
                // {
                //     b = filePtr[i];
                // }
                // access.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        [Benchmark]
        public unsafe void MemoryMappedSingleFilePerformance()
        {
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open))
            using (var access = mmf.CreateViewAccessor())
            {
                byte* filePtr = null;
                access.SafeMemoryMappedViewHandle.AcquirePointer(ref filePtr);

                byte b = 0;

                for (int i = 0; i < access.Capacity; ++i)
                {
                    b = filePtr[i];
                }
                access.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        // [Benchmark]
        // public void FileStreamSinglePerformance()
        // {
        //     byte[] ba = new byte[4];
        //     using (FileStream fs = File.OpenRead(fileName))
        //     {
        //         for (int i = 0; i < fs.Length / 4; ++i)
        //             fs.Read(ba, 0, 4);
        //     }
        // }

        [Benchmark]
        public void FileStreamPerformance()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                fs.Seek(1, SeekOrigin.End);
                byte b = (byte)fs.ReadByte();
            }
        }

        [Benchmark]
        public void SafeMemoryMappedFullFilePerformance()
        {
            byte[] data;
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName))
            using (var access = mmf.CreateViewAccessor())
            {
                data = new byte[access.Capacity];
                access.ReadArray<byte>(0, data, 0, (int)access.Capacity);
            }
        }

        [Benchmark]
        public unsafe void MemoryMappedFullFilePerformance()
        {
            byte[] data;
            using (var mmf = MemoryMappedFile.CreateFromFile(fileName))
            using (var access = mmf.CreateViewAccessor())
            {
                data = new byte[access.Capacity];
                byte* filePtr = null;
                access.SafeMemoryMappedViewHandle.AcquirePointer(ref filePtr);
                Unsafe.CopyBlockUnaligned((byte*)Unsafe.AsPointer(ref data[0]), filePtr, (uint)access.Capacity);
                access.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        [Benchmark]
        public void FileStreamFullPerformance()
        {
            File.ReadAllBytes(fileName);
        }

    }
}