using System;
using BenchmarkDotNet.Running;

namespace NoFrillSMF.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BinaryBench>();
        }
    }
}
