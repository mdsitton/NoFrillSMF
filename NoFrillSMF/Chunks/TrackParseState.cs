using System;
using NoFrillSMF.Events;

namespace NoFrillSMF.Chunks
{
    public class TrackParseState
    {
        public UInt64 tickTime = 0;
        public UInt32 deltaTicks = 0;
        public IEvent eventElement = null;
        public IEvent prevEvent = null;
        public byte status = 0;
    }
}
