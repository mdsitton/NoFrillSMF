using System;
using NoFrill.Common;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events
{
    public abstract class BaseTrackEvent : IPoolable
    {
        public int EventID;
        public uint DeltaTick;
        public ulong TickTime;
        public byte Status;
        public EventType eventType;
        public int Size;
        public BaseTrackEvent Previous;

        public abstract void Parse(byte[] data, ref int offset, TrackParseState state);
        public abstract void Compose(byte[] data, ref int offset);

        public abstract void Clear();
    }
}