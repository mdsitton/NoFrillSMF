using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class NoteOffEvent : BaseMidiEvent
    {
        public byte note { get; private set; }
        public byte velocity { get; private set; }

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);

            note = data.ReadByte(ref offset);
            velocity = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, note);
            data.WriteByte(ref offset, velocity);
        }
    }
}