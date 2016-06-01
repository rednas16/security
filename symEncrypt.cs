using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Windows;

namespace security
{
    class AesEncryptie
    {
        public byte[] KEY { get; private set;}
        public byte[] IV { get; private set; }
        private RijndaelManaged algoritm;
        private ICryptoTransform transformer;
        MemoryStream transformed;
        CryptoStream encryptor;


        public AesEncryptie(bool des= false)
        {
            
            transformed = new MemoryStream();

            algoritm = new RijndaelManaged();

            algoritm.Mode = CipherMode.CBC;
           
            algoritm.BlockSize = 128;
            algoritm.KeySize = 256;
            
            algoritm.GenerateKey();
            algoritm.GenerateIV();



            KEY = algoritm.Key;
            IV = algoritm.IV;

            transformer = algoritm.CreateEncryptor();

            encryptor = new CryptoStream(transformed, transformer, CryptoStreamMode.Write);
            algoritm.Dispose();
        }
        public AesEncryptie(byte[] IV, byte[] key, bool des= false)
        {

            
            transformed = new MemoryStream();

            algoritm = new RijndaelManaged();
            algoritm.Mode = CipherMode.CBC;
            algoritm.BlockSize = 128;
            algoritm.KeySize = 256;
            
            algoritm.Padding = PaddingMode.PKCS7;

            algoritm.Key = KEY = key;
            algoritm.IV=this.IV=IV;

            transformer = algoritm.CreateDecryptor();
            
            encryptor = new CryptoStream(transformed, transformer, CryptoStreamMode.Write);
            algoritm.Dispose();
        }

        public void end_encryption()
        {
            transformer.Dispose();
            transformed.Close();
            encryptor.Close();
        }

        public byte[] encrypt(byte[] data)
        { 
            transformed.SetLength(0);

            encryptor.Write(data, 0, data.Length);
            return transformed.ToArray();                          
        }
    }
}
