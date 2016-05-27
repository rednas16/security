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

namespace security
{
    /// <summary>
    /// Interaction logic for SubFunction.xaml
    /// </summary>
    public partial class SubFunction : UserControl
    {
        private string hoofdtab { get; set; }
        private string subtab { get; set; }




        public SubFunction(ProgramController program,Boolean encrypt = true )
        {
            InitializeComponent();

            subtab = "";


            TabItem destab = new TabItem();
            destab.Header = "DES";
            destab.Content = new basic_ui(encrypt, basic_ui.alg_type.DES,program);

            TabItem aestab = new TabItem();
            aestab.Header = "AES";
            aestab.Content = new basic_ui(encrypt, basic_ui.alg_type.AES, program);

            TabItem rsatab = new TabItem();
            rsatab.Header = "RSA";
            rsatab.Content = new basic_ui(encrypt, basic_ui.alg_type.RSA, program);

            tab_subFunction.Items.Add(destab);
            tab_subFunction.Items.Add(aestab);
            tab_subFunction.Items.Add(rsatab);

        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = sender as TabControl;
            var selected = item.SelectedItem as TabItem;

            if (!subtab.Equals(selected.Header.ToString()))
                subtab = selected.Header.ToString();
        }
    }
}
