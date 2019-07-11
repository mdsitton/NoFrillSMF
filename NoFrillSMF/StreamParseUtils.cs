using System;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace NoFrillSMF
{
    internal static class StreamParseUtils
    {

        [ThreadStatic] static byte[] scratchData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] EnsureScratch(int size)
        {
            if (scratchData == null || scratchData.Length < size)
            {
                scratchData = new byte[size];
            }

            return scratchData;
        }

        public static string ReadString(this Stream data, int size, Encoding encoding = null)
        {
            byte[] scratch = EnsureScratch(size);
            data.Read(scratch, 0, size);
            int i = 0;
            return scratch.ReadString(ref i, size, encoding);
        }

        public static int WriteString(this Stream data, string str, Encoding encoding = null)
        {
            byte[] scratch = EnsureScratch(str.Length);
            int i = 0;
            int count = scratch.WriteString(ref i, str, encoding);
            data.Write(scratch, 0, count);
            return count;
        }
    }
}
