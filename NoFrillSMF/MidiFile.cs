using System.Security.Authentication.ExtendedProtection;
using System;
using System.Collections.Generic;
using System.IO;

using NoFrillSMF.Chunks;
using System.Text;

namespace NoFrillSMF
{
    public class MidiFile
    {
        protected List<IChunk> chunks = new List<IChunk>();

        public void ReadData(Stream data)
        {
            if (!data.CanRead)
                throw new NotSupportedException("Stream does not support reading.");

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
        }

        public void WriteData(Stream data)
        {
            if (!data.CanWrite)
                throw new NotSupportedException("Stream does not support reading.");

            if (chunks.Count == 0 && (chunks[0] as HeaderChunk) == null)
            {
                throw new FormatException("Header chunk not found");
            }

            foreach (IChunk chunk in chunks)
            {
                // TODO - Split these out into Utilities
                data.Write(Encoding.ASCII.GetBytes(chunk.TypeStr), 0, 4);

                byte[] lengthData = BitConverter.GetBytes(chunk.Length);
                Array.Reverse(lengthData, 0, 4);
                data.Write(lengthData, 0, 4);

                chunk.Write(data);
            }
        }
    }
}
