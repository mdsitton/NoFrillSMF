﻿using System;
using NoFrill.Common;

namespace NoFrillSMF.Events.MetaEvents
{
    public class UnsupportedEvent : BaseMetaEvent
    {
        public byte[] dataBlock { get; private set; }

        public override void Parse(byte[] data, ref int offset)
        {
            ParseStatus(data, ref offset);
            dataBlock = new byte[Size];
            data.ReadBytes(ref offset, dataBlock, Size);
        }

        public override void Compose(byte[] data, ref int offset)
        {
            ComposeStatus(data, ref offset);
            data.WriteBytes(ref offset, dataBlock, Size);
        }
    }
}