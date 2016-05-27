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

                return RSA.Encrypt(data, true);
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

                return RSA.Decrypt(data, true);
            }
            return null;
        }
    }
}

