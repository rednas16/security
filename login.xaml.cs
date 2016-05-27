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
using System.Windows.Shapes;

namespace security
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        ProgramController program;

        public login()
        {
            InitializeComponent();
            UsernameTextBox.Focus();
            program = new ProgramController();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string naam = "";
            string wachtwoord = "";
            
            if (UsernameTextBox.Text != null && PasswordTextBox.Password != null)
            { 
                naam = UsernameTextBox.Text;
                wachtwoord = PasswordTextBox.Password;

                if (program.checkLogin(naam,wachtwoord)) 
                {
                    

                        ChangeWindow.CloseThisOpenNext(this, new MainWindow(program));
                        
                    
                

                }
                else
                    MessageBox.Show("Uw logingegevens werden niet gevonden.");
            }


        }

        private void RegistreerButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeWindow.CloseThisOpenNext(this, new RegistreerWindow(new login())); 

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
