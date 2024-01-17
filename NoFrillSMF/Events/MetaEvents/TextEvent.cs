using System;
using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TextEvent : BaseTrackEvent
    {
        public string Text = "";

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            this.ParseStatus(data, ref offset, state);
            Text = data.ReadString(ref offset, (int)Size);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            this.ComposeStatus(data, ref offset);
            data.WriteString(ref offset, Text);
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

    // TODO - get rid of this event class and have EventType search methods
    public class TrackNameEvent : TextEvent
    {
    }

}