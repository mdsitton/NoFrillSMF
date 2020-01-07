using System.Diagnostics;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class NoteEvent : BaseMidiEvent
    {
        public byte Note;
        public byte Velocity;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ParseStatus(data, ref offset, state);

            Note = data.ReadByte(ref offset);
            Velocity = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, Note);
            data.WriteByte(ref offset, Velocity);
        }

        public override void Clear()
        {
            Note = 0;
            Velocity = 0;
            base.Clear();
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 2;
        }
    }
}