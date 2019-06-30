using System;
using System.IO;

namespace NoFrillSMF.Chunks
{
    public class TrackChunk : IChunk
    {
        public string TypeStr => "MTrk";

        public uint Length { get; private set; }

        protected byte[] chunkData;

        public void Read(Stream data, uint chunkLength)
        {
            chunkData = new byte[chunkLength];
            data.Read(chunkData, 0, (int)chunkLength);
        }

        public void Write(Stream data)
        {
            throw new NotImplementedException();
        }
    }
}
