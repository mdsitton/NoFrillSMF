using System;
using System.Linq;
using BenchmarkDotNet.Running;

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
        }
    }
}
