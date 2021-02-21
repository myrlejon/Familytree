using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Familjeträd
{
    class Program : CRUD
    {
        /// <summary>
        /// Main metoden innehåller två metoder: Intro och Meny. Du kan läsa mer om dessa metoder i CRUD.cs klassen
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Intro();
            Meny();
        }
    }
}