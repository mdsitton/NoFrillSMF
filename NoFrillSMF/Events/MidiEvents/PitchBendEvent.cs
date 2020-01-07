using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class PitchBendEvent : BaseMidiEvent
    {
        public byte PitchLow;
        public byte PitchHigh;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);

            PitchLow = data.ReadByte(ref offset);
            PitchHigh = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, PitchLow);
            data.WriteByte(ref offset, PitchHigh);
        }

        public override void Clear()
        {
            PitchLow = 0;
            PitchHigh = 0;
            base.Clear();
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 2;
        }
    }
}