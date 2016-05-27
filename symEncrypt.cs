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
    class symEncrypt<T> where T : SymmetricAlgorithm, new()
    {
        public byte[] KEY { get; private set;}
        public byte[] IV { get; private set; }
        private byte[] buffer = new byte[4096];
        private T algoritm;
        private ICryptoTransform transformer;
        MemoryStream transformed;
        CryptoStream encryptor;


        public symEncrypt(bool des= false)
        {
            
            transformed = new MemoryStream();

            algoritm = new T();

            algoritm.Mode = CipherMode.CBC;
           
            algoritm.BlockSize = 128;
            algoritm.KeySize = 256;
            
            algoritm.GenerateKey();
            algoritm.GenerateIV();



            KEY = algoritm.Key;
            IV = algoritm.IV;

            transformer = algoritm.CreateEncryptor(algoritm.Key, algoritm.IV);

            encryptor = new CryptoStream(transformed, transformer, CryptoStreamMode.Write);
            algoritm.Dispose();
        }
        public symEncrypt(byte[] IV, byte[] key, bool des= false)
        {

            
            transformed = new MemoryStream(buffer);

            algoritm = new T();
            algoritm.Mode = CipherMode.CBC;
            algoritm.BlockSize = 128;
            algoritm.KeySize = 256;
            
            algoritm.Padding = PaddingMode.PKCS7;

            algoritm.Key = KEY = key;
            algoritm.IV=this.IV=IV;

            transformer = algoritm.CreateDecryptor(algoritm.Key, algoritm.IV);
            
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
        public byte[] encrypt(byte[] data,int count)
        {
            transformed.SetLength(0);

            encryptor.Write(data, 0,count);
            return transformed.ToArray();
        }
    }
}
