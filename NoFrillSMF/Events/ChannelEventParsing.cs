using System;
using System.IO;
using BinaryEx;
using NoFrillSMF.Chunks;

namespace NoFrillSMF.Events
{

    public static class ChannelEventParsing
    {
        public static void ParseStatus<T>(this T channelEvent, byte[] data, ref int offset, TrackParseState state) where T : MidiChannelEvent
        {
            channelEvent.Status = state.status;
            channelEvent.eventType = (EventType)(channelEvent.Status & 0xF0);
            channelEvent.Channel = (byte)(channelEvent.Status & 0xF);
        }

        public static void ComposeStatus<T>(this T channelEvent, byte[] data, ref int offset) where T : MidiChannelEvent
        {
            if (channelEvent.Previous.Status != channelEvent.Status)
            {
                data.WriteByte(ref offset, channelEvent.Status); // TODO - merge Message/Channel instead.
            }
        }

    }
}