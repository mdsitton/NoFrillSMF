using System;
using System.Buffers.Binary;
using NoFrill.Common;
using Xunit;

namespace NoFrillSMF.Tests
{
    public class BinUtilsSwapEndianTests
    {
        [Fact]
        public void SwapUInt64Test()
        {
            // arrange
            UInt64 startValue = 0xDEADBEEFDEADBE23;
            UInt64 expected = BinaryPrimitives.ReverseEndianness(startValue);

            // act
            UInt64 result = BinUtils.SwapEndianess(startValue);

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SwapUInt32Test()
        {
            // arrange
            UInt32 startValue = 0xDEADBEEF;
            UInt32 expected = BinaryPrimitives.ReverseEndianness(startValue);


            // act
            UInt32 result = BinUtils.SwapEndianess(startValue);

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SwapUInt16Test()
        {
            // arrange
            UInt16 startValue = 0xDEAD;
            UInt16 expected = BinaryPrimitives.ReverseEndianness(startValue);

            // act
            UInt16 result = BinUtils.SwapEndianess(startValue);

            // assert
            Assert.Equal(expected, result);
        }
        [Fact]
        public void SwapInt64Test()
        {
            // arrange
            Int64 startValue = -0xDEADBEEFDEADBE2;
            Int64 expected = BinaryPrimitives.ReverseEndianness(startValue);

            // act
            Int64 result = BinUtils.SwapEndianess(startValue);

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SwapInt32Test()
        {
            // arrange
            Int32 startValue = -0xDEADBEE;
            Int32 expected = BinaryPrimitives.ReverseEndianness(startValue);

            // act
            Int32 result = BinUtils.SwapEndianess(startValue);

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SwapInt16Test()
        {
            // arrange
            Int16 startValue = -0xDEA;
            Int16 expected = BinaryPrimitives.ReverseEndianness(startValue);

            // act
            Int16 result = BinUtils.SwapEndianess(startValue);

            // assert
            Assert.Equal(expected, result);
        }
    }
}