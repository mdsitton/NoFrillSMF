using System;
using System.IO;
using NoFrill.Common;

namespace NoFrillSMF.Events
{

    public abstract class BaseMidiEvent : TrackEvent
    {

        public byte Channel;

        protected void ParseStatus(byte[] data, ref int offset)
        {

            eventType = (EventType)(Status & 0xF0);
            Channel = (byte)(Status & 0xF);
        }

        protected void ComposeStatus(byte[] data, ref int offset)
        {
            if (Previous.Status != Status)
            {
                data.WriteByte(ref offset, Status); // TODO - merge Message/Channel instead.
            }
        }
    }
}