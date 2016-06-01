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
            byte[] hulp = new byte[text.Length/3];
            StringBuilder substring = new StringBuilder();
            string character= "";
            int teller=0;

            for (int i = 0; i < text.Length; i++)
            {
                character = text.Substring(i, 1);
                if (character.Equals(";"))
                {
                    hulp[teller] = Convert.ToByte(substring.ToString(), 16);
                    teller++;
                    substring.Length = 0;
                }
                else
                    substring.Append(character);

            }
            return hulp;
            
        }

        public static string Byte_to_string(byte[] bytes)
        {
            StringBuilder text = new StringBuilder();
            foreach (byte b in bytes)
            {
                text.Append(b.ToString("X2").ToLower());
                text.Append(";");
            }
            return text.ToString();
        }

        public static byte[] GetByteValue(string text) // blank string omzetten
        {
            return ASCIIEncoding.UTF8.GetBytes(text);
        }
    }
}
