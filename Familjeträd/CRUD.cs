using System;
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

        // TODO: Ta bort onödiga metoder
        
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
                            förnamn nvarchar(50),
                            efternamn nvarchar(50),
                            ålder int,
                            stad nvarchar(50),
                            född int,
                            död int,
                            mor nvarchar(50),
                            far nvarchar(50),
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
                Console.WriteLine("\n(1) Skapa en ny person.\n(2) Redigera ett värde hos en person.\n(3) Sök på en person.\n(4) Lista upp alla personer.");
                Console.WriteLine("(5) Uppdatera alla värden på en person\n(6) Visa syskon till en person.\n(7) Radera en person.\n");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    crud.CreatePerson();
                }
                else if (choice == "2")
                {
                    crud.EditPerson();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Vem vill du söka på?");
                    string namn = Console.ReadLine();
                    var person = crud.Read(namn);
                    Print(person);
                }
                else if (choice == "4")
                {

                }
                else if (choice == "5")
                {
                    crud.UpdateValues();
                }
                else if (choice == "6")
                {

                }
                else if (choice == "7")
                {
                    Console.WriteLine("Skriv in namnet på personen du vill radera.");
                    var delInput = Console.ReadLine(); 
                    crud.Delete(delInput);
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
            db.SQL(@"UPDATE Personer SET Förnamn = @Förnamn, Efternamn = @Efternamn, Ålder = @Ålder,
                    Stad = @Stad, Född = @Född, Död = @Död, Mor = @Mor, Far = @Far, MorID = @MorID, 
                    FarID = @FarID, BarnID = @BarnID WHERE ID = @ID", db.Familjeträd,
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

        public void UpdateParent(Person person)
        {
            var db = new Databas();
            db.SQL(@"UPDATE Personer SET Förnamn = @Förnamn, Efternamn = @Efternamn, BarnID = @BarnID WHERE BarnID = @BarnID", db.Familjeträd,
                  ("@Förnamn", person.Förnamn),
                  ("@Efternamn", person.Efternamn),
                  ("BarnID", person.BarnID.ToString()));
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

        public Person ReadID(int ID)
        {
            var db = new Databas();
            DataTable dt;

            dt = db.GetDataTable("SELECT TOP 1 * FROM Personer WHERE ID = @ID", Familjeträd, ("@ID", ID.ToString()));

            if (dt.Rows.Count == 0)
                return null;

            return GetPersonObject(dt.Rows[0]);
        }

            private static void Print(Person person)
        {
            if (person != null)
            {
                if (person.Ålder == 0)
                {
                    Console.WriteLine($"Du har inte angett alla värden till {person.Förnamn} {person.Efternamn}.\nAnvänd menyval (5) för att lägga till mer information!");
                }
                else
                {
                    Console.WriteLine($"{person.Förnamn} {person.Efternamn}, är {person.Ålder} år och bor i {person.Stad}.");
                    Console.WriteLine($"{person.Förnamn} föddes {person.Född} och dog {person.Död}.");
                    Console.WriteLine($"Far: {person.Far} {person.Efternamn}\nMor: {person.Mor} {person.Efternamn}");
                }
            }
            else
            {
                Console.WriteLine("Person not found");
            }
        }

        private static bool DoesPersonExist(Person person)
        {
            if (person != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // TODO: Skapa en GetPersonObjekt för föräldrar, så att personer som har rader med NULL kan läsas in ändå.
        private static Person GetPersonObject(DataRow row)
        {
            try
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
            catch
            {
                return new Person
                {
                    Förnamn = row["Förnamn"].ToString(),
                    Efternamn = row["Efternamn"].ToString(),
                    BarnID = (int)row["BarnID"]
                };
            }
        }


        public void Delete(string namn)
        {
            var crud = new CRUD();
            var db = new Databas();
            var person = Read(namn);

            if (person != null)
            {
                var mor = Read($"{person.Mor} {person.Efternamn}");
                var far = Read($"{person.Far} {person.Efternamn}");
                var barn = ReadID(person.BarnID);

                if (DoesPersonExist(mor) == true)
                {
                    barn.Mor = " ";
                    barn.MorID = 0;
                    mor.BarnID = 0;

                    crud.Update(mor);
                    crud.Update(barn);
                }
                else if (DoesPersonExist(far) == true)
                {
                    //barn.Far = " ";
                    //barn.FarID = 0;
                    far.BarnID = 0;

                    crud.Update(far);
                    crud.Update(barn);
                }
                else if (DoesPersonExist(barn) == true)
                {
                    if (barn.MorID == person.ID)
                    {
                        barn.MorID = 0;
                        barn.Mor = " ";
                        crud.Update(barn);
                    }
                    else if (barn.FarID == person.ID)
                    {
                        barn.FarID = 0;
                        barn.Far = " ";
                        crud.Update(barn);
                    }
                }



                db.SQL("DELETE FROM Personer WHERE Förnamn = @Förnamn AND Efternamn = @Efternamn", Familjeträd, 
                      ("@Förnamn", person.Förnamn),
                      ("@Efternamn", person.Efternamn));
                Console.WriteLine($"Personen har raderats ifrån databasen.");
            }
            else if (person == null)
            {
                Console.WriteLine("Personen finns inte i databasen.");
            }
        }

        //public Person GetPersonID(string förnamn, string efternamn)
        //{
        //    var db = new Databas();
        //    DataTable dt;
        //    db.SQL(@"SELECT TOP 1 * FROM Personer WHERE Förnamn = @Förnamn AND Efternamn = @Efternamn", Familjeträd);

        //    if (dt.Rows.Count == 0)
        //        return null;

        //    return GetPersonObject(dt.Rows[0]);
        //}

        public void InputValues(Person person)
        {
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
        }
        
        public void UpdateValues()
        {
            var crud = new CRUD();
            var person = new Person();
            Console.WriteLine("Vem vill du uppdatera?\n");
            var input = Console.ReadLine();
            person = crud.Read(input);

            if (DoesPersonExist(person))
            {
                InputValues(person);
                Console.WriteLine($"{person.Ålder}");
                Update(person);
                Console.WriteLine("Personen har blivit uppdaterad.");
            }
            else if (DoesPersonExist(person) == false)
            {
                Console.WriteLine("Personen finns inte i databasen.");
            }
        }

        public void CreatePerson()
        {
            var crud = new CRUD();
            var person = new Person();
            var far = new Person();
            var mor = new Person();
            InputValues(person);
            crud.Create(person);
            var readPerson = crud.Read($"{person.Förnamn} {person.Efternamn}");
            person.ID = readPerson.ID;
            person.MorID = readPerson.ID + 1;
            person.FarID = person.MorID + 1;

            mor.Förnamn = person.Mor;
            mor.Efternamn = person.Efternamn;
            mor.BarnID = person.ID;
            DefaultValue(mor);
            crud.Create(mor);

            far.Förnamn = person.Far;
            far.Efternamn = person.Efternamn;
            far.BarnID = person.ID;
            DefaultValue(far);
            crud.Create(far);

            crud.Update(person);

            //Skapa en Read för parent, som inte har alla variabler inmatade
            //TODO: if barnID > 0 så skapa en ny read funktion för parent?
        }

        //Man kan ställa in defaultvärden när man skapar databasen men denna lösningen fungerade utmärkt så bestämde mig för att inte ändra på det.
        public void DefaultValue(Person person)
        {
            person.Ålder = 0;
            person.Stad = " ";
            person.Född = 0;
            person.Död = 0;
            person.Mor = " ";
            person.Far = " ";
        }


        public void EditPerson()
        {
            var crud = new CRUD();
            Console.WriteLine("Vem vill du redigera?");
            var namn = Console.ReadLine();
            var person = crud.Read(namn);

            if (person == null)
            {
                Console.WriteLine("\nPersonen existerar inte.");
            }
            else if (person != null)
            {
                Console.WriteLine("\nVad vill du ändra?");
                Console.WriteLine("(1) Förnamn\n(2) Efternamn\n(3) Ålder\n(4) Stad \n(5) Födelsedatum \n(6) Dödsår\n(7) Mor \n(8) Far \n(9) Barn\n");
                var input = Console.ReadLine();

                if (input == "1")
                {
                    Console.WriteLine("Skriv in det nya förnamnet.");
                    person.Förnamn = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Förnamnet ändrades till {person.Förnamn}");
                }
                else if (input == "2")
                {
                    Console.WriteLine("Skriv in det nya efternamnet.");
                    person.Efternamn = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Efternamnet ändrades till {person.Efternamn}");
                }
                else if (input == "3")
                {
                    Console.WriteLine("Skriv in den nya åldern.");
                    var ageInput = Console.ReadLine();
                    person.Ålder = Convert.ToInt32(ageInput);
                    crud.Update(person);
                    Console.WriteLine($"Åldern ändrades till {person.Ålder}");
                }
                else if (input == "4")
                {
                    Console.WriteLine("Skriv in den nya staden.");
                    person.Stad = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Staden ändrades till {person.Stad}");
                }
                else if (input == "5")
                {
                    Console.WriteLine("Skriv in det nya födelsedatumet.");
                    var föddInput = Console.ReadLine();
                    person.Född = Convert.ToInt32(föddInput);
                    crud.Update(person);
                    Console.WriteLine($"Födelsedatumet ändrades till {person.Född}");
                }
                else if (input == "6")
                {
                    Console.WriteLine("Skriv in det nya dödsdatumet.");
                    var dödInput = Console.ReadLine();
                    person.Död = Convert.ToInt32(dödInput);
                    crud.Update(person);
                    Console.WriteLine($"Dödsdatumet ändrades till {person.Död}");
                }
                // TODO: Ändra så att ID ändras till den nya föräldern? Om det inte finns så är det en vanlig redigering.
                else if (input == "7")
                {
                    bool loop = true;
                    while (loop)
                    {
                        Console.WriteLine($"(1) Redigera bara namnet.\n(2) Ändra så att modern inte är släkt med {person.Förnamn}.");
                        var morInput = Console.ReadLine();

                        if (morInput == "1")
                        {
                            Console.WriteLine($"Skriv in det nya namnet på modern till {person.Förnamn}.");
                            var morNamn = Console.ReadLine();
                            var mor = crud.Read(morNamn);
                            person.Mor = morNamn;
                            mor.Förnamn = morNamn;
                            person.ID = mor.BarnID;
                            crud.UpdateParent(mor);
                            crud.Update(person);
                            Console.WriteLine($"Moderns namn ändrades till {person.Mor}");
                            loop = false;
                        }
                        else if (morInput == "2")
                        {
                            var mor = crud.Read(person.MorID.ToString());
                            Console.WriteLine($"Skriv in det nya namnet på modern till {person.Förnamn}.");
                            person.Mor = Console.ReadLine();
                            if (DoesPersonExist(mor))
                            {
                                mor.Förnamn = person.Mor;   
                                mor.Efternamn = person.Efternamn;
                                person.MorID = mor.ID;
                                Console.WriteLine($"Moderns namn ändrades till {person.Mor}.");
                                crud.UpdateParent(mor);
                                crud.Update(person);
                            }
                            else if (DoesPersonExist(mor) == false)
                            {
                                Console.WriteLine("Modern finns inte i databasen.");
                                CreateParent(person.Mor, person.Efternamn, person.BarnID);
                                Console.WriteLine($"Skapade modern {person.Mor} {person.Efternamn}\nDu kan lägga till fler värden i menyn.");
                            }
                            loop = false;
                        }
                    }
                }

                else if (input == "8")
                {
                    Console.WriteLine($"Skriv in den nya fadern till {person.Förnamn}.");
                    person.Far = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Fadern ändrades till {person.Far}");
                }
                else if (input == "9")
                {
                    Console.WriteLine($"Skriv in namnet på det nya barnet till {person.Förnamn}.");

                    crud.Update(person);
                    Console.WriteLine($"Fadern ändrades till {person.Far}");
                }
            }


        }
    }
}
