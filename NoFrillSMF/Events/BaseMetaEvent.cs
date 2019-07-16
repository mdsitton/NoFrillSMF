using System.Diagnostics;
using System;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public abstract class BaseMetaEvent : TrackEvent
    {
        public byte MetaType;

        protected void ParseStatus(byte[] data, ref int offset)
        {
            Status = data.ReadByte(ref offset);
            MetaType = data.ReadByte(ref offset);
            Debug.Assert(Status == 0xFF);
            Size = data.ReadVlv(ref offset);
        }

        protected void ComposeStatus(byte[] data, ref int offset)
        {
            data.WriteByte(ref offset, Status);
            data.WriteByte(ref offset, MetaType);
            data.WriteVlv(ref offset, Size);
        }
    }
}