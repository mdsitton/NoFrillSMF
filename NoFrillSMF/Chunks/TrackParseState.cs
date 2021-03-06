using System;
using NoFrillSMF.Events;

namespace NoFrillSMF.Chunks
{
    public class TrackParseState
    {
        public UInt64 tickTime = 0;
        public UInt32 deltaTicks = 0;
        public TrackEvent eventElement = null;
        public byte prevMidiStatus = 0;
        public byte status = 0;
    }
}
