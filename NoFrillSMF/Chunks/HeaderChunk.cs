using System.Text;
using System;
using System.IO;
using NoFrill.Common;

namespace NoFrillSMF.Chunks
{
    internal class HeaderChunk : IChunk
    {
        public string TypeStr => "MThd";
        public UInt32 Length { get; private set; }
        public UInt16 Format { get; private set; }
        public UInt16 TrackCount { get; private set; }
        private UInt16 rawDivision;
        public UInt16 Division => (UInt16)(rawDivision & ~0x8000);

        public bool IsSmpte => (rawDivision & 0x8000) != 0;

        protected byte[] chunkData;

        public void Read(Stream data, UInt32 chunkLength)
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
            int position = 0;
            Format = chunkData.ReadUInt16BE(ref position);
            TrackCount = chunkData.ReadUInt16BE(ref position);
            rawDivision = chunkData.ReadUInt16BE(ref position);
        }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
