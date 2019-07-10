namespace NoFrillSMF.Events
{
    public class SysexEventProcessor : IEventTypeProcessor
    {
        public void Compose(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            throw new System.NotImplementedException();
        }

        public void Parse(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            offset += (int)data.ReadVlv(ref offset);
        }
    }
}