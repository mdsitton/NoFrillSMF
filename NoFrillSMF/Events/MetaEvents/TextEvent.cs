using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TextEvent : BaseMetaEvent
    {
        public string Text { get; private set; }

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
            Text = data.ReadString(ref offset, (int)Size);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteString(ref offset, Text);
        }
    }
}