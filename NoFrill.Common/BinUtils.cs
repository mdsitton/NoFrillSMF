using System;
using System.Runtime.CompilerServices;

namespace NoFrill.Common
{
    public static partial class BinUtils
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 SwapEndianess(Int16 val)
        {
            return (Int16)SwapEndianess((UInt16)val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 SwapEndianess(Int32 val)
        {
            return (Int32)SwapEndianess((UInt32)val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 SwapEndianess(Int64 val)
        {
            return (Int64)SwapEndianess((UInt64)val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 SwapEndianess(UInt16 val)
        {
            return (UInt16)((val << 8) | (val >> 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 SwapEndianess(UInt32 val)
        {
            UInt32 tmp = (val << 16) | ((val >> 16) & 0x00ffff);
            return ((tmp >> 8) & 0x00ff00ff) | ((tmp & 0x00ff00ff) << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 SwapEndianess(UInt64 val)
        {
            return (((UInt64)SwapEndianess((UInt32)val)) << 32) | (UInt64)SwapEndianess((UInt32)(val >> 32));
        }
    }
}
