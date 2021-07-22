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
                case EventType.NoteOn:
                case EventType.NoteOff:
                    MidiEvents.NoteEvent.ParseFast(data, ref offset);
                    break;
                case EventType.ControlChange:
                    MidiEvents.ControlChangeEvent.ParseFast(data, ref offset);
                    break;
                case EventType.ProgramChange:
                    MidiEvents.ProgramChangeEvent.ParseFast(data, ref offset);
                    break;
                case EventType.ChannelPressure:
                    MidiEvents.ChannelPressureEvent.ParseFast(data, ref offset);
                    break;
                case EventType.PitchBend:
                    MidiEvents.PitchBendEvent.ParseFast(data, ref offset);
                    break;
                case EventType.SysexEventStart:
                case EventType.SysexEventEscape:
                    break;
                case EventType.SequenceNumber:
                case EventType.Text:
                case EventType.Copyright:
                case EventType.InstrumentName:
                case EventType.Lyrics:
                case EventType.Marker:
                case EventType.CuePoint:
                case EventType.ProgramName:
                case EventType.DeviceName:
                case EventType.TextReserved3:
                case EventType.TextReserved4:
                case EventType.TextReserved5:
                case EventType.TextReserved6:
                case EventType.TextReserved7:
                case EventType.TextReserved8:
                case EventType.TrackName:
                case EventType.Tempo:
                case EventType.TimeSignature:
                case EventType.EndOfTrack:
                    MetaEventParsing.ParseFast(data, ref offset);
                    break;
                default:
                    MetaEventParsing.ParseFast(data, ref offset);
                    break;

            }
        }
    }
}