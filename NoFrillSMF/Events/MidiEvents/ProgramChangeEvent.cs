using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ProgramChangeEvent : MidiChannelEvent
    {
        public byte Program;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ChannelEventParsing.ParseStatus(this, data, ref offset, state);
            Program = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ChannelEventParsing.ComposeStatus(this, data, ref offset);
            data.WriteByte(ref offset, Program);
        }

        public override void Clear()
        {
            Program = 0;
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
            offset += 1;
        }
    }
}