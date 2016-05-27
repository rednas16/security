using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace security
{
    static class Transformer
    {
        public static byte[] String_to_byte(string text)
        {

            return Enumerable.Range(0, text.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(text.Substring(x, 2), 16))
                     .ToArray();
        }

        public static string Byte_to_string(byte[] bytes)
        {
            StringBuilder text = new StringBuilder();
            foreach (byte b in bytes)
            {
                text.Append(b.ToString("X2").ToLower());
            }
            return text.ToString();
        }

        public static byte[] GetByteValue(string text) // blank string omzetten
        {
            return ASCIIEncoding.UTF8.GetBytes(text);
        }
    }
}
