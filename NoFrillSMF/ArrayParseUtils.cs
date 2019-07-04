using System;
using System.Text;

namespace NoFrillSMF
{
    internal static class ArrayParseUtils
    {
        public static string ReadString(this byte[] data, ref int position, byte[] scratch, int size, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;

            Array.Copy(data, position, scratch, 0, size);
            return encoding.GetString(scratch);
        }
    }
}
