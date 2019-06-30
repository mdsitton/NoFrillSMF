using System;
using System.IO;

namespace NoFrillSMF.Chunks
{
    internal class HeaderChunk : IChunk
    {
        public string TypeStr => "MThd";
        public UInt32 Length { get; private set; }
        public UInt16 Format { get; private set; }
        public UInt16 TrackCount { get; private set; }
        public UInt16 RawDivision { get; private set; }
        public UInt16 Division { get; private set; }

        public bool IsSmpte => throw new NotImplementedException();

        protected byte[] chunkData;

        public void Read(Stream data, UInt32 chunkLength)
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
