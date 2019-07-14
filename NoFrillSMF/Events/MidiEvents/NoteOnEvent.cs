using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class NoteOnEvent : BaseMidiEvent
    {
        public byte note { get; internal set; }
        public byte velocity { get; internal set; }
        public NoteOffEvent NoteOff { get; internal set; }

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

        public NoteOffEvent ToOffEvent()
        {
            var off = new NoteOffEvent();
            off.note = note;
            off.velocity = velocity;
            off.Status = Status;
            off.Size = Size;
            off.DeltaTick = DeltaTick;
            off.Message = MidiChannelMessage.NoteOff;
            off.Channel = Channel;
            off.Previous = Previous;
            return off;
        }
    }
}