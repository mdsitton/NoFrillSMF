using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TextEvent : BaseMetaEvent
    {
        public string Text = "";

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);
            Text = data.ReadString(ref offset, (int)Size);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteString(ref offset, Text);
        }
    }

    public class TrackNameEvent : TextEvent
    {
    }

}