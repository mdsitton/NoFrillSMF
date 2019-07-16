using System.Diagnostics;
using System;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public abstract class BaseMetaEvent : TrackEvent
    {

        protected void ParseStatus(byte[] data, ref int offset)
        {
            Status = data.ReadByte(ref offset);
            eventType = (EventType)data.ReadByte(ref offset);
            Debug.Assert(Status == 0xFF);
            Size = data.ReadVlv(ref offset);
        }

        protected void ComposeStatus(byte[] data, ref int offset)
        {
            data.WriteByte(ref offset, Status);
            data.WriteByte(ref offset, (byte)eventType);
            data.WriteVlv(ref offset, Size);
        }
    }
}