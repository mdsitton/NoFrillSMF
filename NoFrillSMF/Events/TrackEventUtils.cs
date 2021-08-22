using System;
using System.IO;
using System.Runtime.CompilerServices;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events
{

    public static class TrackEventUtils
    {

        public static ObjectPool eventPool = new ObjectPool();

        static TrackEventUtils()
        {
            DefinePooledEventTypes();
        }

        public static void DefinePooledEventTypes()
        {
            eventPool.AddType<MidiEvents.NoteEvent>();
            eventPool.AddType<MidiEvents.ControlChangeEvent>();
            eventPool.AddType<MidiEvents.ProgramChangeEvent>();
            eventPool.AddType<MidiEvents.ChannelPressureEvent>();
            eventPool.AddType<MidiEvents.PitchBendEvent>();
            eventPool.AddType<MetaEvents.SequenceNumberEvent>();
            eventPool.AddType<MetaEvents.TextEvent>();
            eventPool.AddType<MetaEvents.TrackNameEvent>();
            eventPool.AddType<MetaEvents.TempoEvent>();
            eventPool.AddType<MetaEvents.TimeSignatureEvent>();
            eventPool.AddType<MetaEvents.EndOfTrackEvent>();
            eventPool.AddType<MetaEvents.UnsupportedEvent>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastParse(EventType eventType, byte[] data, ref int offset)
        {
            switch (eventType)
            {
                case EventType.ChannelPressure:
                    offset += 1;
                    break;
                case EventType.NoteOn:
                case EventType.NoteOff:
                case EventType.ControlChange:
                case EventType.ProgramChange:
                case EventType.PitchBend:
                    offset += 2;
                    break;
                default:
                    MetaEventParsing.ParseFast(data, ref offset);
                    break;

            }
        }
    }
}