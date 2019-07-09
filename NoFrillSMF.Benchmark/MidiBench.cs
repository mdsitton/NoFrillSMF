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

        [GlobalSetup]
        public void Setup()
        {
            // = File.ReadAllBytes(fileName);
            //ms = new MemoryStream(src);
        }

        [Benchmark]
        public void NAudioMidi()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                var mf = new NAudio.Midi.MidiFile(fs, false);
            }
        }

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

            MidiFile reader = new MidiFile();

            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }
        }
    }
}