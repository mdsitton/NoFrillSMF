using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ProgramChangeEvent : BaseMidiEvent
    {
        public byte program;

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
            program = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, program);
        }
    }
}