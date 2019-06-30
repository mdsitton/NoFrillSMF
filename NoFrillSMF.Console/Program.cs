using System.IO;
using System;

using NoFrillSMF;

namespace NoFrillSMF.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Stream file = File.OpenRead(args[0]))
            {
                SmfReader reader = new SmfReader(file);
                reader.ParseData();
            }
        }
    }
}
