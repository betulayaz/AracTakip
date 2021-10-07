using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takip
{
    class connection
    {
        public string Address = System.IO.File.ReadAllText(@"C:\AracTakip.txt");
    }
}
