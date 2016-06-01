using System;
using System.IO;
using System.Security.Cryptography;

namespace security
{
    public class RSA
    {

        public string privKey { get; private set; }
        public string pubKey { get; private set; }
        private RijndaelManaged rndm = new RijndaelManaged();

        public void generateNewKeyPair()
        {
            CspParameters cspParams = new CspParameters();
            cspParams.ProviderType = 1;
            cspParams.Flags = CspProviderFlags.UseArchivableKey;
            cspParams.KeyNumber = (int)KeyNumber.Exchange;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);


            privKey = RSA.ToXmlString(true);
            pubKey = RSA.ToXmlString(false);

            RSA.Dispose();
        }

        public void SetPrivKey(string privKey)
        {
            this.privKey = privKey;
        }

        public void SetPubKey(string pubKey)
        {
            this.pubKey = pubKey;
        }
        

        public byte[] run_encrypt(byte[] data)
        {
            if (pubKey != null)
            {
                CspParameters cspParams = new CspParameters();
                cspParams.ProviderType = 1; // 1 = PROV_RSA_FULL
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);
                RSA.FromXmlString(pubKey);




                using (MemoryStream ms = new MemoryStream())

                {

                    //Create a buffer with the maximum allowed size

                    byte[] buffer = new byte[86];

                    int count = 0;

                    int copyLength = buffer.Length;

                    while (true)

                    {

                        //Check if the bytes left to read is smaller than the buffer size, then limit the buffer size to the number of bytes left

                        if (count + copyLength > data.Length)

                            copyLength = data.Length - count;

                        //Create a new buffer that has the correct size

                        buffer = new byte[copyLength];

                        //Copy as many bytes as the algorithm can handle at a time, iterate until the whole input array is encoded

                        Array.Copy(data, count, buffer, 0, copyLength);

                        //Start from here in next iteration

                        count += copyLength;

                        //Encrypt the data using the public key and add it to the memory buffer

                        //_DecryptionBufferSize is the size of the encrypted data

                        ms.Write(RSA.Encrypt(buffer, true), 0, 128);

                        //Clear the content of the buffer, otherwise we could end up copying the same data during the last iteration

                        Array.Clear(buffer, 0, copyLength);

                        //Check if we have reached the end, then exit

                        if (count >= data.Length)

                            break;

                    }

                    //Return the encrypted data

                    return ms.ToArray();

                }





              //  return RSA.Encrypt(data,);
            }
            return null;
        }


        public byte[] run_decrypt(byte[] data)
        {
            if (privKey != null)
            {
                CspParameters cspParams = new CspParameters();
                cspParams.ProviderType = 1; // 1 = PROV_RSA_FULL
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);
                RSA.FromXmlString(privKey);

                using (MemoryStream decrypted = new MemoryStream(data.Length))

                {

                    byte[] buffer = new byte[128];

                    int count = 0;

                    int copyLength = buffer.Length;

                    while (true)

                    {

                        //Copy a chunk of encrypted data / iteration

                        Array.Copy(data, count, buffer, 0, copyLength);

                        //Set the next start position

                        count += copyLength;

                        //Decrypt the data using the private key

                        //We need to store the decrypted data temporarily because we don't know the size of it; unlike with encryption where we know the size is 128 bytes. The only thing we know is that it's between 1-117 bytes

                        byte[] resp = RSA.Decrypt(buffer, true);

                        decrypted.Write(resp, 0, resp.Length);

                        //Clear the buffers

                        Array.Clear(resp, 0, resp.Length);

                        Array.Clear(buffer, 0, copyLength);

                        //Are we ready to exit?

                        if (count >= data.Length)

                            break;

                    }
                    return decrypted.ToArray();
                }

               
            }
            return null;
        }
    }
}

