using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TimeSignatureEvent : BaseTrackEvent
    {
        public byte Numerator;
        public byte Denominator;
        public byte MidiClocksPerMet;
        public byte ThirtySecondsPerQuarterNote;

        public override void Parse(byte[] data, ref int offset, TrackParseState state)
        {
            this.ParseStatus(data, ref offset, state);
            Numerator = data.ReadByte(ref offset);
            Denominator = data.ReadByte(ref offset);
            MidiClocksPerMet = data.ReadByte(ref offset);
            ThirtySecondsPerQuarterNote = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            this.ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, Numerator);
            data.WriteByte(ref offset, Denominator);
            data.WriteByte(ref offset, MidiClocksPerMet);
            data.WriteByte(ref offset, ThirtySecondsPerQuarterNote);
        }

        public override void Clear()
        {
            EventID = 0;
            DeltaTick = 0;
            TickTime = 0;
            Status = 0;
            eventType = default;
            Size = 0;
            Previous = null;
        }
    }
}