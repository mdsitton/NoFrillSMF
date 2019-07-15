using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TempoEvent : BaseMetaEvent
    {
        public UInt32 Tempo;

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
            Tempo = data.ReadUInt24BE(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteUInt24BE(ref offset, Tempo);
        }
    }
}