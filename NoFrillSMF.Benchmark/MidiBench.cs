using System.IO;
using System;
using BenchmarkDotNet.Attributes;
using Melanchall.DryWetMidi.Smf;
using NoFrillSMF;
using NAudio.Midi;

namespace NoFrillSMF.Benchmark
{
    [MonoJob, CoreJob]
    [MemoryDiagnoser]
    public class MidiBench
    {
        string fileName = "/home/matthew/development/NoFrillSMF/NoFrillSMF.Tests/midifiles/notes-9k.mid";


        MidiFile reader = new MidiFile(false);

        [GlobalSetup]
        public void Setup()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }
        }

        [Benchmark]
        public void NoFrillSMFEnum()
        {
            var types = new Events.EventType[1] { Events.EventType.TrackName };

            foreach (var track in reader.GetTrackChunks())
            {
                foreach (var ev in track.ParseEvents<NoFrillSMF.Events.MetaEvents.TextEvent>(types))
                {
                    // Console.WriteLine(ev.Text);
                    break;
                }
            }
        }
        // [Benchmark]
        // public void NoFrillSMF()
        // {
        //     MidiFile reader = new MidiFile();
        //     using (FileStream fs = File.OpenRead(fileName))
        //     {
        //         reader.ReadData(fs);
        //     }
        //     reader.Parse();
        // }


        //     [Benchmark]
        //     public void NAudioMidi()
        //     {
        //         using (FileStream fs = File.OpenRead(fileName))
        //         {
        //             var mf = new NAudio.Midi.MidiFile(fs, false);
        //         }
        //     }

        //     [Benchmark]
        //     public void DryWetMidi()
        //     {
        //         using (FileStream fs = File.OpenRead(fileName))
        //         {
        //             Melanchall.DryWetMidi.Smf.MidiFile.Read(fs);
        //         }
        //     }
    }
}