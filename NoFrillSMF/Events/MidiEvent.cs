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

        public void Parse(byte[] data, ref int offset)
        {
            // Check if we should use the running status.
            if ((Status & 0xF0) >= 0x80)
            {
                data.ReadByte(ref offset);
            }
            else
            {
                MidiEvent prev = Previous as MidiEvent;

                if (prev == null)
                {
                    Console.WriteLine("Warning: Incorrect running status found, assuming last midi event status");
                    prev = EventUtils.FindLast<MidiEvent>(this);
                }

                Status = prev.Status;
            }
            var message = Status & 0xF0;
            var channel = Status & 0xF;
            var note = data.ReadByte(ref offset);
            var vel = data.ReadByte(ref offset);
            //Console.WriteLine($"{note} {vel}");
        }

        public void Compose(byte[] data, ref int offset)
        {
            throw new System.NotImplementedException();
        }
    }
}