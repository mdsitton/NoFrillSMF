using System;
using System.IO;
using NoFrill.Common;

namespace NoFrillSMF.Chunks
{
    public class TrackChunk : IChunk
    {
        public string TypeStr => "MTrk";

        public uint Length { get; private set; }

        protected byte[] chunkData;

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
            int pos = 0;
            UInt64 tickTime = 0;
            UInt32 deltaTicks = 0;
            Events.IEvent eventElement = null;
            Events.IEvent prevEvent = null;

            while (pos < Length)
            {
                deltaTicks = chunkData.ReadVlv(ref pos);
                tickTime += deltaTicks;
                var status = chunkData.ReadByte(pos);

                prevEvent = eventElement;

                eventElement = Events.EventUtils.EventFactory(status);
                eventElement.Previous = prevEvent;
                eventElement.Parse(chunkData, ref pos);
            }
        }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
