using System;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public class MetaEventProcessor : IEventTypeProcessor
    {

        public static IEvent MetaEventFactory(MidiMetaEvent metaType, UInt32 size)
        {
            switch (metaType)
            {
                case MidiMetaEvent.SequenceNumber:
                    return new MidiEvents.SequenceNumberEvent();
                case MidiMetaEvent.Text:
                case MidiMetaEvent.Copyright:
                case MidiMetaEvent.InstrumentName:
                case MidiMetaEvent.Lyrics: // TODO - Implement ruby parser for lyrics
                case MidiMetaEvent.Marker:
                case MidiMetaEvent.CuePoint:
                // RP-019 - SMF Device Name and Program Name Meta Events
                case MidiMetaEvent.ProgramName:
                case MidiMetaEvent.DeviceName:
                // The midi spec says the following text events exist and act the same as meta_Text.
                case MidiMetaEvent.TextReserved3:
                case MidiMetaEvent.TextReserved4:
                case MidiMetaEvent.TextReserved5:
                case MidiMetaEvent.TextReserved6:
                case MidiMetaEvent.TextReserved7:
                case MidiMetaEvent.TextReserved8:
                case MidiMetaEvent.TrackName:
                    return null;
                case MidiMetaEvent.MIDIChannelPrefix:
                    return null;
                case MidiMetaEvent.EndOfTrack:
                    return null;
                case MidiMetaEvent.Tempo:
                    return null;
                case MidiMetaEvent.TimeSignature:
                    return null;
                // These are mainly here to just represent them existing
                case MidiMetaEvent.MIDIPort:  // obsolete no longer used.
                case MidiMetaEvent.SMPTEOffset: // Not currently implemented, maybe someday.
                case MidiMetaEvent.KeySignature: // Not very useful for us
                case MidiMetaEvent.XMFPatchType: // probably not used
                case MidiMetaEvent.SequencerSpecific:
                    return null;
                default:
                    return null;
            }
        }

        public void Parse(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            MidiMetaEvent metaType = (MidiMetaEvent)data.ReadByte(ref offset);
            UInt32 size = data.ReadVlv(ref offset);

            state.eventElement = MetaEventFactory(metaType, size);
            if (state.eventElement != null)
                offset += (int)size;
        }

        public void Compose(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            throw new System.NotImplementedException();
        }
    }
}