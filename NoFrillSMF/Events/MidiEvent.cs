using System;
using System.IO;
using NoFrill.Common;

namespace NoFrillSMF.Events
{

    enum MidiChannelMessage : byte
    {
        NoteOff = 0x80,
        NoteOn = 0x90,
        KeyPressure = 0xA0,
        ControlChange = 0xB0,
        ProgramChange = 0xC0,
        ChannelPressure = 0xD0,
        PitchBend = 0xE0,
    };

    public class MidiEvent : IEvent
    {
        public byte Status { get; set; }
        public byte MetaType { get; private set; }

        public int Length { get; private set; }
        public int TotalSize { get; private set; } // TODO - Make function to figure out length of varlen

        public uint DeltaTick { get; private set; }

        public IEvent Previous { get; set; }


        void ReadMidiEvent(byte[] data, ref int offset, MidiChannelMessage status)
        {
            switch (status)
            {
                case MidiChannelMessage.NoteOn:
                    data.ReadByte(ref offset);
                    data.ReadByte(ref offset);
                    break;
                case MidiChannelMessage.NoteOff:
                    data.ReadByte(ref offset);
                    data.ReadByte(ref offset);
                    break;
                case MidiChannelMessage.ControlChange:
                    data.ReadByte(ref offset);
                    data.ReadByte(ref offset);
                    break;
                case MidiChannelMessage.ProgramChange:
                    data.ReadByte(ref offset);
                    break;
                case MidiChannelMessage.ChannelPressure:
                    data.ReadByte(ref offset);
                    break;
                case MidiChannelMessage.PitchBend:
                    data.ReadByte(ref offset);
                    data.ReadByte(ref offset);
                    break;
                default:
                    return;

            }
        }

        public void Parse(byte[] data, ref int offset)
        {
            // Check if we should use the running status.
            if ((Status & 0xF0) >= 0x80)
            {
                data.ReadByte(ref offset);
            }
            else
            {
                if (!(Previous is MidiEvent prev))
                {
                    Console.WriteLine("Warning: Incorrect running status found, assuming last midi event status");
                    prev = EventUtils.FindLast<MidiEvent>(this);
                }

                Status = prev.Status;
            }

            MidiChannelMessage message = (MidiChannelMessage)(Status & 0xF0);
            byte channel = (byte)(Status & 0xF);
            ReadMidiEvent(data, ref offset, message);
            //Console.WriteLine($"{note} {vel}");
        }

        public void Compose(byte[] data, ref int offset)
        {
            throw new System.NotImplementedException();
        }
    }
}