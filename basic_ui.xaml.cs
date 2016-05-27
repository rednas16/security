
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace security
{
    /// <summary>
    /// Interaction logic for basic_ui.xaml
    /// </summary>
    public partial class basic_ui : UserControl
    {
        private bool encrypt;
        private alg_type algtype;

        private string source, destination;
        ProgramController programma;




        public enum alg_type { DES,AES,RSA}

        

        public basic_ui(bool hoofdtab, alg_type algtype, ProgramController program)
        {
            InitializeComponent();
            btnAction.IsEnabled = false;
            this.encrypt = hoofdtab;
            this.algtype = algtype;
            this.programma = program;

            if (!encrypt)
            {
                lblsentto.Content = "Komt van: ";
            }

            if (algtype == alg_type.RSA)
            {
                lblsentto.Visibility = Visibility.Visible;
                cbbsentto.Visibility = Visibility.Visible;
                List<string> value = programma.Get_Users();

                if (value != null)
                    cbbsentto.ItemsSource = value;
            }
            else {

                cbbsentto.Visibility = Visibility.Hidden;
                lblsentto.Visibility = Visibility.Hidden;
            }

        }


        private void btnSourceFile_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog chooseFile = new OpenFileDialog();
            chooseFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if(encrypt)
                chooseFile.Title = "Kies een bestand om te encrypten.";
            else
                chooseFile.Title = "Kies een geëncrypteerd bestand om te decrypten.";

            if (chooseFile.ShowDialog() == true)
            {

                txtSource.Text = chooseFile.FileName;
                txtSource.IsEnabled = false;
            }

        }

        private void btnDestinationFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog chooseFile = new OpenFileDialog();
            chooseFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            chooseFile.Title = "Kies een doelbestand";


            if (chooseFile.ShowDialog() == true )
            {
                
                txtDestination.Text = chooseFile.FileName;

                if(encrypt)
                    txtDestination.IsEnabled = false;
            }


        }

        private void txtDestination_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(txtDestination.Text))
            {
                if(encrypt)
                    MessageBox.Show("Pas op, het doelbestand wordt onbruikbaar.");
                else
                    MessageBox.Show("Pas op, het doelbestand wordt herschreven.");

            }
            txtDestination.SetValue(BorderBrushProperty, Brushes.Green);
            destination = txtDestination.Text;

        }

        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(txtSource.Text))
            {
                if (encrypt)
                {
                    txtSource.SetValue(BorderBrushProperty, Brushes.Green);
                    FileInfo info = new FileInfo(txtSource.Text);

                    txtDestination.Text = info.Directory + "\\" + System.IO.Path.GetFileNameWithoutExtension(info.Name) + "_encrypted" + info.Extension;


                    lblext.Content = "Extensie: " + info.Extension;
                    lblGrootte.Content = "Grootte: " + BytesToString(info.Length);
                }
                btnAction.IsEnabled = true;
                source = txtSource.Text;
            }
            else
            {
                txtSource.SetValue(BorderBrushProperty, Brushes.Red);
                btnAction.IsEnabled = false;
            }

        }
        
            private String BytesToString(long aantal)
            {
                string[] grootes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
                if (aantal == 0)
                    return "0" + grootes[0];
                long bytes = Math.Abs(aantal);
                int teller = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, teller), 1);
                return (Math.Sign(aantal) * num).ToString() + grootes[teller];
            }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {


            if (encrypt)
            {
                if (algtype == alg_type.DES) {
                    //programma.TDESencrypt(source, destination);


                }
                if (algtype == alg_type.AES)
                {
                    programma.encryptSym(source, destination);

                }
                if (algtype == alg_type.RSA)
                {
                   
                }
                MessageBox.Show("Succesvol geëncrypteerd!");
            }
            if (!encrypt) {
                if (algtype == alg_type.DES)
                {
                    //programma.TDESdecrypt(source, destination);
                }
                if (algtype == alg_type.AES)
                {
                    programma.decryptSym(source, destination);


                }
                if (algtype == alg_type.RSA)
                {
                    string andere_persoon = cbbsentto.
                }

                MessageBox.Show("Succesvol gedecrypteerd!");
            }
        }
    }
        
    
    
}
