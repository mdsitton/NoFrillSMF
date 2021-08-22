using System;
using System.Collections.Generic;
using System.Linq;
using NoFrillSMF.Events;
using NoFrillSMF.Events.MidiEvents;

namespace NoFrillSMF.Chunks
{
    public class TrackParseState
    {
        public int eventCount;
        public int trackDataPosition;
        public UInt64 tickTime;
        public UInt32 deltaTicks;
        public BaseTrackEvent eventElement;
        public byte prevMidiStatus;
        public byte status;
        public bool isUsingObjPool = false;

        private static Stack<NoteEvent>[] notesActiveCache;

        public Stack<NoteEvent>[] notesActive;
        public readonly bool noteEventMatching;
        // TODO - Add last tempo event tracking,

        public TrackParseState(int _trackDataPosition = 0, UInt64 _tickTime = 0, UInt32 _deltaTicks = 0, BaseTrackEvent _eventElement = null, byte _prevMidiStatus = 0, byte _status = 0, bool noteEventMatching = false)
        {

            // In some cases we don't want to use a cached stack array because we may want to parse something totally diffenent and not break an existing parser state
            if (noteEventMatching)
            {
                this.noteEventMatching = noteEventMatching;
                // Allocate a stack per note per channel, to handle so we can handle matching note on/off events with O(1) complexity.
                // This is important for handling extremely large midi files.
                notesActive = Enumerable.Repeat(new Stack<NoteEvent>(), 128 * 16).ToArray();
                // TODO - Build caching system for tracking unused instances of NoteEvent stack arrays
                // This will be essential in having decent performance for lots of small search tasks
                // if (useCachedNoteMatching)
                // {

                //     if (notesActive == null)
                //     {
                //         notesActiveCache = Enumerable.Repeat(new Stack<NoteEvent>(), 128 * 16).ToArray();
                //         notesActive = notesActiveCache;
                //     }
                //     else
                //     {
                //         // Clear notes cache
                //         notesActive = notesActiveCache;
                //         for (int i = 0; i < notesActive.Length; ++i)
                //         {
                //             notesActive[i].Clear();
                //         }
                //     }
                // }
            }
            trackDataPosition = _trackDataPosition;
            tickTime = _tickTime;
            deltaTicks = _deltaTicks;
            eventElement = _eventElement;
            prevMidiStatus = _prevMidiStatus;
            status = _status;
            eventCount = 0;
        }
    }
}
