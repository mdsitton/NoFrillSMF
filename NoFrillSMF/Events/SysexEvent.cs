namespace NoFrillSMF.Events
{
    public class SysexEvent : IEvent
    {
        public byte Status { get; set; }
        public byte MetaType { get; private set; }

        public int Length { get; private set; }
        public int TotalSize { get; private set; } // TODO - Make function to figure out length of varlen

        public uint DeltaTick { get; private set; }

        public IEvent Previous { get; set; }

        public void Parse(byte[] data, ref int offset)
        {
            Length = (int)data.ReadVlv(ref offset);
            offset += Length;
        }

        public void Compose(byte[] data, ref int offset)
        {
            throw new System.NotImplementedException();
        }
    }
}