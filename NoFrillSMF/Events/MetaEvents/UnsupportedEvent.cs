using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class UnsupportedEvent : BaseTrackEvent
    {
        public byte[] dataBlock;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            this.ParseStatus(data, ref offset, state);
            if (Size > 0)
            {
                dataBlock = new byte[Size];
                data.ReadBytes(ref offset, dataBlock, Size);
            }

        }

        public override void Compose(byte[] data, ref int offset)
        {
            this.ComposeStatus(data, ref offset);
            data.WriteBytes(ref offset, dataBlock, Size);
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