using System;
using System.Linq;
using BenchmarkDotNet.Running;
using System.Diagnostics;

using static _globals;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

static class _globals
{
    [MethodImpl(MethodImplOptions.NoInlining), DebuggerHidden]
    public static void Nop<T>(out T x) => x = default(T);
};

namespace NoFrillSMF.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Contains("--vlvBench"))
                BenchmarkRunner.Run<VlvBench>();
            else if (args.Contains("--copyBench"))
                BenchmarkRunner.Run<CopyBench>();
            else if (args.Contains("--binaryBench"))
                BenchmarkRunner.Run<BinaryBench>();
            else if (args.Contains("--fileBench"))
                BenchmarkRunner.Run<FileBench>();
            else if (args.Contains("--midiBench"))
                BenchmarkRunner.Run<MidiBench>();
            else
            {
                Console.WriteLine("Error: Please select which benchmark:\n\t--vlvBench\n\t--copyBench\n\t--binaryBench\n\t--midiBench\n\t--fileBench");
            }
            // bool[] t = null;
            // MidiBench test = new MidiBench();
            // test.Setup();
            // Stopwatch sw = new Stopwatch();
            // sw.Start();

            // double time1 = sw.Elapsed.TotalMilliseconds;
            // for (int i = 0; i < 10; ++i)
            // {
            // }
            // double time2 = sw.Elapsed.TotalMilliseconds;
            // Console.WriteLine($"{time2 - time1}ms");
            // // sw.Reset();

            // time1 = sw.Elapsed.TotalMilliseconds;
            // for (int i = 0; i < 1; ++i)
            // {
            //     t = test.NoFrillSMFEnum();

            // }
            // time2 = sw.Elapsed.TotalMilliseconds;
            // Console.WriteLine($"{time2 - time1}ms");
            // // sw.Reset();

            // time1 = sw.Elapsed.TotalMilliseconds;
            // for (int i = 0; i < 1; ++i)
            // {
            //     t = test.NoFrillSMFEnum();

            // }
            // time2 = sw.Elapsed.TotalMilliseconds;
            // Console.WriteLine($"{time2 - time1}ms");
            // // sw.Reset();

            // time1 = sw.Elapsed.TotalMilliseconds;
            // for (int i = 0; i < 1; ++i)
            // {
            //     t = test.NoFrillSMFEnum();

            // }
            // time2 = sw.Elapsed.TotalMilliseconds;
            // Console.WriteLine($"{time2 - time1}ms");
            // // sw.Reset();

            // time1 = sw.Elapsed.TotalMilliseconds;
            // for (int i = 0; i < 1; ++i)
            // {
            //     t = test.NoFrillSMFEnum();

            // }
            // time2 = sw.Elapsed.TotalMilliseconds;
            // Console.WriteLine($"{time2 - time1}ms");
            // // sw.Reset();
            // foreach (var i in t)
            // {
            //     Console.WriteLine(i);
            // }
        }
    }
}
