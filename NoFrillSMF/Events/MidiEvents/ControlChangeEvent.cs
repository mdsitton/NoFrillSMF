using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ControlChangeEvent : BaseMidiEvent
    {
        public byte Controller;
        public byte ControlValue;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);

            Controller = data.ReadByte(ref offset);
            ControlValue = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, Controller);
            data.WriteByte(ref offset, ControlValue);
        }

        public override void Clear()
        {
            Controller = 0;
            ControlValue = 0;
            base.Clear();
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 2;
        }
    }
}