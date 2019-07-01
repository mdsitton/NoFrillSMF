using System.IO;

namespace NoFrillSMF.Events
{

    enum EventStatus : byte
    {
        MetaEvent = 0xFF,
        SysexEventStart = 0xF0,
        SysexEventEscape = 0xF7,
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