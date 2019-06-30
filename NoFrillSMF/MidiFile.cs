using System;
using System.Collections.Generic;
using System.Text;
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
            UInt32 chunkLength;
            int chunkCount = 0;

            while (data.Position < fileSize)
            {
                data.Read(dataBuffer, 0, 4);
                string str = Encoding.ASCII.GetString(dataBuffer);

                data.Read(dataBuffer, 0, 4);
                Array.Reverse(dataBuffer);
                chunkLength = BitConverter.ToUInt32(dataBuffer, 0);

                chunk = ChunkMappings.ChunkFactory(str);

                if (chunk != null)
                {
                    chunk.Read(data, chunkLength);
                    chunks.Add(chunk);
                    chunkCount++;
                }
                Console.WriteLine(chunkCount);
            }

            return true;
        }
    }
}
