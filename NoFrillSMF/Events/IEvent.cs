using System;
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
        UInt32 DeltaTick { get; set; }
        byte Status { get; }
        UInt32 Size { get; }
        UInt32 TotalSize { get; }

        IEvent Previous { get; set; }

        void Parse(byte[] data, ref int offset);
        void Compose(byte[] data, ref int offset);
    }
}