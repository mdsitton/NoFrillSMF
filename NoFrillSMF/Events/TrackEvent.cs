using System;
using System.IO;
using System.Runtime.CompilerServices;

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

    public abstract class TrackEvent
    {
        public int EventID;
        public UInt32 DeltaTick;
        public byte Status;
        public EventType eventType;
        public UInt32 Size;
        public UInt32 TotalSize;
        public TrackEvent Previous;

        public abstract void Parse(byte[] data, ref int offset);
        public abstract void Compose(byte[] data, ref int offset);

        public static MidiEvents.NoteOnEvent NoteOnEvent { get; private set; } = new MidiEvents.NoteOnEvent();
        public static MidiEvents.NoteOffEvent NoteOffEvent { get; private set; } = new MidiEvents.NoteOffEvent();
        public static MidiEvents.ControlChangeEvent ControlChangeEvent { get; private set; } = new MidiEvents.ControlChangeEvent();
        public static MidiEvents.ProgramChangeEvent ProgramChangeEvent { get; private set; } = new MidiEvents.ProgramChangeEvent();
        public static MidiEvents.ChannelPressureEvent ChannelPressureEvent { get; private set; } = new MidiEvents.ChannelPressureEvent();
        public static MidiEvents.PitchBendEvent PitchBendEvent { get; private set; } = new MidiEvents.PitchBendEvent();
        public static MetaEvents.SequenceNumberEvent SequenceNumberEvent { get; private set; } = new MetaEvents.SequenceNumberEvent();
        public static MetaEvents.TextEvent TextEvent { get; private set; } = new MetaEvents.TextEvent();
        public static MetaEvents.TempoEvent TempoEvent { get; private set; } = new MetaEvents.TempoEvent();
        public static MetaEvents.TimeSignatureEvent TimeSignatureEvent { get; private set; } = new MetaEvents.TimeSignatureEvent();
        public static MetaEvents.EndOfTrackEvent EndOfTrackEvent { get; private set; } = new MetaEvents.EndOfTrackEvent();
        public static MetaEvents.UnsupportedEvent UnsupportedEvent { get; private set; } = new MetaEvents.UnsupportedEvent();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TrackEvent GetStaticEvents(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.NoteOn:
                    return TrackEvent.NoteOnEvent;
                case EventType.NoteOff:
                    return TrackEvent.NoteOffEvent;
                case EventType.ControlChange:
                    return TrackEvent.ControlChangeEvent;
                case EventType.ProgramChange:
                    return TrackEvent.ProgramChangeEvent;
                case EventType.ChannelPressure:
                    return TrackEvent.ChannelPressureEvent;
                case EventType.PitchBend:
                    return TrackEvent.PitchBendEvent;
                case EventType.SequenceNumber:
                    return TrackEvent.SequenceNumberEvent;
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
                    return TrackEvent.TextEvent;
                case EventType.Tempo:
                    return TrackEvent.TempoEvent;
                case EventType.TimeSignature:
                    return TrackEvent.TimeSignatureEvent;
                case EventType.EndOfTrack:
                    return TrackEvent.EndOfTrackEvent;
                default:
                    return TrackEvent.UnsupportedEvent;
            }
        }
    }
}