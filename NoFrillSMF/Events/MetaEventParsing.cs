using System.Diagnostics;
using System;
using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events
{
    public static class MetaEventParsing
    {

        public static void ParseStatus<T>(this T metaEvent, byte[] data, ref int offset, TrackParseState state) where T : BaseTrackEvent
        {
            metaEvent.Status = state.status;
            metaEvent.eventType = (EventType)data.ReadByte(ref offset);
            Debug.Assert(metaEvent.Status == 0xFF);
            metaEvent.Size = (int)data.ReadVlv(ref offset);
        }

        public static void ComposeStatus<T>(this T metaEvent, byte[] data, ref int offset) where T : BaseTrackEvent
        {
            data.WriteByte(ref offset, metaEvent.Status);
            data.WriteByte(ref offset, (byte)metaEvent.eventType);
            data.WriteVlv(ref offset, metaEvent.Size);
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            int size = data[offset + 1];
            // Slow path, rarely almost never used in practice 99% of messages only have 1 byte for message length
            if ((size & 0x80) != 0)
            {
                offset++;
                size = (int)data.ReadVlv(ref offset);
                offset += size;
                return;
            }
            offset += size + 2;
        }
    }
}