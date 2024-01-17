using System;
using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TempoEvent : BaseTrackEvent
    {
        public UInt32 Tempo;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            this.ParseStatus(data, ref offset, state);
            Tempo = data.ReadUInt24BE(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            this.ComposeStatus(data, ref offset);
            data.WriteUInt24BE(ref offset, Tempo);
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