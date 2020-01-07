using System;
using NoFrillSMF.Events;

namespace NoFrillSMF.Chunks
{
    public class TrackParseState
    {
        public int eventCount;
        public int trackDataPosition;
        public UInt64 tickTime;
        public UInt32 deltaTicks;
        public TrackEvent eventElement;
        public byte prevMidiStatus;
        public byte status;
        // TODO - Add last tempo event tracking,

        public TrackParseState(int _trackDataPosition = 0, UInt64 _tickTime = 0, UInt32 _deltaTicks = 0, TrackEvent _eventElement = null, byte _prevMidiStatus = 0, byte _status = 0)
        {
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
