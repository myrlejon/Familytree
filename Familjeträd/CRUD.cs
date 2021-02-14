using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Familjeträd
{
    class CRUD : Databas
    {
        public static void Intro()
        {
            var db = new Databas();
            if (!db.DoesDatabasExist(db.Familjeträd))
            {
                Console.WriteLine("Skapar databas.");
                db.CreateDatabase(db.Familjeträd, "master");
                db.UseDatabase(db.Familjeträd, db.Familjeträd);
                db.CreateDatatable("Personer", @"
                            ID int NOT NULL Identity (1,1),
                            förnamn varchar(50),
                            efternamn varchar(50),
                            ålder int,
                            stad varchar (50),
                            född int,
                            död int,
                            mor varchar (50),
                            far varchar (50)", db.Familjeträd);
                Console.WriteLine("Skapade databasen.\n");
                Console.ReadLine();
            }
            else if (db.DoesDatabasExist(db.Familjeträd))
            {
                Console.WriteLine("Kör menyn.");
            }
        }

        public static void Meny()
        {
            var db = new Databas();
            var crud = new CRUD();
            Console.WriteLine($"Aktuell databas: {db.Familjeträd}");
            Console.WriteLine("\n\n(1) Skapa en ny person.\n(2) Redigera en person.\n(3) Radera en person.\n(4) Lista upp alla personer.");
            Console.WriteLine("(5) Visa mor/far till en person.\n(6) Visa syskon till en person.");
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                var person = new Person
                {
                    Förnamn = "Nils",
                    Efternamn = "Odén",
                    Ålder = 21,
                    Stad = "Onsala",
                    Född = 1999,
                    Död = 2090,
                    Mor = "Åsa",
                    Far = "Svante"
                };
                crud.Create(person);
            }
        }

        internal void Create(Person person)
        {
            var db = new Databas();
            var sqlString = "INSERT INTO Personer (Förnamn, Efternamn, Ålder, Stad, Född, Död, Mor, Far) VALUES(@Förnamn, @Efternamn, @Ålder, @Stad, @Född, @Död, @Mor, @Far)";
            try
            {
                var connString = string.Format(db.ConnectionString, db.Familjeträd);
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(sqlString, conn);
                    cmd.Parameters.AddWithValue("@Förnamn", person.Förnamn);
                    cmd.Parameters.AddWithValue("@Efternamn", person.Efternamn);
                    cmd.Parameters.AddWithValue("@Ålder", person.Ålder);
                    cmd.Parameters.AddWithValue("@Stad", person.Stad);
                    cmd.Parameters.AddWithValue("@Född", person.Född);
                    cmd.Parameters.AddWithValue("@Död", person.Död);
                    cmd.Parameters.AddWithValue("@Mor", person.Mor);
                    cmd.Parameters.AddWithValue("@Far", person.Far);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception except)
            {
                System.Console.WriteLine(except.Message);
            }
        }
    }
}
