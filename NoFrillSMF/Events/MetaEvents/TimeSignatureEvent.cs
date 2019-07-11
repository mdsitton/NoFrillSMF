﻿using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MetaEvents
{
    public class TimeSignatureEvent : BaseMetaEvent
    {
        public byte Numerator { get; private set; }
        public byte Denominator { get; private set; }
        public byte MidiClocksPerMet { get; private set; }
        public byte ThirtySecondsPerQuarterNote { get; private set; }

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
            Numerator = data.ReadByte(ref offset);
            Denominator = data.ReadByte(ref offset);
            MidiClocksPerMet = data.ReadByte(ref offset);
            ThirtySecondsPerQuarterNote = data.ReadByte(ref offset);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteByte(ref offset, Numerator);
            data.WriteByte(ref offset, Denominator);
            data.WriteByte(ref offset, MidiClocksPerMet);
            data.WriteByte(ref offset, ThirtySecondsPerQuarterNote);
        }
    }
}