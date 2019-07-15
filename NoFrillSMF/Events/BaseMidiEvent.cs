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

    public abstract class BaseMidiEvent : TrackEvent
    {

        public MidiChannelMessage Message;
        public byte Channel;

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
    }
}