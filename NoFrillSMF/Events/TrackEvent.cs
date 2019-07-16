using System;
using System.IO;

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
    }
}