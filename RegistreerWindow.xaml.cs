using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace security
{
    /// <summary>
    /// Interaction logic for RegistreerWindow.xaml
    /// </summary>
    public partial class RegistreerWindow : Window
    {

        private Window parentwindow;
        private ProgramController programma;

        public RegistreerWindow(Window parentwindow)
        {
            InitializeComponent();

            this.parentwindow = parentwindow;
            gebruikerTextBox.Focus();

            programma = new ProgramController();
        }

        private void btnRegistreer_Click(object sender, RoutedEventArgs e)
        {
            string name = gebruikerTextBox.Text;
            string password="";

            if (gebruikerTextBox.Text == "" || programma.User_exists(name))
            {
                MessageBox.Show("Kies een andere gebruiker.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            if (PasswordTextBox.Password.Equals(PasswordTextBox2.Password) && PasswordTextBox.Password != "")
                password = PasswordTextBox.Password;
            else
                MessageBox.Show("Paswoorden matchen niet.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);


            
            
            try
            {
                programma.Add_user(name, password);
            }
            catch(IOException ex)
            {
                MessageBox.Show("Er ging iets fout met het wegschrijven."+ex.Message);
            }

            MessageBox.Show("U bent succesvol geregistreerd", "Proficiat", MessageBoxButton.OK);

            ChangeWindow.CloseThisOpenParentwindow(this, parentwindow); // Terug gaan naar het login window
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChangeWindow.CloseThisOpenParentwindow(this, parentwindow); // Terug gaan naar het login window

        }
    }
}
