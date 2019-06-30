using System;
using System.IO;

namespace NoFrillSMF.Chunks
{
    public interface IChunk
    {
        string TypeStr { get; }
        UInt32 Length { get; }

        void Read(Stream data, UInt32 chunkLength);
        void Write(Stream data);

        void Parse();
        byte[] Compose();
    }
}
