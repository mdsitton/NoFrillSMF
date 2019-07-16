using System.IO;
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
        string fileName = "/home/matthew/development/NoFrillSMF/NoFrillSMF.Tests/midifiles/notes.mid";


        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public void NoFrillSMF()
        {
            MidiFile reader = new MidiFile();
            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }
            reader.Parse();
        }

        [Benchmark]
        public void NoFrillSMFEnum()
        {
            MidiFile reader = new MidiFile();
            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }

            foreach (var track in reader.GetTrackChunks())
            {
                foreach (var ev in track.GetEvents())
                {
                    bool foundDiffs = true;
                    switch (ev.note)
                    {
                        case 60:
                        case 72:
                        case 84:
                        case 96: foundDiffs = true; break;

                        case 61:
                        case 73:
                        case 85:
                        case 97: foundDiffs = true; break;

                        case 62:
                        case 74:
                        case 86:
                        case 98: foundDiffs = true; break;

                        case 63:
                        case 75:
                        case 87:
                        case 99: foundDiffs = true; break;

                        case 64:
                        case 76:
                        case 88:
                        case 100: foundDiffs = true; break;

                        case 65:
                        case 77:
                        case 89:
                        case 101:
                            foundDiffs = true; break;
                        default:
                            //Console.WriteLine("Unknown note: " + note.NoteNumber);
                            foundDiffs = false; break; ;
                    }
                    if (foundDiffs)
                        break;
                }
            }
        }

        [Benchmark]
        public void NAudioMidi()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                var mf = new NAudio.Midi.MidiFile(fs, false);
            }
        }

        [Benchmark]
        public void DryWetMidi()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                Melanchall.DryWetMidi.Smf.MidiFile.Read(fs);
            }
        }
    }
}