using System.IO;

namespace NoFrillSMF.Events
{
    public class MetaEvent : IEvent
    {
        public byte Status => (byte)EventStatus.MetaEvent;

        public byte Length { get; private set; }

        public void Read(Stream data, byte readAmount)
        {
            throw new System.NotImplementedException();
        }

        public void Write(Stream data)
        {
            throw new System.NotImplementedException();
        }

        public void Compose()
        {
            throw new System.NotImplementedException();
        }

        public void Parse()
        {
            throw new System.NotImplementedException();
        }

    }

}