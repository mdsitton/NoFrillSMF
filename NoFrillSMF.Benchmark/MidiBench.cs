using System.IO;
using BenchmarkDotNet.Attributes;
using Melanchall.DryWetMidi.Smf;
using NoFrillSMF;
using NAudio.Midi;

namespace NoFrillSMF.Benchmark
{
    [MonoJob, CoreJob]
    public class MidiBench
    {
        string fileName = "/home/matthew/development/NoFrillSMF/NoFrillSMF.Tests/midifiles/notes-9k.mid";

        MemoryStream ms;
        byte[] src;

        MidiFile reader = new MidiFile();

        [GlobalSetup]
        public void Setup()
        {
            // = File.ReadAllBytes(fileName);
            //ms = new MemoryStream(src);

            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }
        }

        // [Benchmark]
        // public void NAudioMidi()
        // {
        //     using (FileStream fs = File.OpenRead(fileName))
        //     {
        //         var mf = new NAudio.Midi.MidiFile(fs, false);
        //     }
        // }

        // [Benchmark]
        // public void DryWetMidi()
        // {
        //     ms.Position = 0;
        //     Melanchall.DryWetMidi.Smf.MidiFile.Read(ms);
        // }

        [Benchmark]
        public void NoFrillSMF()
        {
            //ms.Position = 0;
            reader.Parse();
        }
    }
}