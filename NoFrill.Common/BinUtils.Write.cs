using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NoFrill.Common
{
    public static partial class BinUtils
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64BE(this byte[] buff, int offset, UInt64 value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<UInt64>());
            Unsafe.WriteUnaligned<UInt64>(ref buff[offset], SwapEndianess(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64LE(this byte[] buff, int offset, UInt64 value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<UInt64>());
            Unsafe.WriteUnaligned<UInt64>(ref buff[offset], value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32BE(this byte[] buff, int offset, UInt32 value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<UInt32>());
            Unsafe.WriteUnaligned<UInt32>(ref buff[offset], SwapEndianess(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32LE(this byte[] buff, int offset, UInt32 value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<UInt32>());
            Unsafe.WriteUnaligned<UInt32>(ref buff[offset], value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16BE(this byte[] buff, int offset, UInt16 value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<UInt16>());
            Unsafe.WriteUnaligned<UInt16>(ref buff[offset], SwapEndianess(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16LE(this byte[] buff, int offset, UInt16 value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<UInt16>());
            Unsafe.WriteUnaligned<UInt16>(ref buff[offset], value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Writebyte(this byte[] buff, int offset, byte value)
        {
            Debug.Assert(buff.Length >= Unsafe.SizeOf<byte>());
            Unsafe.WriteUnaligned<byte>(ref buff[offset], value);
        }

    }
}