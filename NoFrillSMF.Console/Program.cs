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

            if (File.Exists(args[1]))
            {
                File.Delete(args[1]);
            }

            using (Stream file = new FileStream(args[1], FileMode.CreateNew))
            {
                reader.WriteData(file);
            }
            System.Console.WriteLine("Finished");
            return 0;
        }
    }
}
