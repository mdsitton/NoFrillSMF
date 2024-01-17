using System.Security.Authentication.ExtendedProtection;
using System;
using System.Collections.Generic;
using System.IO;
using BinaryEx;

using NoFrillSMF.Chunks;
using System.Text;

namespace NoFrillSMF
{
    public class MidiFile
    {
        protected HeaderChunk headerChunk = new HeaderChunk();
        protected List<TrackChunk> trackChunks = new List<TrackChunk>();
        private readonly bool noteEventMatching;

        public MidiFile(bool noteEventMatching = true)
        {
            this.noteEventMatching = noteEventMatching;
        }

        public void ReadData(Stream data)
        {
            trackChunks.Clear();
            if (!data.CanRead)
                throw new NotSupportedException("Stream does not support reading.");

            long fileSize = data.Length;

            headerChunk.Read(data);

            int trackCount = 0;
            while (data.Position < fileSize)
            {
                string str = data.ReadString(size: 4);
                UInt32 chunkLength = data.ReadUInt32BE();

                if (chunkLength > fileSize)
                {
                    // TODO - Turn this into an exception?
                    Console.WriteLine("WARNING: Chunk size beyond end of stream, ignoring chunk.");
                    break;
                }

                if (str != "MTrk")
                    data.Position += chunkLength;

                TrackChunk chunk = new TrackChunk(noteEventMatching);
                chunk.Read(data, chunkLength);
                trackChunks.Add(chunk);
                trackCount++;
                //Console.WriteLine(chunkCount);
            }
        }

        public IList<TrackChunk> GetTrackChunks()
        {
            return trackChunks;
        }

        public void Parse()
        {
            foreach (TrackChunk chnk in trackChunks)
            {
                chnk.Parse();
            }
        }

        public void WriteData(Stream data)
        {
            if (!data.CanWrite)
                throw new NotSupportedException("Stream does not support reading.");

            headerChunk.Write(data);

            foreach (TrackChunk chunk in trackChunks)
            {
                data.WriteString(chunk.TypeStr);
                data.WriteUInt32BE(chunk.Length);
                chunk.Write(data);
            }
        }
    }
}
