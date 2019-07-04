using System;
using System.Text;
using System.IO;

namespace NoFrillSMF
{
    internal static class StreamParseUtils
    {
        public static string ReadString(this Stream data, byte[] scratch, int size, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.ASCII;
            data.Read(scratch, 0, size);
            return Encoding.ASCII.GetString(scratch);
        }
    }
}
