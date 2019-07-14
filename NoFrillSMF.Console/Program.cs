using System.IO;
using System;

using NoFrillSMF;

namespace NoFrillSMF.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 2)
            {
                System.Console.WriteLine("ERROR: Invalid arguments");
                return 1;
            }
            MidiFile reader = new MidiFile();

            using (Stream file = File.OpenRead(args[0]))
            {
                reader.ReadData(file);
            }

            reader.Parse();

            using (Stream file = File.OpenWrite(args[1]))
            {
                reader.WriteData(file);
            }
            System.Console.WriteLine("Finished");
            return 0;
        }
    }
}
