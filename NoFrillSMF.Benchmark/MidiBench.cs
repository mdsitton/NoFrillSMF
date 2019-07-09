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
            src = File.ReadAllBytes(fileName);
            ms = new MemoryStream(src);
        }

        // [Benchmark]
        // public void NAudioMidi()
        // {
        //     ms.Position = 0;
        //     var mf = new NAudio.Midi.MidiFile(ms, false);
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
            ms.Position = 0;

            NoFrillSMF.MidiFile reader = new NoFrillSMF.MidiFile();

            reader.ReadData(src);
        }
    }
}