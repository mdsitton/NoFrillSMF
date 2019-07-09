using System.IO;

namespace NoFrillSMF.Events
{

    enum EventStatus : byte
    {
        MetaEvent = 0xFF,
        SysexEventStart = 0xF0,
        SysexEventEscape = 0xF7,
    };

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

    public interface IEvent
    {
        byte Status { get; }
        byte Length { get; }

        void Read(Stream data, byte readAmount);
        void Write(Stream data);

        void Parse();
        void Compose();
    }

}