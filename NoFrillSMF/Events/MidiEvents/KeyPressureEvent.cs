using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class KeyPressureEvent : BaseMidiEvent
    {
        public byte Note;
        public byte Pressure;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);
            Note = data.ReadByte(ref offset);
            Pressure = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, Note);
            data.WriteByte(ref offset, Pressure);
        }

        public override void Clear()
        {
            Note = 0;
            Pressure = 0;
            base.Clear();
        }
        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 2;
        }
    }
}