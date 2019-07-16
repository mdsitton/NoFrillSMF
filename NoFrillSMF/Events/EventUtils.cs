using System.Diagnostics;
using System;
using NoFrill.Common;
using System.Runtime.CompilerServices;

namespace NoFrillSMF.Events
{
    public static class EventUtils
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TrackEvent MidiEventFactory(EventType message)
        {
            switch (message)
            {
                case EventType.NoteOn:
                    return new MidiEvents.NoteOnEvent();
                case EventType.NoteOff:
                    return new MidiEvents.NoteOffEvent();
                case EventType.ControlChange:
                    return new MidiEvents.ControlChangeEvent();
                case EventType.ProgramChange:
                    return new MidiEvents.ProgramChangeEvent();
                case EventType.ChannelPressure:
                    return new MidiEvents.ChannelPressureEvent();
                case EventType.PitchBend:
                    return new MidiEvents.PitchBendEvent();
                case EventType.SequenceNumber:
                    return new MetaEvents.SequenceNumberEvent();
                case EventType.Text:
                case EventType.Copyright:
                case EventType.InstrumentName:
                case EventType.Lyrics: // TODO - Implement ruby parser for lyrics
                case EventType.Marker:
                case EventType.CuePoint:
                // RP-019 - SMF Device Name and Program Name Meta Events
                case EventType.ProgramName:
                case EventType.DeviceName:
                // The midi spec says the following text events exist and act the same as meta_Text.
                case EventType.TextReserved3:
                case EventType.TextReserved4:
                case EventType.TextReserved5:
                case EventType.TextReserved6:
                case EventType.TextReserved7:
                case EventType.TextReserved8:
                case EventType.TrackName:
                    return new MetaEvents.TextEvent();
                case EventType.Tempo:
                    return new MetaEvents.TempoEvent();
                case EventType.TimeSignature:
                    return new MetaEvents.TimeSignatureEvent();
                case EventType.EndOfTrack:
                    return new MetaEvents.EndOfTrackEvent();
                // These are mainly here to just represent them existing
                case EventType.MIDIChannelPrefix:
                case EventType.MIDIPort:  // obsolete no longer used.
                case EventType.SMPTEOffset: // Not currently implemented, maybe someday.
                case EventType.KeySignature: // Not very useful for us
                case EventType.XMFPatchType: // probably not used
                case EventType.SequencerSpecific:
                    return new MetaEvents.UnsupportedEvent();
                default:
                    throw new Exception("Unknown event type");
            }
        }

        public static T FindLast<T>(TrackEvent current) where T : TrackEvent
        {
            while (true)
            {
                current = current.Previous;
                if (current is T)
                    return current as T;
            }
        }

        public delegate bool SentinelCheck<T>(T val) where T : TrackEvent;

        public static T FindLast<T>(TrackEvent current, SentinelCheck<T> check) where T : TrackEvent
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