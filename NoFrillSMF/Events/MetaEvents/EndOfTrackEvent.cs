using System;
using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class EndOfTrackEvent : BaseTrackEvent
    {

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            this.ParseStatus(data, ref offset, state);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            this.ComposeStatus(data, ref offset);
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