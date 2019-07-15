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

        public TrackChunk()
        {
            // Allocate a stack per note per channel, to handle so we can handle matching note on/off events with O(1) complexity.
            // This is important for handling extremely large midi files.
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
        public void ProcessEvents(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            switch (state.status)
            {
                case (byte)EventStatus.MetaEvent:
                    MidiMetaEvent metaType = (MidiMetaEvent)data.ReadByte(offset + 1);
                    state.eventElement = EventUtils.MetaEventFactory(metaType);
                    break;
                case (byte)EventStatus.SysexEventStart:
                case (byte)EventStatus.SysexEventEscape:
                    offset += (int)data.ReadVlv(ref offset);
                    break;
                default:

                    if ((state.status & 0xF0) >= 0x80)
                    {
                        offset++; // Skip status byte
                    }
                    else
                    {
                        BaseMidiEvent prev = state.prevEvent as BaseMidiEvent;

                        if (prev is null)
                        {
                            Console.WriteLine("Warning: Incorrect running status found, assuming last midi event status");
                            prev = EventUtils.FindLast<BaseMidiEvent>(state.prevEvent);
                        }

                        state.status = prev.Status;
                    }


                    MidiChannelMessage message = (MidiChannelMessage)(state.status & 0xF0);
                    state.eventElement = EventUtils.MidiEventFactory(message);
                    break;
            }
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
                state.status = chunkData.ReadByte(pos);
                state.prevEvent = state.eventElement;

                ProcessEvents(chunkData, ref pos, state);

                state.eventElement.DeltaTick = state.deltaTicks;
                state.eventElement.Previous = state.prevEvent;
                state.eventElement.EventID = events.Count;
                state.eventElement.Parse(chunkData, ref pos);

                MatchNoteEvents();

                events.Add(state.eventElement);
            }
        }

        // public T GetEvents<T>() where T : class, TrackEvent
        // {
        //     Type tType = typeof(T);
        //     Type midiDataType;

        //     int pos = 0;
        //     while (pos < Length)
        //     {

        //         state.deltaTicks = chunkData.ReadVlv(ref pos);
        //         state.tickTime += state.deltaTicks;
        //         state.status = chunkData.ReadByte(pos);
        //         state.prevEvent = state.eventElement;
        //     }

        // }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
