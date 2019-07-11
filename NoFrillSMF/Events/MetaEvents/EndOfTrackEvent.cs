using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MetaEvents
{
    public class EndOfTrackEvent : BaseMetaEvent
    {

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
        }
    }
}