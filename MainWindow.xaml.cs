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

using System.Security.Cryptography;


namespace security
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string hoofdtab;

        public MainWindow(ProgramController program)
        {
            InitializeComponent();

            hoofdtab = "";




            TabItem encrypt = new TabItem();
            encrypt.Header = "Encrypt";
            encrypt.Content = new SubFunction(program);

            TabItem decrypt = new TabItem();
            decrypt.Header = "Decrypt";
            decrypt.Content = new SubFunction(program,false);


            tab_function.Items.Add(encrypt);
            tab_function.Items.Add(decrypt);


        }
        

        private void tab_function_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var item = sender as TabControl;
            var selected = item.SelectedItem as TabItem;

            if (!hoofdtab.Equals(selected.Header.ToString()))
            {

                this.Title = hoofdtab = selected.Header.ToString();
            }

            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
