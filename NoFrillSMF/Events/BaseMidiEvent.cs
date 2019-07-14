using System;
using System.IO;
using NoFrill.Common;

namespace NoFrillSMF.Events
{

    public enum MidiChannelMessage : byte
    {
        NoteOff = 0x80,
        NoteOn = 0x90,
        KeyPressure = 0xA0,
        ControlChange = 0xB0,
        ProgramChange = 0xC0,
        ChannelPressure = 0xD0,
        PitchBend = 0xE0,
    };

    public abstract class BaseMidiEvent : IEvent
    {

        public int EventID { get; set; }
        public byte Status { get; internal set; }

        public UInt32 Size { get; internal set; }
        public UInt32 TotalSize { get; internal set; } // TODO - Make function to figure out length of varlen

        public UInt32 DeltaTick { get; set; }

        public IEvent Previous { get; set; }

        public MidiChannelMessage Message { get; internal set; }
        public byte Channel { get; internal set; }

        protected void ParseStatus(byte[] data, ref int offset)
        {

            Message = (MidiChannelMessage)(Status & 0xF0);
            Channel = (byte)(Status & 0xF);
        }

        protected void ComposeStatus(byte[] data, ref int offset)
        {
            if (Previous.Status != Status)
            {
                data.WriteByte(ref offset, Status); // TODO - merge Message/Channel instead.
            }
        }

        public abstract void Compose(byte[] data, ref int offset);

        public abstract void Parse(byte[] data, ref int offset);
    }
}