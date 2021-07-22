using System.Diagnostics;
using System;
using NoFrill.Common;
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
            metaEvent.Size = data.ReadVlv(ref offset);
        }

        public static void ComposeStatus<T>(this T metaEvent, byte[] data, ref int offset) where T : BaseTrackEvent
        {
            data.WriteByte(ref offset, metaEvent.Status);
            data.WriteByte(ref offset, (byte)metaEvent.eventType);
            data.WriteVlv(ref offset, metaEvent.Size);
        }

        public static void ParseFast(byte[] data, ref int offset)
        {
            offset += 1;
            uint size = data.ReadVlv(ref offset);
            offset += (int)size;
        }
    }
}