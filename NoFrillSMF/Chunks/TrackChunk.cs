using System;
using System.IO;
using NoFrill.Common;

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
            int pos = 0;
            UInt64 tickTime = 0;
            UInt32 deltaTicks = 0;
            byte runningStatus = 0;

            while (pos < Length)
            {
                deltaTicks = chunkData.ReadVlv(ref pos);
                tickTime += deltaTicks;
                var status = chunkData.ReadByte(pos);

                if (status == (byte)Events.EventStatus.MetaEvent)
                {
                    pos++;
                    var metaType = chunkData.ReadByte(ref pos);
                    var len = chunkData.ReadVlv(ref pos);
                    pos += (int)len;
                }
                else if (status == (byte)Events.EventStatus.SysexEventStart || status == (byte)Events.EventStatus.SysexEventEscape)
                {
                    pos++;
                    var len = chunkData.ReadVlv(ref pos);
                    pos += (int)len;
                }
                else
                {
                    // Check if we should use the running status.
                    if ((status & 0xF0) >= 0x80)
                    {
                        pos++;
                        runningStatus = status;
                    }
                    else
                    {
                        status = runningStatus;
                    }
                    var message = status & 0xF0;
                    var channel = status & 0xF;
                    var note = chunkData.ReadByte(ref pos);
                    var vel = chunkData.ReadByte(ref pos);
                    //Console.WriteLine($"{note} {vel}");
                }
            }
        }

        public byte[] Compose()
        {
            throw new NotImplementedException();
        }
    }
}
