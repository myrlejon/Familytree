using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Familjeträd
{
    /// <summary>
    /// Denna klassen används för att ändra värden i programmet för att sedan skicka tillbaka dom till SQL databasen med hjälp av parametrar.
    /// </summary>
    class Person
    {
        public int ID { get; set; }
        public string Förnamn { get; set; }
        public string Efternamn { get; set; }
        public int Ålder { get; set; }
        public string Födelseland { get; set; }
        public string Födelsestad { get; set; }
        public int Född { get; set; }
        public int Död { get; set; }
        public string Dödsland { get; set; }
        public string Dödsstad { get; set; }
        public string Mor { get; set; }
        public string Far { get; set; }
        public int MorID { get; set; }
        public int FarID { get; set; }
        public int BarnID { get; set; }
    }
}
