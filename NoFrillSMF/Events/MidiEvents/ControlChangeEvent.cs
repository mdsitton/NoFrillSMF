using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ControlChangeEvent : BaseMidiEvent
    {
        public byte controller;
        public byte controlValue;

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);

            controller = data.ReadByte(ref offset);
            controlValue = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, controller);
            data.WriteByte(ref offset, controlValue);
        }
    }
}