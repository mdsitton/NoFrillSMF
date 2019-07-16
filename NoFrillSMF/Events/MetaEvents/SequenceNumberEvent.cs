using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MetaEvents
{
    public class SequenceNumberEvent : BaseMetaEvent
    {
        public UInt16 SequenceNumber;

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
            SequenceNumber = data.ReadUInt16BE(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteUInt16BE(ref offset, SequenceNumber);
        }
    }
}