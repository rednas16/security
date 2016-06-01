using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace security
{
    public class ProgramController
    {
        public string private_rsa_key { get; private set; }
        public string public_rsa_key { get; private set; }
        private RSA program_encryptor;
        private string username;
        private string user_public_key;
        private string user_private_key;
        private FileIO bestand;
        private FileIO input, output;
        private hasher hashing;
        private hasher inputHasher;
        private hasher outputHasher;


        private Queue<byte[]> stack;




        public ProgramController()
        {
            bestand = new FileIO();
            Get_prog_keys();

            program_encryptor = new RSA();

            program_encryptor.SetPrivKey(private_rsa_key);
            program_encryptor.SetPubKey(public_rsa_key);



        }

        private void Get_prog_keys()
        {
            if (!bestand.FileCheck_Program_key())
                Generate_Prog_keys();

           List<string> keys= bestand.Read_Program_keys();

            private_rsa_key = keys[0];
            public_rsa_key = keys[1];

        }

        private void Generate_Prog_keys()
        {
           
            RSA rsaencryptor = new RSA();
            rsaencryptor.generateNewKeyPair();
            bestand.Write_program_key_pair(rsaencryptor.privKey, rsaencryptor.pubKey) ;

        }

        public List<string> get_user(string naam) {
            List<string> userValues = bestand.Get_User_Values(naam);
            if (userValues!=null)
                return userValues;

            return null;
        }

        public bool checkLogin(string naam, string wachtwoord)
        {
            hashing = new hasher();
            List<string> userValues = get_user(naam);

            if (userValues == null)
                return false;


            if (!hashing.check_hash(Transformer.GetByteValue(wachtwoord), userValues[0]))
                return false;

            username = naam;
            user_private_key = userValues[1];
            user_public_key = userValues[2];

            return true;

        }
        public bool User_exists(string name)
        {

            if (bestand.Get_User_Values(name) != null)
                return true;

            return false;

        }
        public List<string> Get_Users()
        {

            return bestand.Get_Users(username);

        }
        

        public void Add_user(string name, string password)
        {
            hashing = new hasher();

            password = hashing.Hash_met_salt(Transformer.GetByteValue(password));

            RSA rsaencryptor = new RSA();
            
            rsaencryptor.generateNewKeyPair();
            bestand.Add_User(name,password, rsaencryptor.privKey, rsaencryptor.pubKey);

        }

        private void writeBegin(string fileName, byte[] key, byte[] iv)
        {
            output.Open_Writer();
            output.Write_line(fileName);
            output.Write_line("KEY:");
            output.Write_line(key);
            output.Write_line("IV:");
            output.Write_line(iv);
            output.Write_line("CODE:");
        }

        private void writeEnd()
        {
            output.Write_line("HASH input:");
            output.Write_line(inputHasher.Hash_file());
            output.Write_line("HASH output:");
            output.Write_line(outputHasher.Hash_file());
        }

        public void ReadBegin(ref string filename, ref byte[] key, ref byte[] IV) // fileopen, leest bestandsnaam, key en iv
        {
            input.Open_Reader();
            filename = input.read_till("KEY:");

            key = Transformer.String_to_byte(input.read_till("IV:"));

            IV = Transformer.String_to_byte(input.read_till("CODE:"));


        }
        public void ReadEnd(ref string hashInput, ref string hashOutput)
        {
            hashInput = input.read_till("HASH output:");
            hashOutput = input.Read_line();
            input.Reader_close();
        }


        public bool encryptSym(string source, string destination)
        {
            AesEncryptie symEncryptor = new AesEncryptie();
            inputHasher= new hasher();
            outputHasher= new hasher();


            FileInfo info = new FileInfo(source);
            input = new FileIO(source);
            output = new FileIO(destination);

            try {
                writeBegin(info.Name, program_encryptor.run_encrypt(symEncryptor.KEY), program_encryptor.run_encrypt(symEncryptor.IV));
            }
            catch(FileNotFoundException ex)
            {
                throw new Exceptions("File not found:" + ex.FileName, "Encryptie");

            }
            catch (FileLoadException ex)
            {
                throw new Exceptions("Geen toegangsrechten:" + ex.FileName, "Encryptie");

            }


            int count;
            byte[] hulp;

            input.OpenRead();

            while ((count = input.Read_part()) > 0)
            {
                inputHasher.Add_part(input.buffer);
                hulp = symEncryptor.encrypt(input.buffer);
                outputHasher.Add_part(hulp);
                output.Write_line(hulp);
            }
            symEncryptor.end_encryption();
            input.close_file();

            writeEnd();
            output.Writer_close();
            return true;
        }

        


        public bool decryptSym(string source, string destination)
        {
            AesEncryptie symEncryptor;
            inputHasher = new hasher();
            outputHasher = new hasher();
            
            string filename="";
            byte[] key = new byte[0];
            byte[] IV = new byte[0];

            input = new FileIO(source);

            try
            {
                ReadBegin(ref filename, ref key, ref IV);
            }
            catch (Exceptions ex)
            {
                throw new Exceptions("Wegschrijven lukte niet:" + ex.Message, "Decryptie");
            }

            try
            {
                FileInfo info = new FileInfo(filename);
                FileInfo infoDestination = new FileInfo(destination);
            if(!infoDestination.Extension.Equals(info.Extension))
                output = new FileIO(destination,info.Extension);
            else
                output = new FileIO(destination);

            symEncryptor =  new AesEncryptie(program_encryptor.run_decrypt(IV), program_encryptor.run_decrypt(key));
          
                
            byte[] hulp;

            output.OpenWrite();

            while ( input.readCode() > 0)
            {
                inputHasher.Add_part(input.buffer);
                hulp = symEncryptor.encrypt(input.buffer);
                    
                outputHasher.Add_part(hulp);
                output.Write_part(hulp);
            }

                symEncryptor.end_encryption();
                Check_Hashes();
            }
            catch(Exceptions ex)
            {
                throw new Exceptions("Er is geknoeit met de input files:" + ex.Message,"Decryptie");
            }
            output.close_file();

            return true;
            
        }
        /*
        hash controle
        */
        private void Check_Hashes()
        {

            string hash_original_input = "";
            string hash_original_output = "";

            ReadEnd(ref hash_original_input, ref hash_original_output);

            if (!inputHasher.Check_file(hash_original_output))
                throw new Exceptions("Het originele bestand is gewijzigd.");
            if (!outputHasher.Check_file(hash_original_input))
                throw new Exceptions("Het geëncrypteerde bestand is gewijzigd.");


        }

        public bool encryptrsa(string source, string destination,string key_friend)
        {
            RSA encryptor = new RSA();
            RSA friendEncryptor=new RSA();

            if (key_friend.Equals(""))
            {
                encryptor.SetPubKey(user_public_key);
            }
            else
            {
                encryptor.SetPubKey(user_private_key);
                friendEncryptor.SetPubKey(key_friend);
                
            }


            inputHasher = new hasher();
            outputHasher = new hasher();


            FileInfo info = new FileInfo(source);
            input = new FileIO(source);
            output = new FileIO(destination);

            output.Open_Writer();
            output.Write_line(info.Name);
            output.Write_line("CODE:");

            int count;
            byte[] hulp;

            input.OpenRead();

            while ((count = input.Read_part()) > 0)
            {
                inputHasher.Add_part(input.buffer);
                hulp = encryptor.run_encrypt(input.buffer);

                if (!key_friend.Equals(""))
                    //hulp = friendEncryptor.run_encrypt(hulp);

                outputHasher.Add_part(hulp);
                output.Write_line(hulp);

            }


            input.close_file();

            writeEnd();
            output.Writer_close();

            return true;
        }
        public bool decryptrsa(string source, string destination, string key_friend)
        {

            RSA encryptor = new RSA();
            RSA friendEncryptor = new RSA();


            encryptor.SetPrivKey(user_private_key);

            if (!key_friend.Equals("")) {
                friendEncryptor.SetPrivKey(key_friend);
            }

            inputHasher = new hasher();
            outputHasher = new hasher();

            input = new FileIO(source);

            input.Open_Reader();
            string filename = input.read_till("CODE:");

            FileInfo info = new FileInfo(filename);
            FileInfo infoDestination = new FileInfo(destination);
            if (!infoDestination.Extension.Equals(info.Extension))
                output = new FileIO(destination, info.Extension);
            else
                output = new FileIO(destination);
            
            byte[] hulp;

            output.OpenWrite();

            while (input.readCode() > 0)
            {
                inputHasher.Add_part(input.buffer);

                if (!key_friend.Equals(""))
                {
                    hulp = friendEncryptor.run_decrypt(input.buffer);
                    hulp = encryptor.run_decrypt(hulp);
                }
                else
                    hulp = encryptor.run_decrypt(input.buffer);

                outputHasher.Add_part(hulp);
                output.Write_part(hulp);
            }

            Check_Hashes();

            output.close_file();

            return true;
        }





        /*
        public bool TDESencrypt(string source, string destination)
        {
            symEncrypt<TripleDESCryptoServiceProvider> symEncryptor = new symEncrypt<TripleDESCryptoServiceProvider>(true);
            FileInfo info = new FileInfo(source);
            input = new FileIO(source);
            output = new FileIO(destination);
            hasher inputHasher = new hasher();
            hasher outputHasher = new hasher();

            try
            {
                writeBegin(info.Name, program_encryptor.run_encrypt(symEncryptor.KEY), program_encryptor.run_encrypt(symEncryptor.IV));
            }
            catch (FileNotFoundException ex)
            {
                throw new Exceptions("File not found:" + ex.FileName, "Encryptie");

            }
            catch (FileLoadException ex)
            {
                throw new Exceptions("Geen toegangsrechten:" + ex.FileName, "Encryptie");

            }


            int count;
            byte[] hulp;

            input.open_for_read();

            while ((count = input.read_part()) > 0)
            {
                inputHasher.add_part(input.buffer, count);
                hulp = symEncryptor.encrypt(input.buffer);
                outputHasher.add_part(hulp, hulp.Length);
                output.Write_bytes(hulp);
            }

            writeEnd(inputHasher.hash_file(), outputHasher.hash_file());



            input.close_file();
            output.writer_close();



            return true;
        }
        public bool TDESdecrypt(string source, string destination)
        {
            symEncrypt<TripleDESCryptoServiceProvider> symEncryptor;
            input = new FileIO(source);


            hasher inputHasher = new hasher();
            hasher outputHasher = new hasher();

            string filename = "";
            byte[] key = new byte[0];
            byte[] IV = new byte[0];

            try
            {
                input.read_Begin_Text(ref filename, ref key, ref IV);
            }
            catch (Exceptions ex)
            {
                throw new Exceptions("Wegschrijven lukte niet:" + ex.Message, "Decryptie");
            }

            try
            {
                FileInfo info = new FileInfo(filename);
                FileInfo infoDestination = new FileInfo(destination);
                if (!infoDestination.Extension.Equals(info.Extension))
                    output = new FileIO(destination, info.Extension);
                else
                    output = new FileIO(destination);

              symEncryptor = new symEncrypt<TripleDESCryptoServiceProvider>(true);


                int count;
                byte[] hulp;

                output.Open_for_write();

                while ((count = input.readCode()) > 0)
                {
                    inputHasher.add_part(input.buffer, count);
                    hulp = symEncryptor.encrypt(input.buffer);
                    outputHasher.add_part(hulp, hulp.Length);
                    output.Write_part(hulp);
                }


                checked_salts(ref inputHasher, ref outputHasher);
            }
            catch (Exceptions ex)
            {
                throw new Exceptions("Er is geknoeit met de input files:" + ex.Message, "Decryptie");
            }

            output.close_file();

            return true;

        }*/
    }
}


