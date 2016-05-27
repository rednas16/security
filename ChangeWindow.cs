using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace security
{
    class ChangeWindow
    {
        public static void CloseThisOpenNext(Window toClose, Window toOpen)
        {
            toClose.Hide();
            toOpen.Show();
        }

        public static void CloseThisOpenParentwindow(Window toClose, Window parentwindow)
        {
            CloseThisOpenNext(toClose, parentwindow);
        }
    }
}
