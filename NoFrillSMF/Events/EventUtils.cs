namespace NoFrillSMF.Events
{
    public class EventUtils
    {
        public static IEvent EventFactory(byte status)
        {
            switch (status)
            {
                case (byte)EventStatus.MetaEvent:
                    return new MetaEvent();
                case (byte)EventStatus.SysexEventStart:
                case (byte)EventStatus.SysexEventEscape:
                    return new SysexEvent();
                default:
                    return new MidiEvent();
            }
        }

        public static T FindLast<T>(IEvent current) where T : class, IEvent
        {
            T ofType;
            while (true)
            {
                current = current.Previous;
                ofType = current as T;
                if (ofType != null)
                    return ofType;
            }
        }

    }
}