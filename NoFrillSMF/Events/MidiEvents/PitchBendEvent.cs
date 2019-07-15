using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class PitchBendEvent : BaseMidiEvent
    {
        public byte pitchLow;
        public byte pitchHigh;

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);

            pitchLow = data.ReadByte(ref offset);
            pitchHigh = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, pitchLow);
            data.WriteByte(ref offset, pitchHigh);
        }
    }
}