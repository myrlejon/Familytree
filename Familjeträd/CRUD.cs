﻿using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

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
                            far varchar (50),
                            morID int,
                            farID int,
                            barnID int", db.Familjeträd);
                Console.WriteLine($"Skapade databasen {db.Familjeträd}");
            }
            else if (db.DoesDatabasExist(db.Familjeträd))
            {
                Console.WriteLine("Kör menyn.");
                Console.WriteLine($"Aktuell databas: {db.Familjeträd}");
            }
        }

        public static void Meny()
        {
            bool menyLoop = true;
            while (menyLoop)
            {
                var db = new Databas();
                var crud = new CRUD();
                Console.WriteLine("\n(1) Skapa en ny person.\n(2) Redigera en person.\n(3) Sök på en person.\n(4) Lista upp alla personer.");
                Console.WriteLine("(5) Visa mor/far till en person.\n(6) Visa syskon till en person.\n(7) Radera en person.\n");
                string choice = Console.ReadLine();
                
                if (choice == "1")
                {
                    crud.CreatePerson();
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Vem vill du redigera?");
                    string namn = Console.ReadLine();
                    Console.WriteLine("Vad vill du ändra?");
                    Console.WriteLine("(1) Förnamn (2) Efternamn (3) Ålder (4) Stad (5) Födelsedatum (6) Dödsår (7) Mor (8) Far (9) Barn");
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Vem vill du söka på?\n");
                    string namn = Console.ReadLine();
                    var person = crud.Read(namn);
                    Print(person);
                }
                else if (choice == "4")
                {

                }
                else if (choice == "5")
                {

                }
            }
        }

        public void Create(Person person)
        {
            var db = new Databas();
            var sqlString = "INSERT INTO Personer (Förnamn, Efternamn, Ålder, Stad, Född, Död, Mor, Far, MorID, FarID, BarnID) VALUES(@Förnamn, @Efternamn, @Ålder, @Stad, @Född, @Död, @Mor, @Far, @MorID, @FarID, @BarnID)";
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
                    cmd.Parameters.AddWithValue("@MorID", person.MorID);
                    cmd.Parameters.AddWithValue("@FarID", person.FarID);
                    cmd.Parameters.AddWithValue("@BarnID", person.BarnID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception except)
            {
                System.Console.WriteLine(except.Message);
            }
        }

        public void CreateParent(string parentFirstName, string parentLastName, int barnID)
        {
            var db = new Databas();
            var sqlString = "INSERT INTO Personer (Förnamn, Efternamn, BarnID) VALUES(@Förnamn, @Efternamn, @BarnID)";
            try
            {
                var connString = string.Format(db.ConnectionString, db.Familjeträd);
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(sqlString, conn);
                    cmd.Parameters.AddWithValue("@Förnamn", parentFirstName);
                    cmd.Parameters.AddWithValue("@Efternamn", parentLastName);
                    cmd.Parameters.AddWithValue("@BarnID", barnID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception except)
            {
                System.Console.WriteLine(except.Message);
            }
        }

        public void Update(Person person)
        {
            var db = new Databas();
            db.SQL(@"UPDATE Personer SET Förnamn = @Förnamn, Efternamn = @Efternamn, 
                     Ålder = @Ålder, Stad = @Stad, Född = @Född, Död = @Död, Mor = @Mor,
                     Far = @Far, MorID = @MorID, FarID = @FarID, BarnID = @BarnID WHERE ID = @ID", db.Familjeträd,
                     ("@Förnamn", person.Förnamn),
                     ("@Efternamn", person.Efternamn),
                     ("@Ålder", person.Ålder.ToString()),
                     ("@Stad", person.Stad),
                     ("@Född", person.Född.ToString()),
                     ("@Död", person.Död.ToString()),
                     ("@Mor", person.Mor),
                     ("@Far", person.Far),
                     ("@ID", person.ID.ToString()),
                     ("MorID", person.MorID.ToString()),
                     ("FarID", person.FarID.ToString()),
                     ("BarnID", person.BarnID.ToString())
                     );
        }

        public Person Read(string name)
        {
            var db = new Databas();
            DataTable dt;

            if (name.Contains(" "))
            {
                var names = name.Split(' ');
                var lastName = names[names.Length - 1];
                dt = db.GetDataTable("SELECT TOP 1 * FROM Personer WHERE Förnamn LIKE @Förnamn AND Efternamn LIKE @Efternamn", Familjeträd,
                    ("@Förnamn", names[0]), ("@Efternamn", lastName));
            }
            else
            {
                dt = db.GetDataTable("SELECT TOP 1 * FROM Personer WHERE Förnamn LIKE @name OR Efternamn LIKE @name ", Familjeträd, ("@name", name));
            }

            if (dt.Rows.Count == 0)
                return null;

            return GetPersonObject(dt.Rows[0]);
        }

        private static void Print(Person person)
        {
            if (person != null)
            {
                Console.WriteLine($"{person.Förnamn} {person.Efternamn}, är {person.Ålder} år och bor i {person.Stad}.");
                Console.WriteLine($"{person.Förnamn} föddes {person.Född} och dog {person.Död}.");
                Console.WriteLine($"Far: {person.Far} {person.Efternamn}\nMor: {person.Mor} {person.Efternamn}");
            }
            else
            {
                Console.WriteLine("Person not found");
            }
        }

        private static Person GetPersonObject(DataRow row)
        {
            return new Person
            {
                Förnamn = row["Förnamn"].ToString(),
                Efternamn = row["Efternamn"].ToString(),
                Ålder = (int)row["Ålder"],  
                Stad = row["Stad"].ToString(),
                Född = (int)row["Född"],
                Död = (int)row["Död"],
                Mor = row["Mor"].ToString(),
                Far = row["Far"].ToString(),
                ID = (int)row["ID"],
                MorID = (int)row["MorID"],
                FarID = (int)row["FarID"],
                BarnID = (int)row["BarnID"]
            };
        }


        //public void Delete(string name)
        //{
        //    var person = Read(name);
        //    if (person != null Delete(person));
        //}

        //public Person GetPersonID(string förnamn, string efternamn)
        //{
        //    var db = new Databas();
        //    DataTable dt;
        //    db.SQL(@"SELECT TOP 1 * FROM Personer WHERE Förnamn = @Förnamn AND Efternamn = @Efternamn", Familjeträd);

        //    if (dt.Rows.Count == 0)
        //        return null;

        //    return GetPersonObject(dt.Rows[0]);
        //}

        public void CreatePerson()
        {
            var crud = new CRUD();
            var person = new Person();
            Console.WriteLine("Mata in värden för personen.");
            Console.WriteLine("Förnamn: ");
            person.Förnamn = Console.ReadLine();
            Console.WriteLine("Efternamn: ");
            person.Efternamn = Console.ReadLine();
            Console.WriteLine("Ålder: ");
            string ålderInput = Console.ReadLine();
            person.Ålder = Convert.ToInt32(ålderInput);
            Console.WriteLine("Stad: ");
            person.Stad = Console.ReadLine();
            Console.WriteLine("Födelseår: ");
            string föddInput = Console.ReadLine();
            person.Född = Convert.ToInt32(föddInput);
            Console.WriteLine("Dödsår: ");
            string dödInput = Console.ReadLine();
            person.Död = Convert.ToInt32(dödInput);
            Console.WriteLine("Mor: ");
            person.Mor = Console.ReadLine();
            Console.WriteLine("Far: ");
            person.Far = Console.ReadLine();
            crud.Create(person);
            var readPerson = crud.Read($"{person.Förnamn} {person.Efternamn}");
            person.ID = readPerson.ID;
            person.MorID = readPerson.ID + 1;
            crud.CreateParent(person.Mor, person.Efternamn, readPerson.ID);
            person.FarID = person.MorID + 1;
            crud.CreateParent(person.Far, person.Efternamn, readPerson.ID);

            crud.Update(person);

            //Skapa en Read för parent, som inte har alla variabler inmatade
            //TODO: if barnID > 0 så skapa en ny read funktion för parent?
        }
    }
}
