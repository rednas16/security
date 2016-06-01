using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace security
{
    class hasher
    {
        private static int minSaltLength = 4;
        private static int maxSaltLength = 16;
        private int totalBytes;
        private byte[] SaltBytes;
        private string saltString;
        private string hashString;
        SHA256Managed sha;

        public hasher()
        {
            sha = new SHA256Managed();
            totalBytes=0;
            Random r = new Random();
            int SaltLength = r.Next(minSaltLength, maxSaltLength);
            SaltBytes = new byte[SaltLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(SaltBytes);
            rng.Dispose();
        }

        //functioneel
        private void Split_hash(string hashValue) // split de hashstring
        {
            saltString = hashValue.Split('/')[1];
            hashString = hashValue.Split('/')[0];
        }
        public void Add_part(byte[] text) // voeg een deel van de hash toe
        {
            int hulp = text.Length;
            totalBytes += hulp;
            sha.TransformBlock(text,0,hulp,null,0);
            
        }
        //hash
        public string Hash_file(bool salt = true) // hash wat in de hasher zit, true= salt toevoegen
        {
            if(salt)
                Add_part(SaltBytes);

            byte[] hulp = new byte[0];
            sha.TransformFinalBlock(hulp, 0, 0);

            if(salt)
                return Transformer.Byte_to_string(sha.Hash) + "/" + Transformer.Byte_to_string(SaltBytes);

            return Transformer.Byte_to_string(sha.Hash);
        }

        public string Hash_met_salt(String text) // hash string met salt
        {
            Add_part(Transformer.String_to_byte(text));
            
            return Hash_file(true);
        }


        public string Hash_met_salt(byte[] bytes) // hash byte met salt
        {
            Add_part(bytes);

            return Hash_file(true);
        }

        //check
        public bool Check_file(string hashValue, bool salt = true) //check of huidig hash correct is, true = salt toevoegen false = geen salt gebruikt
        {
            if (salt)
            {
                Split_hash(hashValue);
                Add_part(Transformer.String_to_byte(saltString));
            }

            if (Hash_file(false).Equals(hashString))
                return true;

            return false;
        }


        public bool check_hash(string text, string hashValue) // text toevoegen aan hasher, checkfile zal salt toevoegen aan hasher en controler
        {            
            
            Add_part(Transformer.String_to_byte(saltString));

            if (Check_file(hashValue))
                return true;

            return false;
        }
        
        public bool check_hash(byte[] bytes, string hashValue)// bytes toevoegen aan hasher, checkfile zal salt toevoegen aan hasher en controler
        {
            
            Add_part(bytes);

                    
            if (Check_file(hashValue))
                return true;

            return false;
        }
    }
}
