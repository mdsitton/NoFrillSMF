using NoFrill.Common;

namespace NoFrillSMF.Events.MidiEvents
{
    public class NoteOnEvent : NoteEvent
    {
        public NoteOffEvent NoteOff;

        public NoteOffEvent ToOffEvent()
        {
            var off = new NoteOffEvent();
            off.note = note;
            off.velocity = velocity;
            off.Status = Status;
            off.Size = Size;
            off.DeltaTick = DeltaTick;
            off.eventType = EventType.NoteOff;
            off.Channel = Channel;
            off.Previous = Previous;
            return off;
        }
    }
}