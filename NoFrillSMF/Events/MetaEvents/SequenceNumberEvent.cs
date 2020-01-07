using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class SequenceNumberEvent : BaseMetaEvent
    {
        public UInt16 SequenceNumber;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);
            SequenceNumber = data.ReadUInt16BE(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteUInt16BE(ref offset, SequenceNumber);
        }
    }
}