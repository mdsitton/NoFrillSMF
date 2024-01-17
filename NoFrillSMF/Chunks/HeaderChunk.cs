using System.Text;
using System;
using System.IO;
using BinaryEx;

namespace NoFrillSMF.Chunks
{
    public struct HeaderChunk
    {
        public string TypeStr => "MThd";
        public UInt32 Length;
        public UInt16 Format;
        public UInt16 TrackCount;
        private UInt16 rawDivision;
        public UInt16 Division => (UInt16)(rawDivision & ~0x8000);

        public bool IsSmpte => (rawDivision & 0x8000) != 0;

        public void Read(Stream data)
        {
            string str = data.ReadString(size: 4);

            if (str != TypeStr)
                throw new InvalidDataException("Incorrect header chunk type string found");

            Length = data.ReadUInt32BE();
            Format = data.ReadUInt16BE();
            TrackCount = data.ReadUInt16BE();
            rawDivision = data.ReadUInt16BE();
        }

        public void Write(Stream data)
        {
            data.WriteString(TypeStr);
            data.WriteUInt32BE(Length);
            data.WriteUInt16BE(Format);
            data.WriteUInt16BE(TrackCount);
            data.WriteUInt16BE(rawDivision);
        }
    }
}
