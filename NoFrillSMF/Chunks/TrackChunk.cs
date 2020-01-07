using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NoFrill.Common;
using NoFrillSMF.Events;
using NoFrillSMF.Events.MidiEvents;
using System.Linq;

namespace NoFrillSMF.Chunks
{
    public class TrackChunk
    {
        public string TypeStr => "MTrk";

        public uint Length { get; private set; }

        protected byte[] chunkData;
        protected List<TrackEvent> events = new List<TrackEvent>();
        protected TrackParseState state = new TrackParseState();

        Stack<NoteOnEvent>[] notesActive;
        readonly bool noteEventMatching;


        public struct TrackEventFilter<TEvent> where TEvent : TrackEvent
        {
            public UInt32 Size;
            public UInt64 TickTimeStart;
            public UInt64 TickTimeEnd;
            public List<TEvent> EventTemplates;


            public TrackEventFilter(List<TEvent> eventTemplates = null, UInt32 size = UInt32.MaxValue, UInt64 tickTimeStart = 0, UInt64 tickTimeEnd = UInt32.MaxValue)
            {
                EventTemplates = eventTemplates == null ? new List<TEvent>(4) : eventTemplates;
                Size = size;
                TickTimeStart = tickTimeStart;
                TickTimeEnd = tickTimeEnd;
            }

            public void AddTemplate(TEvent eventTemplate)
            {
                EventTemplates.Add(eventTemplate);
            }

            public void ClearTemplates()
            {
                EventTemplates.Clear();
            }

        }

        public TrackChunk(bool noteEventMatching = true)
        {
            this.noteEventMatching = noteEventMatching;
            // Allocate a stack per note per channel, to handle so we can handle matching note on/off events with O(1) complexity.
            // This is important for handling extremely large midi files.
            if (noteEventMatching)
                notesActive = Enumerable.Repeat(new Stack<NoteOnEvent>(), 128 * 16).ToArray();
        }

        public void Read(Stream data, uint chunkLength)
        {
            Length = chunkLength;
            chunkData = new byte[chunkLength];
            data.Read(chunkData, 0, (int)chunkLength);

        }

        public void Write(Stream data)
        {
            data.Write(chunkData, 0, (int)Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventType? ProcessEvents(byte[] data, ref int offset, ref Chunks.TrackParseState state)
        {
            EventType? evType = null;
            switch (state.status)
            {
                case (byte)EventType.MetaEvent:
                    offset++; // Seek over status
                    evType = (EventType)data.ReadByte(offset);
                    break;
                case (byte)EventType.SysexEventStart:
                case (byte)EventType.SysexEventEscape:
                    offset++; // Seek over status
                    offset += (int)data.ReadVlv(ref offset);
                    break;
                default:

                    if ((state.status & 0xF0) >= 0x80)
                    {
                        offset++; // Skip status byte
                    }
                    else
                    {

                        if (!(state.eventElement is BaseMidiEvent))
                        {
                            Console.WriteLine("Warning: Incorrect running status found, assuming last midi event status");
                        }

                        state.status = state.prevMidiStatus;
                    }


                    evType = (EventType)(state.status & 0xF0);
                    break;
            }
            return evType;
        }

        public static uint CalcIndex(byte note, byte channel) => ((uint)channel << 7) | note; // equivelent to channel * 128 + note

        public void MatchNoteEvents()
        {
            // Handle note on/off event matching, as well as converting vel 0 noteon's to note off events.
            if (state.eventElement is NoteOnEvent noteOn)
            {
                uint key = CalcIndex(noteOn.Note, noteOn.Channel);
                if (noteOn.Velocity == 0 && notesActive[key].Count > 0)
                {
                    NoteOffEvent off = noteOn.ToOffEvent();
                    NoteOnEvent on = notesActive[key].Pop();
                    on.NoteOff = off;
                    state.eventElement = off;
                }
                else
                {
                    notesActive[key].Push(noteOn);
                }
            }
            else if (state.eventElement is NoteOffEvent noteOff)
            {
                uint key = CalcIndex(noteOff.Note, noteOff.Channel);
                NoteOnEvent on = notesActive[key].Pop();
                on.NoteOff = noteOff;
            }
        }

        public void Parse()
        {
            events.Clear();
            while (state.trackDataPosition < Length)
            {
                state.deltaTicks = chunkData.ReadVlv(ref state.trackDataPosition);
                state.tickTime += state.deltaTicks;

                if (state.eventElement is BaseMidiEvent)
                {
                    state.prevMidiStatus = state.status;
                }

                state.status = chunkData.ReadByte(state.trackDataPosition);

                EventType? type = ProcessEvents(chunkData, ref state.trackDataPosition, ref state);

                if (type != null)
                {
                    TrackEvent ev = EventUtils.MidiEventFactory(type.Value);
                    ev.Previous = state.eventElement;
                    state.eventElement = ev;
                    state.eventElement.DeltaTick = state.deltaTicks;
                    state.eventElement.EventID = events.Count;
                    state.eventElement.Parse(chunkData, ref state.trackDataPosition, state);

                    if (noteEventMatching)
                        MatchNoteEvents();

                    events.Add(state.eventElement);
                }

            }
        }

        public IEnumerable<TEvent> ParseEvents<TEvent>(TrackEventFilter<TEvent> typeFilter) where TEvent : TrackEvent
        {
            // TODO - Implement FastSeek Method for all Event types for use by this incremental parser.
            // TODO - Implement state cloning for mid-parse forward searching.
            // TODO - Move parse pos into TrackParseState that way cloned parsers can easily move forwards indepenantly from the current one.
            // TODO - Implement option for simple quick event Find/FindAny/FindFirst, or correct state-full incremental parsing.
            // TODO - Implement sysex events, escape sequence metaevents, and continous sysex events
            // TODO - Implement Sequence Name text events.
            // TODO - Implement MIDI Channel Prefix support
            // TODO - Implement tempo map handling, and time conversion.
            //  If we do tempo map we can either store it statically, generated once globally.
            //  We can also dynamically generate it along with parsing whichever event track is being enumerated through.
            //      This would involve holding extra state data for the parallel finding of the next event of that type.
            // TODO - Investigate exposing state structs to the api, and single-run parsing functions that return a single event based on the state, and filters given.
            // NoteOnEvent ev = new NoteOnEvent();
            TrackParseState localState = new TrackParseState();
            while (localState.trackDataPosition < Length)
            {

                localState.deltaTicks = chunkData.ReadVlv(ref localState.trackDataPosition);
                localState.tickTime += localState.deltaTicks;

                if (localState.eventElement is BaseMidiEvent)
                {
                    localState.prevMidiStatus = localState.status;
                }

                localState.status = chunkData.ReadByte(localState.trackDataPosition);

                EventType? typeFound = ProcessEvents(chunkData, ref localState.trackDataPosition, ref localState);

                if (typeFound != null)
                {
                    bool found = false;
                    foreach (var t in typeFilter.EventTemplates)
                    {
                        if (t.eventType == typeFound.Value)
                        {
                            TEvent ev = TrackEvent.GetStaticEvents(typeFound.Value) as TEvent;

                            ev.Parse(chunkData, ref localState.trackDataPosition, localState);
                            ev.TickTime = localState.tickTime;
                            localState.eventElement = ev;
                            localState.eventElement.DeltaTick = localState.deltaTicks;
                            localState.eventElement.EventID = localState.eventCount++;

                            yield return ev;
                            TrackEvent.ReturnStaticEvents(ev);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        TrackEvent.FastParse(typeFound.Value, chunkData, ref localState.trackDataPosition);
                        localState.eventCount++;
                    }
                }
            }
        }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
