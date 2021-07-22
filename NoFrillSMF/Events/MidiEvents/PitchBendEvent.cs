using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class PitchBendEvent : MidiChannelEvent
    {
        public byte PitchLow;
        public byte PitchHigh;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ChannelEventParsing.ParseStatus(this, data, ref offset, state);

            PitchLow = data.ReadByte(ref offset);
            PitchHigh = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ChannelEventParsing.ComposeStatus(this, data, ref offset);
            data.WriteByte(ref offset, PitchLow);
            data.WriteByte(ref offset, PitchHigh);
        }

        public override void Clear()
        {
            PitchLow = 0;
            PitchHigh = 0;
            Channel = 0;
            EventID = 0;
            DeltaTick = 0;
            TickTime = 0;
            Status = 0;
            eventType = default;
            Size = 0;
            Previous = null;
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 2;
        }
    }
}