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
            throw new NotImplementedException();
        }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
