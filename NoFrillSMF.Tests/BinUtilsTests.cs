using System;
using System.Buffers.Binary;
using NoFrill.Common;
using Xunit;

namespace NoFrillSMF.Tests
{
    public class BinUtilsTests
    {
        [Fact]
        public void UInt64Test()
        {
            // arrange
            byte[] data = new byte[8];
            UInt64 expected = 0xDEADBEEFDEAD;
            // act
            data.WriteUInt64BE(0, expected);
            UInt64 result = data.ReadUInt64BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Int64Test()
        {
            // arrange
            byte[] data = new byte[8];
            Int64 expected = -0xDEADBEEFDEA;
            // act
            data.WriteInt64BE(0, expected);
            Int64 result = data.ReadInt64BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void UInt32Test()
        {
            // arrange
            byte[] data = new byte[8];
            UInt32 expected = 0xDEADBEEF;
            // act
            data.WriteUInt32BE(0, expected);
            UInt32 result = data.ReadUInt32BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Int32Test()
        {
            // arrange
            byte[] data = new byte[8];
            Int32 expected = -0xDEADBEE;
            // act
            data.WriteInt32BE(0, expected);
            Int32 result = data.ReadInt32BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void UInt24Test()
        {
            // arrange
            byte[] data = new byte[8];
            UInt32 expected = 0xDEADBE;
            // act
            data.WriteUInt24BE(0, expected);
            UInt32 result = data.ReadUInt24BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Int24Test()
        {
            // arrange
            byte[] data = new byte[8];
            Int32 expected = -0x01;
            // act
            data.WriteInt24BE(0, expected);
            Int32 result = data.ReadInt24BE(0);

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void UInt16Test()
        {
            // arrange
            byte[] data = new byte[8];
            UInt16 expected = 0xDEAD;
            // act
            data.WriteUInt16BE(0, expected);
            UInt16 result = data.ReadUInt16BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Int16Test()
        {
            // arrange
            byte[] data = new byte[8];
            Int16 expected = -0xDEA;
            // act
            data.WriteInt16BE(0, expected);
            Int16 result = data.ReadInt16BE(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ByteTest()
        {
            // arrange
            byte[] data = new byte[8];
            byte expected = 0xDE;
            // act
            data.WriteByte(0, expected);
            byte result = data.ReadByte(0);
            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SByteTest()
        {
            // arrange
            byte[] data = new byte[8];
            SByte expected = -0xD;
            // act
            data.WriteSByte(0, expected);
            SByte result = data.ReadSByte(0);
            // assert
            Assert.Equal(expected, result);
        }

    }
}