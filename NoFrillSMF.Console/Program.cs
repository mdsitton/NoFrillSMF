using System.IO;
using System;

using NoFrillSMF;

namespace NoFrillSMF.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            MidiFile reader = new MidiFile();

            using (Stream file = File.OpenRead(args[0]))
            {
                reader.ReadData(file);
            }
            using (Stream file = File.OpenWrite(args[1]))
            {
                reader.WriteData(file);
            }
        }
    }
}
