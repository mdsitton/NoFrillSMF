using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public static class EventUtils
    {

        public static IEventTypeProcessor EventProcessorFactory(byte status)
        {
            switch (status)
            {
                case (byte)EventStatus.MetaEvent:
                    return new MetaEventProcessor();
                case (byte)EventStatus.SysexEventStart:
                case (byte)EventStatus.SysexEventEscape:
                    return new SysexEventProcessor();
                default:
                    return new MidiEventProcessor();
            }
        }

        public static IEvent MetaEventFactor(MidiMetaEvent metaType)
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

        public static IEvent MidiEventFactory(MidiChannelMessage message)
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

        public static T FindLast<T>(IEvent current) where T : class, IEvent
        {
            while (true)
            {
                current = current.Previous;
                if (current is T)
                    return current as T;
            }
        }

    }
}