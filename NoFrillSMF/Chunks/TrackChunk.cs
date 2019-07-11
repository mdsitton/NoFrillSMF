using System;
using System.Collections.Generic;
using System.IO;
using NoFrill.Common;
using NoFrillSMF.Events;

namespace NoFrillSMF.Chunks
{
    public class TrackChunk : IChunk
    {
        public string TypeStr => "MTrk";

        public uint Length { get; private set; }

        protected byte[] chunkData;
        protected List<IEvent> events = new List<IEvent>();
        protected TrackParseState state = new TrackParseState();

        public void Read(Stream data, uint chunkLength)
        {
            Length = chunkLength;
            chunkData = new byte[chunkLength];
            data.Read(chunkData, 0, (int)chunkLength);
        }

        public void Write(Stream data)
        {
            data.Write(chunkData, 0, chunkData.Length);
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

                // TODO - Cache these instances
                Events.IEventTypeProcessor processor = EventUtils.EventProcessorFactory(state.status);
                processor.Parse(chunkData, ref pos, state);

                state.prevEvent = state.eventElement;
                state.eventElement.DeltaTick = state.deltaTicks;

                state.eventElement.Previous = state.prevEvent;
                events.Add(state.eventElement);
                state.eventElement.Parse(chunkData, ref pos);
            }
        }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
