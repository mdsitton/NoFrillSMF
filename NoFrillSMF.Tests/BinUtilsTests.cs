using System;
using System.Buffers.Binary;
using NoFrill.Common;
using Xunit;

namespace NoFrillSMF.Tests
{
    public class BinUtilsTests
    {
        [Theory]
        [InlineData(UInt64.MinValue)]
        [InlineData(0xDEADBEEFDEAD)]
        [InlineData(UInt64.MaxValue)]
        public void UInt64BETest(UInt64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt64BE(0, expected);
            UInt64 result = data.ReadUInt64BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt64.MinValue)]
        [InlineData(0xDEADBEEFDEAD)]
        [InlineData(UInt64.MaxValue)]
        public void UInt64BEReadTest(UInt64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            UInt64 result = data.ReadUInt64BE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(UInt64.MinValue)]
        [InlineData(0xDEADBEEFDEAD)]
        [InlineData(UInt64.MaxValue)]
        public void UInt64BEWriteTest(UInt64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteUInt64BE(ref pos, expected);
            UInt64 result = BinaryPrimitives.ReadUInt64BigEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(UInt64.MinValue)]
        [InlineData(0xDEADBEEFDEAD)]
        [InlineData(UInt64.MaxValue)]
        public void UInt64LETest(UInt64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt64LE(0, expected);
            UInt64 result = data.ReadUInt64LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt64.MinValue)]
        [InlineData(0xDEADBEEFDEAD)]
        [InlineData(UInt64.MaxValue)]
        public void UInt64LEReadTest(UInt64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            UInt64 result = data.ReadUInt64LE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(UInt64.MinValue)]
        [InlineData(0xDEADBEEFDEAD)]
        [InlineData(UInt64.MaxValue)]
        public void UInt64LEWriteTest(UInt64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteUInt64LE(ref pos, expected);
            UInt64 result = BinaryPrimitives.ReadUInt64LittleEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(Int64.MinValue)]
        [InlineData(-0xDEADBEEFDEA)]
        [InlineData(Int64.MaxValue)]
        public void Int64BETest(Int64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt64BE(0, expected);
            Int64 result = data.ReadInt64BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Int64.MinValue)]
        [InlineData(-0xDEADBEEFDEA)]
        [InlineData(Int64.MaxValue)]
        public void Int64BEReadTest(Int64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteInt64BigEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            Int64 result = data.ReadInt64BE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(Int64.MinValue)]
        [InlineData(-0xDEADBEEFDEA)]
        [InlineData(Int64.MaxValue)]
        public void Int64BEWriteTest(Int64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteInt64BE(ref pos, expected);
            Int64 result = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(Int64.MinValue)]
        [InlineData(-0xDEADBEEFDEA)]
        [InlineData(Int64.MaxValue)]
        public void Int64LETest(Int64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt64LE(0, expected);
            Int64 result = data.ReadInt64LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Int64.MinValue)]
        [InlineData(-0xDEADBEEFDEA)]
        [InlineData(Int64.MaxValue)]
        public void Int64LEReadTest(Int64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteInt64LittleEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            Int64 result = data.ReadInt64LE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }

        [Theory]
        [InlineData(Int64.MinValue)]
        [InlineData(-0xDEADBEEFDEA)]
        [InlineData(Int64.MaxValue)]
        public void Int64LEWriteTest(Int64 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteInt64LE(ref pos, expected);
            Int64 result = BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(8, pos);
        }


        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBEEF)]
        [InlineData(UInt32.MaxValue)]
        public void UInt32BETest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt32BE(0, expected);
            UInt32 result = data.ReadUInt32BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBEEF)]
        [InlineData(UInt32.MaxValue)]
        public void UInt32BEReadTest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            UInt32 result = data.ReadUInt32BE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBEEF)]
        [InlineData(UInt32.MaxValue)]
        public void UInt32BEWriteTest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteUInt32BE(ref pos, expected);
            UInt32 result = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBEEF)]
        [InlineData(UInt32.MaxValue)]
        public void UInt32LETest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt32LE(0, expected);
            UInt32 result = data.ReadUInt32LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBEEF)]
        [InlineData(UInt32.MaxValue)]
        public void UInt32LEReadTest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            UInt32 result = data.ReadUInt32LE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBEEF)]
        [InlineData(UInt32.MaxValue)]
        public void UInt32LEWriteTest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteUInt32LE(ref pos, expected);
            UInt32 result = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(Int32.MinValue)]
        [InlineData(-0xDEADBEE)]
        [InlineData(Int32.MaxValue)]
        public void Int32BETest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt32BE(0, expected);
            Int32 result = data.ReadInt32BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Int32.MinValue)]
        [InlineData(-0xDEADBEE)]
        [InlineData(Int32.MaxValue)]
        public void Int32BEReadTest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteInt32BigEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            Int32 result = data.ReadInt32BE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(Int32.MinValue)]
        [InlineData(-0xDEADBEE)]
        [InlineData(Int32.MaxValue)]
        public void Int32BEWriteTest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteInt32BE(ref pos, expected);
            Int32 result = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(Int32.MinValue)]
        [InlineData(-0xDEADBEE)]
        [InlineData(Int32.MaxValue)]
        public void Int32LETest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt32LE(0, expected);
            Int32 result = data.ReadInt32LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Int32.MinValue)]
        [InlineData(-0xDEADBEE)]
        [InlineData(Int32.MaxValue)]
        public void Int32LEReadTest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            Int32 result = data.ReadInt32LE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(Int32.MinValue)]
        [InlineData(-0xDEADBEE)]
        [InlineData(Int32.MaxValue)]
        public void Int32LEWriteTest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteInt32LE(ref pos, expected);
            Int32 result = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(4, pos);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBE)]
        [InlineData(0xFFFFFF)]
        public void UInt24BETest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt24BE(0, expected);
            UInt32 result = data.ReadUInt24BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt32.MinValue)]
        [InlineData(0xDEADBE)]
        [InlineData(0xFFFFFF)]
        public void UInt24LETest(UInt32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt24LE(0, expected);
            UInt32 result = data.ReadUInt24LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-0x7fffff)]
        [InlineData(-0xDEADB)]
        [InlineData(0x7fffff)]
        public void Int24BETest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt24BE(0, expected);
            Int32 result = data.ReadInt24BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-0x7fffff)]
        [InlineData(-0xDEADB)]
        [InlineData(0x7fffff)]
        public void Int24LETest(Int32 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt24LE(0, expected);
            Int32 result = data.ReadInt24LE(0);
            // assert
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(UInt16.MinValue)]
        [InlineData(0xDEAD)]
        [InlineData(UInt16.MaxValue)]
        public void UInt16BETest(UInt16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt16BE(0, expected);
            UInt16 result = data.ReadUInt16BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt16.MinValue)]
        [InlineData(0xDEAD)]
        [InlineData(UInt16.MaxValue)]
        public void UInt16BEReadTest(UInt16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            UInt16 result = data.ReadUInt16BE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(UInt16.MinValue)]
        [InlineData(0xDEAD)]
        [InlineData(UInt16.MaxValue)]
        public void UInt16BEWriteTest(UInt16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteUInt16BE(ref pos, expected);
            UInt16 result = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(UInt16.MinValue)]
        [InlineData(0xDEAD)]
        [InlineData(UInt16.MaxValue)]
        public void UInt16LETest(UInt16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteUInt16LE(0, expected);
            UInt16 result = data.ReadUInt16LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UInt16.MinValue)]
        [InlineData(0xDEAD)]
        [InlineData(UInt16.MaxValue)]
        public void UInt16LEReadTest(UInt16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            UInt16 result = data.ReadUInt16LE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(UInt16.MinValue)]
        [InlineData(0xDEAD)]
        [InlineData(UInt16.MaxValue)]
        public void UInt16LEWriteTest(UInt16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteUInt16LE(ref pos, expected);
            UInt16 result = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(Int16.MinValue)]
        [InlineData(-0xDEA)]
        [InlineData(Int16.MaxValue)]
        public void Int16BETest(Int16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt16BE(0, expected);
            Int16 result = data.ReadInt16BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Int16.MinValue)]
        [InlineData(-0xDEA)]
        [InlineData(Int16.MaxValue)]
        public void Int16BEReadTest(Int16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteInt16BigEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            Int16 result = data.ReadInt16BE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(Int16.MinValue)]
        [InlineData(-0xDEA)]
        [InlineData(Int16.MaxValue)]
        public void Int16BEWriteTest(Int16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteInt16BE(ref pos, expected);
            Int16 result = BinaryPrimitives.ReadInt16BigEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(Int16.MinValue)]
        [InlineData(-0xDEA)]
        [InlineData(Int16.MaxValue)]
        public void Int16LETest(Int16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteInt16LE(0, expected);
            Int16 result = data.ReadInt16LE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Int16.MinValue)]
        [InlineData(-0xDEA)]
        [InlineData(Int16.MaxValue)]
        public void Int16LEReadTest(Int16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            BinaryPrimitives.WriteInt16LittleEndian(data.AsSpan(), expected);

            // act
            int pos = 0;
            Int16 result = data.ReadInt16LE(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }

        [Theory]
        [InlineData(Int16.MinValue)]
        [InlineData(-0xDEA)]
        [InlineData(Int16.MaxValue)]
        public void Int16LEWriteTest(Int16 expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteInt16LE(ref pos, expected);
            Int16 result = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan());

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(2, pos);
        }


        [Theory]
        [InlineData(Byte.MinValue)]
        [InlineData(0xDE)]
        [InlineData(Byte.MaxValue)]
        public void ByteTest(Byte expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteByte(0, expected);
            Byte result = data.ReadByte(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Byte.MinValue)]
        [InlineData(0xDE)]
        [InlineData(Byte.MaxValue)]
        public void ByteReadTest(Byte expected)
        {
            // arrange
            byte[] data = new byte[8];
            data[0] = expected;

            // act
            int pos = 0;
            Byte result = data.ReadByte(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(1, pos);
        }

        [Theory]
        [InlineData(Byte.MinValue)]
        [InlineData(0xDE)]
        [InlineData(Byte.MaxValue)]
        public void ByteWriteTest(Byte expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteByte(ref pos, expected);
            Byte result = data[0];

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(1, pos);
        }

        [Theory]
        [InlineData(SByte.MinValue)]
        [InlineData(-0xD)]
        [InlineData(SByte.MaxValue)]
        public void SByteTest(SByte expected)
        {
            // arrange
            byte[] data = new byte[8];
            // act
            data.WriteSByte(0, expected);
            SByte result = data.ReadSByte(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(SByte.MinValue)]
        [InlineData(-0xD)]
        [InlineData(SByte.MaxValue)]
        public void SByteReadTest(SByte expected)
        {
            // arrange
            byte[] data = new byte[8];
            data[0] = (Byte)expected;

            // act
            int pos = 0;
            SByte result = data.ReadSByte(ref pos);

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(1, pos);
        }

        [Theory]
        [InlineData(SByte.MinValue)]
        [InlineData(-0xD)]
        [InlineData(SByte.MaxValue)]
        public void SByteWriteTest(SByte expected)
        {
            // arrange
            byte[] data = new byte[8];
            int pos = 0;

            // act
            data.WriteSByte(ref pos, expected);
            SByte result = (SByte)data[0];

            // assert
            Assert.Equal(expected, result);
            Assert.Equal(1, pos);
        }
    }
}