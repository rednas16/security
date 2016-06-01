using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace security
{
    class FileIO
    {
        private string path;
        private static string basepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "security");
        private FileStream file;
        private StreamWriter writer;
        private StreamReader reader;
        private static int bufferlengte = 4096;
        public byte[] buffer { get; private set; }

        public FileIO(string bestand = "",string ext="")
        {
            if (bestand.Equals(""))
                return;

            if(!ext.Equals(""))
                bestand+= ext;

            path = bestand;

        }

        private void init()
        {
            if (!Directory.Exists(basepath))
                Directory.CreateDirectory(basepath);
                
            
            if (!File.Exists(path))
            {
                file = File.Create(path);
                close_file();
            }

        }

        public void close_file()
        {
            file.Dispose();
            file.Close();
        }
        
        //programma
        public List<string> Get_Users(string username)
        {
            path=Path.Combine(basepath, "login.txt");
            init();
            List<string> value = new List<string>();
            string naam = "";

            value.Add("");


            try
            {
                foreach (string line in File.ReadLines(path))
                {
                    naam = line.Split(',')[0].Trim();

                    if (!naam.Equals("") && !naam.Equals(username))
                    {
                        value.Add(naam);

                    }
                }
                return value;
            }
            catch (FileFormatException ex)
            {
                throw new IOException("bestandsprobleem: " + ex.Message);
            }

        }

        public void Add_User(string naam, string hash,string Private_key, string public_key)
        {

            path = Path.Combine(basepath, "login.txt");
            init();
            

            try
            {
                writer = File.AppendText(path);

                writer.WriteLine(naam + " , " + hash  + " , " + Private_key + " , " + public_key);
                Writer_close();
            }
            catch (FileFormatException ex)
            {
                throw new IOException("bestandsprobleem: " + ex.Message);
            }

        }


        public List<string> Get_User_Values(string naam)
        {
            path = Path.Combine(basepath, "login.txt");
            init();

            string read;
            List<string> value = new List<string>();

            try
            {
                foreach (string line in File.ReadLines(path))
                { 

                    read = line.Split(',')[0].Trim();

                    if (read.Equals(naam))
                    {
                        value.Add(line.Split(',')[1].Trim());

                        value.Add(line.Split(',')[2].Trim());
                        value.Add(line.Split(',')[3].Trim());
                        return value;
                    }
                }
                return null;
            }
            catch (FileFormatException ex)
            {
                throw new IOException("bestandsprobleem: " + ex.Message);
            }

        }


        public bool FileCheck_Program_key()
        {
            path = Path.Combine(basepath, "ProgRSAKey.txt");
            if(Directory.Exists(path))
                return File.Exists(path);

            return false;

        }


        public void Write_program_key_pair(string private_key, string public_key)
        {
            path = Path.Combine(basepath, "ProgRSAKey.txt");
            init();

            try
            {
                writer = File.AppendText(path);
                writer.WriteLine(private_key);
                writer.WriteLine(public_key);
                Writer_close();

            }
            catch (FileFormatException ex)
            {
                throw new IOException("bestandsprobleem: " + ex.Message);
            }

        }

        public List<string> Read_Program_keys()
        {
            path = Path.Combine(basepath, "ProgRSAKey.txt");
            init();

            List<string> keys = new List<string>();
            
            try
            {
                foreach (string line in File.ReadLines(path))
                {
                    keys.Add(line.Trim());
                }
                return keys;
            }
            catch (FileFormatException ex)
            {
                throw new IOException("bestandsprobleem: " + ex.Message);
            }
        }
        //einde programma

        // file read en write
        public void OpenWrite()
        {
            file = new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        public void Write_part(byte[] text)
        {
            file.Write(text, 0, text.Length);
        }

        public void OpenRead() //opend voor lezen
        {
          
            file = new FileStream(path, FileMode.Open, FileAccess.Read);

        }

        public int Read_part()
        {

            byte[] readed = new byte[bufferlengte];
            int teller= file.Read(readed, 0, readed.Length);
            buffer = new byte[teller];
            Array.Copy(readed,buffer, teller);
            Array.Clear(readed,0,bufferlengte);

            return teller;
        }

        //einde file read en write

        //writer en reader openen
        public void Open_Writer() //close file
        {
            writer = new StreamWriter(path);
        }

        public void Writer_close() //close file
        {
            writer.Dispose();
            writer.Close();
        }

        public void Open_Reader() //close file
        {
            reader = new StreamReader(path);
        }

        public void Reader_close() //close file
        {
            reader.Dispose();
            reader.Close();
        }
        //einde writer en reader openen

       
        //write encryptie
        public void Write_line(byte[] text) // schrijft bytes als hex string
        {
            writer.WriteLine(Transformer.Byte_to_string(text));
        }
        public void Write_line(string text) //schrijft tekst met enter
        {
            writer.WriteLine(text);
        }
        public void Write(string text) //schrijft tekst zonder enter
        {
            writer.Write(text);
        }
        //einde write encryptie



        //read decryptie
        public string Read_line() // fileopen, leest bestandsnaam
        {
            return reader.ReadLine();
        }

        public string read_till(string nextLine) //read till, string geeft de 
        {

            StringBuilder hulp = new StringBuilder();
            string line_readed;
            bool stop = false;

            do
            {
                line_readed = reader.ReadLine();

                if (!line_readed.Equals(nextLine) && !line_readed.Equals(""))
                    hulp.Append(line_readed);
                else
                    stop = true;
            } while (!stop);

            return hulp.ToString();

        }

        public int readCode() //lees encrypted code
        {
            bool stop = false;
            StringBuilder hulp = new StringBuilder();

            string line_readed;

    
                line_readed = reader.ReadLine();

                if (!line_readed.Equals("HASH input:") && !line_readed.Equals(""))
                {
                    hulp.Append(line_readed);
                }
                else
                    stop = true;

            if (!stop)
            {
                buffer = Transformer.String_to_byte(hulp.ToString());

                return buffer.Length;
            }
            else
                return 0;

           
        }

    }

}
