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

    public abstract class TrackEvent
    {
        public int EventID;
        public UInt32 DeltaTick;
        public byte Status;
        public UInt32 Size;
        public UInt32 TotalSize;
        public TrackEvent Previous;

        public abstract void Parse(byte[] data, ref int offset);
        public abstract void Compose(byte[] data, ref int offset);
    }
}