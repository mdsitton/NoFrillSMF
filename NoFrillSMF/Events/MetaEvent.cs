using System.IO;
using NoFrill.Common;

namespace NoFrillSMF.Events
{
    public class MetaEvent : IEvent
    {
        public byte Status => (byte)EventStatus.MetaEvent;
        public byte MetaType { get; private set; }

        public int Length { get; private set; }
        public int TotalSize { get; private set; }

        public uint DeltaTick { get; private set; }

        public IEvent Previous { get; set; }

        public void Parse(byte[] data, ref int pos)
        {
            var metaType = data.ReadByte(ref pos);
            Length = (int)data.ReadVlv(ref pos);
            pos += Length;
        }

        public void Compose(byte[] data, ref int offset)
        {
            throw new System.NotImplementedException();
        }


    }

}