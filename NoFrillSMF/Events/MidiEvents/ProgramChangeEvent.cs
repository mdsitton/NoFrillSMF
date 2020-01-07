using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class ProgramChangeEvent : BaseMidiEvent
    {
        public byte Program;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);
            Program = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, Program);
        }

        public override void Clear()
        {
            Program = 0;
            base.Clear();
        }
        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 1;
        }
    }
}