using System.Security.Authentication.ExtendedProtection;
using System;
using System.Collections.Generic;
using System.IO;

using NoFrillSMF.Chunks;

namespace NoFrillSMF
{
    public class MidiFile
    {
        private readonly Stream data;

        protected List<IChunk> chunks = new List<IChunk>();

        public MidiFile(byte[] data)
        {
            this.data = new MemoryStream(data);
        }

        public MidiFile(Stream data)
        {
            this.data = data;
        }

        public bool ParseData()
        {
            long fileSize = data.Length;

            IChunk chunk;
            byte[] dataBuffer = new byte[4];
            int chunkCount = 0;

            while (data.Position < fileSize)
            {
                string str = data.ReadString(dataBuffer, size: 4);
                UInt32 chunkLength = data.ReadUInt32(dataBuffer, flipEndianness: true);

                if (chunkLength > fileSize)
                {
                    // TODO - Turn this into an exception?
                    Console.WriteLine("WARNING: Chunk size beyond end of stream, ignoring chunk.");
                    break;
                }

                chunk = ChunkMappings.ChunkFactory(str);

                chunk.Read(data, chunkLength);
                chunks.Add(chunk);
                chunkCount++;
                Console.WriteLine(chunkCount);
            }

            return true;
        }
    }
}
