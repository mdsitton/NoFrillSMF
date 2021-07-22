using System.Diagnostics;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MidiEvents
{
    public class NoteEvent : MidiChannelEvent
    {
        public byte Note;
        public byte Velocity;
        public NoteEvent NoteOff;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            ChannelEventParsing.ParseStatus(this, data, ref offset, state);

            Note = data.ReadByte(ref offset);
            Velocity = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ChannelEventParsing.ComposeStatus(this, data, ref offset);
            data.WriteByte(ref offset, Note);
            data.WriteByte(ref offset, Velocity);
        }

        public override void Clear()
        {
            Note = 0;
            Velocity = 0;
            Channel = 0;
            EventID = 0;
            DeltaTick = 0;
            TickTime = 0;
            Status = 0;
            eventType = default;
            Size = 0;
            Previous = null;
            NoteOff = null;
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 2;
        }
    }
}