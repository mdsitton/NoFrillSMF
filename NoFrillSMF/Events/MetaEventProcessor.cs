using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public class MetaEventProcessor : IEventTypeProcessor
    {
        public void Compose(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            throw new System.NotImplementedException();
        }

        public void Parse(byte[] data, ref int offset, Chunks.TrackParseState state)
        {
            var metaType = data.ReadByte(ref offset);
            offset += (int)data.ReadVlv(ref offset);
        }
    }
}