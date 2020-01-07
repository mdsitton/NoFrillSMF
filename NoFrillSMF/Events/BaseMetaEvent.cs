using System.Diagnostics;
using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events
{
    public abstract class BaseMetaEvent : TrackEvent
    {

        protected void ParseStatus(byte[] data, ref int offset, TrackParseState state)
        {
            Status = state.status;
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

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 1;
            uint size = data.ReadVlv(ref offset);
            offset += (int)size;
        }
    }
}