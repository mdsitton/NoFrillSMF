using System.IO;
using System;
using BenchmarkDotNet.Attributes;
using Melanchall.DryWetMidi;
using NoFrillSMF;
using NAudio.Midi;
using System.Collections.Generic;
using BenchmarkDotNet.Jobs;
using Melanchall.DryWetMidi.Core;

namespace NoFrillSMF.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net48)]
    // [SimpleJob(RuntimeMoniker.Mono)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class MidiBench
    {
        string fileName = "E:\\development\\NoFrillSMF\\NoFrillSMF.Tests\\midifiles\\rb4midi.mid";


        MidiFile reader = new MidiFile(false);

        byte[] midiData;

        static List<NoFrillSMF.Events.EventType> types = new List<NoFrillSMF.Events.EventType> { NoFrillSMF.Events.EventType.NoteOn, NoFrillSMF.Events.EventType.NoteOff };
        static Chunks.TrackChunk.TrackEventFilter filterObj = new Chunks.TrackChunk.TrackEventFilter(eventTemplates: types);

        static List<NoFrillSMF.Events.EventType> typesText = new List<NoFrillSMF.Events.EventType> { NoFrillSMF.Events.EventType.TrackName };
        static Chunks.TrackChunk.TrackEventFilter filterObjText = new Chunks.TrackChunk.TrackEventFilter(eventTemplates: typesText);

        static List<NoFrillSMF.Events.EventType> typesLyrics = new List<NoFrillSMF.Events.EventType> { NoFrillSMF.Events.EventType.Text, NoFrillSMF.Events.EventType.Lyrics };
        static Chunks.TrackChunk.TrackEventFilter filterObjLyric = new Chunks.TrackChunk.TrackEventFilter(eventTemplates: typesLyrics);

        [GlobalSetup]
        public void Setup()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }
            midiData = File.ReadAllBytes(fileName);
        }

        public enum Difficulty
        {
            Expert = 0,
            Hard = 1,
            Medium = 2,
            Easy = 3,
        }

        public enum Instrument
        {
            Guitar = 0,
            GuitarCoop = 1,
            Bass = 2,
            Keys = 3,
            Drums = 4,
            GHLGuitar = 5,
            GHLBass = 6,
            Vocals = 7,
            Crowd = 8,
            Rhythm = 9,
            None = 10,
        }

        /// <summary>
        /// Determine the <see cref="Song.Difficulty"> of a midi note value.
        /// </summary>
        /// <param name="noteNumber">The midi note value</param>
        /// <returns>Returns <see cref="Song.Difficulty"> derrived from the note value.</returns>
        public static Nullable<Difficulty> SelectNoteDifficulty(int noteNumber)
        {
            if (noteNumber >= 58 && noteNumber <= 66)
                return Difficulty.Easy;
            else if (noteNumber >= 70 && noteNumber <= 78)
                return Difficulty.Medium;
            else if (noteNumber >= 82 && noteNumber <= 90)
                return Difficulty.Hard;
            else if (noteNumber >= 94 && noteNumber <= 102)
                return Difficulty.Expert;
            else
                return null;
        }

        public static Instrument ToInstrument(string trackName)
        {
            switch (trackName.ToLowerInvariant())
            {
                case "t1 gems":
                case "part guitar":
                    return Instrument.Guitar;
                case "part guitar coop":
                    return Instrument.GuitarCoop;
                case "part guitar ghl":
                    return Instrument.GHLGuitar;
                case "part bass ghl":
                    return Instrument.GHLBass;
                case "part bass":
                    return Instrument.Bass;
                case "part drums":
                    return Instrument.Drums;
                case "part keys":
                    return Instrument.Keys;
                case "part rhythm":
                    return Instrument.Rhythm;
                case "part vocals":
                    return Instrument.Vocals;
            }
            return Instrument.None;
        }

        private static readonly int _numOfDifficulties = Enum.GetNames(typeof(Difficulty)).Length;
        private static readonly int _numOfInstruments = Enum.GetNames(typeof(Instrument)).Length;



        [Benchmark]
        public void NoFrillSMFEnum()
        {

            var tracks = new Chunks.TrackChunk[_numOfInstruments];
            var difficultyLoaded = new bool[_numOfInstruments, _numOfDifficulties];
            int lyric = 0;

            foreach (var track in reader.GetTrackChunks())
            {
                foreach (var ev in track.ParseEvents<Events.MetaEvents.TrackNameEvent>(filterObjText))
                {
                    // Console.WriteLine(ev.Text);
                    if (ev.Text == "PART VOCALS")
                    {
                        foreach (var evl in track.ParseEvents<Events.MetaEvents.TextEvent>(filterObjLyric))
                        {
                            // Console.WriteLine(evl.Text);
                            lyric += evl.Text.Length;
                        }
                        // Console.WriteLine(lyric);
                    }
                    var instrument = ToInstrument(ev.Text);

                    if (instrument != Instrument.None)
                    {
                        tracks[(int)instrument] = track;
                        break;
                    }
                }
            }
            int trackIndex = 0;
            foreach (var track in tracks)
            {
                if (track == null)
                {
                    continue;
                }

                // difficultyLoaded.Clear();

                int found = 0;
                foreach (var ev in track.ParseEvents<Events.MidiEvents.NoteEvent>(filterObj))
                {
                    var noteDiff = SelectNoteDifficulty(ev.Note);

                    if (noteDiff == null)
                        continue;

                    var val = (int)noteDiff.Value;

                    if (difficultyLoaded[trackIndex, val] == false)
                    {
                        difficultyLoaded[trackIndex, val] = true;
                        found++;
                    }

                    if (found == 4)
                    {
                        break;
                    }
                }
                trackIndex++;
            }
            // return difficultyLoaded;
        }

        [Benchmark]
        public void DryWetMidiFull()
        {
            var tracks = new Melanchall.DryWetMidi.Core.TrackChunk[_numOfInstruments * _numOfDifficulties];
            bool[] difficultyLoaded = new bool[4];

            using (MemoryStream fs = new MemoryStream(midiData))
            {
                var midi = Melanchall.DryWetMidi.Core.MidiFile.Read(fs);

                foreach (var chunk in midi.GetTrackChunks())
                {
                    foreach (var ev in chunk.Events)
                    {
                        if (ev.EventType == MidiEventType.SequenceTrackName && ev is Melanchall.DryWetMidi.Core.SequenceTrackNameEvent nameEv)
                        {
                            var instrument = ToInstrument(nameEv.Text);

                            if (instrument != Instrument.None)
                            {
                                tracks[(int)instrument] = chunk;
                                break;
                            }
                        }
                    }
                }
                foreach (var track in tracks)
                {
                    if (track == null)
                    {
                        continue;
                    }
                    foreach (var ev in track.Events)
                    {
                        if ((ev.EventType == MidiEventType.NoteOn || ev.EventType == MidiEventType.NoteOff) && ev is Melanchall.DryWetMidi.Core.NoteEvent noteEv)
                        {

                            var noteDiff = SelectNoteDifficulty(noteEv.NoteNumber);

                            if (noteDiff == null)
                                continue;

                            var val = (int)noteDiff.Value;

                            difficultyLoaded[val] = true;
                        }
                    }
                }
            }
        }

        // [Benchmark]
        // public void NoFrillSMF()
        // {
        //     MidiFile reader = new MidiFile();
        //     using (MemoryStream fs = new MemoryStream(midiData))
        //     {
        //         reader.ReadData(fs);
        //     }
        //     reader.Parse();
        // }

        // [Benchmark]
        // public void NAudioMidi()
        // {
        //     using (MemoryStream fs = new MemoryStream(midiData))
        //     {
        //         var mf = new NAudio.Midi.MidiFile(fs, false);
        //     }
        // }

        // [Benchmark]
        // public void DryWetMidi()
        // {
        //     using (MemoryStream fs = new MemoryStream(midiData))
        //     {
        //         Melanchall.DryWetMidi.Core.MidiFile.Read(fs);
        //     }
        // }
    }
}