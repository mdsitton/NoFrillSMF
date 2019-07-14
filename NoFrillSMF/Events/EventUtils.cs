using System.Diagnostics;
using System;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public static class EventUtils
    {

        public static BaseMidiEvent MidiEventFactory(MidiChannelMessage message)
        {
            switch (message)
            {
                case MidiChannelMessage.NoteOn:
                    return new MidiEvents.NoteOnEvent();
                case MidiChannelMessage.NoteOff:
                    return new MidiEvents.NoteOffEvent();
                case MidiChannelMessage.ControlChange:
                    return new MidiEvents.ControlChangeEvent();
                case MidiChannelMessage.ProgramChange:
                    return new MidiEvents.ProgramChangeEvent();
                case MidiChannelMessage.ChannelPressure:
                    return new MidiEvents.ChannelPressureEvent();
                case MidiChannelMessage.PitchBend:
                    return new MidiEvents.PitchBendEvent();
                default:
                    return null;
            }
        }

        public static IEvent MetaEventFactory(MidiMetaEvent metaType)
        {
            switch (metaType)
            {
                case MidiMetaEvent.SequenceNumber:
                    return new MetaEvents.SequenceNumberEvent();
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
                    return new MetaEvents.TextEvent();
                case MidiMetaEvent.Tempo:
                    return new MetaEvents.TempoEvent();
                case MidiMetaEvent.TimeSignature:
                    return new MetaEvents.TimeSignatureEvent();
                case MidiMetaEvent.EndOfTrack:
                    return new MetaEvents.EndOfTrackEvent();
                // These are mainly here to just represent them existing
                case MidiMetaEvent.MIDIChannelPrefix:
                case MidiMetaEvent.MIDIPort:  // obsolete no longer used.
                case MidiMetaEvent.SMPTEOffset: // Not currently implemented, maybe someday.
                case MidiMetaEvent.KeySignature: // Not very useful for us
                case MidiMetaEvent.XMFPatchType: // probably not used
                case MidiMetaEvent.SequencerSpecific:
                    return new MetaEvents.UnsupportedEvent();
                default:
                    throw new Exception("Unknown event type");
            }
        }

        public static T FindLast<T>(IEvent current) where T : class, IEvent
        {
            while (true)
            {
                current = current.Previous;
                if (current is T)
                    return current as T;
            }
        }

        public delegate bool SentinelCheck<T>(T val) where T : class, IEvent;

        public static T FindLast<T>(IEvent current, SentinelCheck<T> check) where T : class, IEvent
        {
            T val;
            while ((current = current.Previous) != null)
            {
                val = current as T;
                if (val != null && check(val))
                    return val;
            }
            return null;
        }
    }
}