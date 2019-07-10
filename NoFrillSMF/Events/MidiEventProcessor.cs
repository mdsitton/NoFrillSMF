using System;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public class MidiEventProcessor : IEventTypeProcessor
    {
        public void Compose(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            if (state.eventElement is null)
            {
                // Check if we should use the running status.
                if ((state.status & 0xF0) >= 0x80)
                {
                    data.ReadByte(ref offset);
                }
                else
                {
                    BaseMidiEvent prev = state.prevEvent as BaseMidiEvent;

                    if (prev is null)
                    {
                        Console.WriteLine("Warning: Incorrect running status found, assuming last midi event status");
                        prev = EventUtils.FindLast<BaseMidiEvent>(state.prevEvent);
                    }

                    state.status = prev.Status;
                }

                MidiChannelMessage message = (MidiChannelMessage)(state.status & 0xF0);
                state.eventElement = EventUtils.MidiEventFactory(message);
            }
        }

        public void Parse(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            throw new System.NotImplementedException();
        }
    }
}