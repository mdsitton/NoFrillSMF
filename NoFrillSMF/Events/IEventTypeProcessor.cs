namespace NoFrillSMF.Events
{
    public interface IEventTypeProcessor
    {
        void Parse(byte[] data, ref int offset, Chunks.TrackParseState state);
        void Compose(byte[] data, ref int offset, Chunks.TrackParseState state);
    }
}