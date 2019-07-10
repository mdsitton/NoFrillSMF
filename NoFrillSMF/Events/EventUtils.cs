using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public static class EventUtils
    {

        public static IEventTypeProcessor EventProcessorFactory(byte status)
        {
            switch (status)
            {
                case (byte)EventStatus.MetaEvent:
                    return new MetaEventProcessor();
                case (byte)EventStatus.SysexEventStart:
                case (byte)EventStatus.SysexEventEscape:
                    return new SysexEventProcessor();
                default:
                    return new MidiEventProcessor();
            }
        }

        public static T FindLast<T>(IEvent current) where T : class, IEvent
        {
            while (true)
            {
                current = current.Previous;
                if (current is T)
                    return current as T;
            }
        }

    }
}