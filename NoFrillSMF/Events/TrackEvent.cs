using System;
using System.IO;
using System.Runtime.CompilerServices;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events
{

    public enum EventType : byte
    {
        SequenceNumber = 0x00,
        Text,
        Copyright,
        TrackName,
        InstrumentName,
        Lyrics,
        Marker,
        CuePoint,

        // RP-019 - SMF Device Name and Program Name Meta Events
        ProgramName,
        DeviceName,

        // The midi spec says the following text events exist and act the same as meta_Text.
        TextReserved3,
        TextReserved4,
        TextReserved5,
        TextReserved6,
        TextReserved7,
        TextReserved8, // 0x0F

        MIDIChannelPrefix = 0x20,
        MIDIPort = 0x21, // obsolete no longer used.
        EndOfTrack = 0x2F,
        Tempo = 0x51,
        SMPTEOffset = 0x54,
        TimeSignature = 0x58,
        KeySignature = 0x59,
        XMFPatchType = 0x60, // For completeness probably wont show up in midi
        SequencerSpecific = 0x7F,

        NoteOff = 0x80,
        NoteOn = 0x90,
        KeyPressure = 0xA0,
        ControlChange = 0xB0,
        ProgramChange = 0xC0,
        ChannelPressure = 0xD0,
        PitchBend = 0xE0,

        MetaEvent = 0xFF,
        SysexEventStart = 0xF0,
        SysexEventEscape = 0xF7,
    };

    public abstract class TrackEvent : IPoolable//, IDisposable
    {
        public int EventID;
        public UInt32 DeltaTick;
        public UInt64 TickTime = 0;
        public byte Status;
        public EventType eventType;
        public UInt32 Size;
        public TrackEvent Previous;

        public abstract void Parse(byte[] data, ref int offset, TrackParseState state);
        public abstract void Compose(byte[] data, ref int offset);

        private static ObjectPool<MidiEvents.NoteOnEvent> noteOnEventPool = new ObjectPool<MidiEvents.NoteOnEvent>(() => new MidiEvents.NoteOnEvent());
        private static ObjectPool<MidiEvents.NoteOffEvent> noteOffEventPool = new ObjectPool<MidiEvents.NoteOffEvent>(() => new MidiEvents.NoteOffEvent());
        private static ObjectPool<MidiEvents.ControlChangeEvent> controlChangeEventPool = new ObjectPool<MidiEvents.ControlChangeEvent>(() => new MidiEvents.ControlChangeEvent());
        private static ObjectPool<MidiEvents.ProgramChangeEvent> programChangeEventPool = new ObjectPool<MidiEvents.ProgramChangeEvent>(() => new MidiEvents.ProgramChangeEvent());
        private static ObjectPool<MidiEvents.ChannelPressureEvent> channelPressureEventPool = new ObjectPool<MidiEvents.ChannelPressureEvent>(() => new MidiEvents.ChannelPressureEvent());
        private static ObjectPool<MidiEvents.PitchBendEvent> pitchBendEventPool = new ObjectPool<MidiEvents.PitchBendEvent>(() => new MidiEvents.PitchBendEvent());
        private static ObjectPool<MetaEvents.SequenceNumberEvent> sequenceNumberEventPool = new ObjectPool<MetaEvents.SequenceNumberEvent>(() => new MetaEvents.SequenceNumberEvent());
        private static ObjectPool<MetaEvents.TextEvent> textEventPool = new ObjectPool<MetaEvents.TextEvent>(() => new MetaEvents.TextEvent());
        private static ObjectPool<MetaEvents.TrackNameEvent> trackNameEventPool = new ObjectPool<MetaEvents.TrackNameEvent>(() => new MetaEvents.TrackNameEvent());
        private static ObjectPool<MetaEvents.TempoEvent> tempoEventPool = new ObjectPool<MetaEvents.TempoEvent>(() => new MetaEvents.TempoEvent());
        private static ObjectPool<MetaEvents.TimeSignatureEvent> timeSignatureEventPool = new ObjectPool<MetaEvents.TimeSignatureEvent>(() => new MetaEvents.TimeSignatureEvent());
        private static ObjectPool<MetaEvents.EndOfTrackEvent> endOfTrackEventPool = new ObjectPool<MetaEvents.EndOfTrackEvent>(() => new MetaEvents.EndOfTrackEvent());
        private static ObjectPool<MetaEvents.UnsupportedEvent> unsupportedEventPool = new ObjectPool<MetaEvents.UnsupportedEvent>(() => new MetaEvents.UnsupportedEvent());

        public static MidiEvents.NoteOnEvent NoteOnEvent = noteOnEventPool.Request();
        public static MidiEvents.NoteOffEvent NoteOffEvent = noteOffEventPool.Request();
        public static MidiEvents.ControlChangeEvent ControlChangeEvent = controlChangeEventPool.Request();
        public static MidiEvents.ProgramChangeEvent ProgramChangeEvent = programChangeEventPool.Request();
        public static MidiEvents.ChannelPressureEvent ChannelPressureEvent = channelPressureEventPool.Request();
        public static MidiEvents.PitchBendEvent PitchBendEvent = pitchBendEventPool.Request();
        public static MetaEvents.SequenceNumberEvent SequenceNumberEvent = sequenceNumberEventPool.Request();
        public static MetaEvents.TextEvent TextEvent = textEventPool.Request();
        public static MetaEvents.TrackNameEvent TrackNameEvent = trackNameEventPool.Request();
        public static MetaEvents.TempoEvent TempoEvent = tempoEventPool.Request();
        public static MetaEvents.TimeSignatureEvent TimeSignatureEvent = timeSignatureEventPool.Request();
        public static MetaEvents.EndOfTrackEvent EndOfTrackEvent = endOfTrackEventPool.Request();
        public static MetaEvents.UnsupportedEvent UnsupportedEvent = unsupportedEventPool.Request();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TrackEvent GetStaticEvents(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.NoteOn:
                    return TrackEvent.noteOnEventPool.Request();
                case EventType.NoteOff:
                    return TrackEvent.noteOffEventPool.Request();
                case EventType.ControlChange:
                    return TrackEvent.controlChangeEventPool.Request();
                case EventType.ProgramChange:
                    return TrackEvent.programChangeEventPool.Request();
                case EventType.ChannelPressure:
                    return TrackEvent.channelPressureEventPool.Request();
                case EventType.PitchBend:
                    return TrackEvent.pitchBendEventPool.Request();
                case EventType.SequenceNumber:
                    return TrackEvent.sequenceNumberEventPool.Request();
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
                    return TrackEvent.textEventPool.Request();
                case EventType.TrackName:
                    return TrackEvent.trackNameEventPool.Request();
                case EventType.Tempo:
                    return TrackEvent.tempoEventPool.Request();
                case EventType.TimeSignature:
                    return TrackEvent.timeSignatureEventPool.Request();
                case EventType.EndOfTrack:
                    return TrackEvent.endOfTrackEventPool.Request();
                default:
                    return TrackEvent.unsupportedEventPool.Request();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReturnStaticEvents(TrackEvent trkEvent)
        {
            switch (trkEvent)
            {
                case MidiEvents.NoteOnEvent evnt:
                    TrackEvent.noteOnEventPool.Return(evnt);
                    break;
                case MidiEvents.NoteOffEvent evnt:
                    TrackEvent.noteOffEventPool.Return(evnt);
                    break;
                case MidiEvents.ControlChangeEvent evnt:
                    TrackEvent.controlChangeEventPool.Return(evnt);
                    break;
                case MidiEvents.ProgramChangeEvent evnt:
                    TrackEvent.programChangeEventPool.Return(evnt);
                    break;
                case MidiEvents.ChannelPressureEvent evnt:
                    TrackEvent.channelPressureEventPool.Return(evnt);
                    break;
                case MidiEvents.PitchBendEvent evnt:
                    TrackEvent.pitchBendEventPool.Return(evnt);
                    break;
                case MetaEvents.SequenceNumberEvent evnt:
                    TrackEvent.sequenceNumberEventPool.Return(evnt);
                    break;
                case MetaEvents.TrackNameEvent evnt:
                    TrackEvent.trackNameEventPool.Return(evnt);
                    break;
                case MetaEvents.TextEvent evnt:
                    TrackEvent.textEventPool.Return(evnt);
                    break;
                case MetaEvents.TempoEvent evnt:
                    TrackEvent.tempoEventPool.Return(evnt);
                    break;
                case MetaEvents.TimeSignatureEvent evnt:
                    TrackEvent.timeSignatureEventPool.Return(evnt);
                    break;
                case MetaEvents.EndOfTrackEvent evnt:
                    TrackEvent.endOfTrackEventPool.Return(evnt);
                    break;
                case MetaEvents.UnsupportedEvent evnt:
                    TrackEvent.unsupportedEventPool.Return(evnt);
                    break;
            }
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
                    BaseMetaEvent.ParseFast(data, ref offset);
                    break;
                default:
                    BaseMetaEvent.ParseFast(data, ref offset);
                    break;

            }
        }

        public virtual void Clear()
        {
            EventID = 0;
            DeltaTick = 0;
            TickTime = 0;
            Status = 0;
            eventType = default;
            Size = 0;
            Previous = null;
        }
    }
}