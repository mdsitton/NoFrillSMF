using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ControlChangeEvent : MidiChannelEvent
    {
        public byte Controller;
        public byte ControlValue;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ChannelEventParsing.ParseStatus(this, data, ref offset, state);

            Controller = data.ReadByte(ref offset);
            ControlValue = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ChannelEventParsing.ComposeStatus(this, data, ref offset);
            data.WriteByte(ref offset, Controller);
            data.WriteByte(ref offset, ControlValue);
        }

        public override void Clear()
        {
            Controller = 0;
            ControlValue = 0;
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