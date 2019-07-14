using System.Diagnostics;
using System;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public enum MidiMetaEvent : byte
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
    };

    public abstract class BaseMetaEvent : IEvent
    {
        public byte Status { get; set; }
        public byte MetaType { get; private set; }

        public UInt32 Size { get; set; }
        public UInt32 TotalSize { get; private set; } // TODO - Make function to figure out length of varlen

        public UInt32 DeltaTick { get; set; }

        public IEvent Previous { get; set; }

        protected void ParseStatus(byte[] data, ref int offset)
        {
            Status = data.ReadByte(ref offset);
            MetaType = data.ReadByte(ref offset);
            Debug.Assert(Status == 0xFF);
            Size = data.ReadVlv(ref offset);
        }

        protected void ComposeStatus(byte[] data, ref int offset)
        {
            data.WriteByte(ref offset, Status);
            data.WriteByte(ref offset, MetaType);
            data.WriteVlv(ref offset, Size);
        }

        public abstract void Compose(byte[] data, ref int offset);
        public abstract void Parse(byte[] data, ref int offset);
    }
}