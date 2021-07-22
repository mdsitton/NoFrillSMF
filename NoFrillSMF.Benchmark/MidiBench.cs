using System.IO;
using System;
using BenchmarkDotNet.Attributes;
// using Melanchall.DryWetMidi.Smf;
using NoFrillSMF;
using NAudio.Midi;
using System.Collections.Generic;

namespace NoFrillSMF.Benchmark
{
    [CoreJob]
    [MemoryDiagnoser]
    public class MidiBench
    {
        string fileName = "E:\\development\\NoFrillSMF\\NoFrillSMF.Tests\\midifiles\\rb4midi.mid";


        MidiFile reader = new MidiFile(false);

        byte[] midiData;

        static Events.MidiEvents.NoteEvent eventTemplate = new Events.MidiEvents.NoteEvent();
        static Events.MetaEvents.TrackNameEvent eventTemplateText = new Events.MetaEvents.TrackNameEvent();
        static List<Events.MidiEvents.NoteEvent> types = new List<Events.MidiEvents.NoteEvent> { eventTemplate };
        static List<Events.MetaEvents.TrackNameEvent> typesText = new List<Events.MetaEvents.TrackNameEvent> { eventTemplateText };
        static Chunks.TrackChunk.TrackEventFilter<Events.MidiEvents.NoteEvent> filterObj = new Chunks.TrackChunk.TrackEventFilter<Events.MidiEvents.NoteEvent>(eventTemplates: types);
        static Chunks.TrackChunk.TrackEventFilter<Events.MetaEvents.TrackNameEvent> filterObjText = new Chunks.TrackChunk.TrackEventFilter<Events.MetaEvents.TrackNameEvent>(eventTemplates: typesText);

        [GlobalSetup]
        public void Setup()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                reader.ReadData(fs);
            }
            midiData = File.ReadAllBytes(fileName);
            eventTemplate.eventType = Events.EventType.NoteOn;
            eventTemplateText.eventType = Events.EventType.TrackName;
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
            }
            return Instrument.None;
        }

        private static readonly int _numOfDifficulties = Enum.GetNames(typeof(Difficulty)).Length;
        private static readonly int _numOfInstruments = Enum.GetNames(typeof(Instrument)).Length;



        [Benchmark]
        public void NoFrillSMFEnum()
        {

            var tracks = new Chunks.TrackChunk[_numOfInstruments * _numOfDifficulties];
            bool[] difficultyLoaded = new bool[4];

            foreach (var track in reader.GetTrackChunks())
            {
                foreach (var ev in track.ParseEvents<Events.MetaEvents.TrackNameEvent>(filterObjText))
                {
                    var instrument = ToInstrument(ev.Text);

                    if (instrument != Instrument.None)
                    {
                        tracks[(int)instrument] = track;
                        break;
                    }
                }
            }
            foreach (var track in tracks)
            {
                if (track == null)
                {
                    continue;
                }

                // difficultyLoaded.Clear();

                foreach (var ev in track.ParseEvents<Events.MidiEvents.NoteEvent>(filterObj))
                {
                    var noteDiff = SelectNoteDifficulty(ev.Note);

                    if (noteDiff == null)
                        continue;

                    var val = (int)noteDiff.Value;

                    difficultyLoaded[val] = true;
                }
            }
            // return difficultyLoaded;
        }

        [Benchmark]
        public void NoFrillSMF()
        {
            MidiFile reader = new MidiFile();
            using (MemoryStream fs = new MemoryStream(midiData))
            {
                reader.ReadData(fs);
            }
            reader.Parse();
        }

        [Benchmark]
        public void NAudioMidi()
        {
            using (MemoryStream fs = new MemoryStream(midiData))
            {
                var mf = new NAudio.Midi.MidiFile(fs, false);
            }
        }

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