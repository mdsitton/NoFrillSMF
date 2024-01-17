using System;
using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class SequenceNumberEvent : BaseTrackEvent
    {
        public UInt16 SequenceNumber;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            this.ParseStatus(data, ref offset, state);
            SequenceNumber = data.ReadUInt16BE(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            this.ComposeStatus(data, ref offset);
            data.WriteUInt16BE(ref offset, SequenceNumber);
        }

        public override void Clear()
        {
            EventID = 0;
            DeltaTick = 0;
            TickTime = 0;
            Status = 0;
            eventType = default;
            Size = 0;
            Previous = null;
        }
    }
}