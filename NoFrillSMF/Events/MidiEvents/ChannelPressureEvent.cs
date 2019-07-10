using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ChannelPressureEvent : BaseMidiEvent
    {
        public byte pressure { get; private set; }

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);

            pressure = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, pressure);
        }
    }
}