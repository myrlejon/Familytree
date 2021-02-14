using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Familjeträd
{
    class Person
    {
        public string Förnamn { get; set; }
        public string Efternamn { get; set; }
        public int Ålder { get; set; }
        public string Stad { get; set; }
        public int Född { get; set; }
        public int Död { get; set; }
        public string Mor { get; set; }
        public string Far { get; set; }
    }
}
