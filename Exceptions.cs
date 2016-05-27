using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace security
{
    class Exceptions: Exception
    {
        public Exceptions(string message)
        : base(message)
        {


        }

        public Exceptions(string message, string id)
        {


        }
    }
}
