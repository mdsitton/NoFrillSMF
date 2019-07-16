using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NoFrill.Common;
using NoFrillSMF.Events;
using NoFrillSMF.Events.MidiEvents;
using System.Linq;
using System.Buffers;

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
        public EventType? ProcessEvents(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            EventType? evType = null;
            switch (state.status)
            {
                case (byte)EventType.MetaEvent:
                    evType = (EventType)data.ReadByte(offset + 1);
                    break;
                case (byte)EventType.SysexEventStart:
                case (byte)EventType.SysexEventEscape:
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
                uint key = CalcIndex(noteOn.note, noteOn.Channel);
                if (noteOn.velocity == 0 && notesActive[key].Count > 0)
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
                uint key = CalcIndex(noteOff.note, noteOff.Channel);
                NoteOnEvent on = notesActive[key].Pop();
                on.NoteOff = noteOff;
            }
        }

        public void Parse()
        {
            events.Clear();
            int pos = 0;
            while (pos < Length)
            {
                state.deltaTicks = chunkData.ReadVlv(ref pos);
                state.tickTime += state.deltaTicks;

                if (state.eventElement is BaseMidiEvent)
                {
                    state.prevMidiStatus = state.status;
                }

                state.status = chunkData.ReadByte(pos);

                EventType? type = ProcessEvents(chunkData, ref pos, state);

                if (type != null)
                {
                    TrackEvent ev = EventUtils.MidiEventFactory(type.Value);
                    ev.Previous = state.eventElement;
                    state.eventElement = ev;
                    state.eventElement.DeltaTick = state.deltaTicks;
                    state.eventElement.EventID = events.Count;
                    state.eventElement.Parse(chunkData, ref pos);

                    if (noteEventMatching)
                        MatchNoteEvents();

                    events.Add(state.eventElement);
                }

            }
        }

        public IEnumerable<T> ParseEvents<T>(EventType[] types) where T : TrackEvent
        {
            // NoteOnEvent ev = new NoteOnEvent();
            TrackParseState localState = new TrackParseState();
            int pos = 0;
            while (pos < Length)
            {

                localState.deltaTicks = chunkData.ReadVlv(ref pos);
                localState.tickTime += localState.deltaTicks;

                if (localState.eventElement is BaseMidiEvent)
                {
                    localState.prevMidiStatus = localState.status;
                }

                localState.status = chunkData.ReadByte(pos);

                EventType? typeFound = ProcessEvents(chunkData, ref pos, localState);
                TrackEvent ev;
                int eventCount = 0;

                if (typeFound != null)
                {
                    ev = TrackEvent.GetStaticEvents(typeFound.Value);
                    ev.Parse(chunkData, ref pos);
                    localState.eventElement = ev;
                    localState.eventElement.DeltaTick = localState.deltaTicks;
                    localState.eventElement.EventID = eventCount++;

                    if (ev is T rtnEv)
                    {
                        foreach (var t in types)
                        {
                            if (t == typeFound.Value)
                            {
                                yield return rtnEv;
                                break;
                            }
                        }
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
