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
        /// <summary>
        /// Denna metoden kopplar upp sig med "master" för att sedan ta reda på alla databaser som finns.
        /// Om det inte finns en databas som heter "Familjeträd" så kommer programmet att skapa en ny databas.
        /// </summary>
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
                            födelseland nvarchar(50),
                            födelsestad nvarchar(50),
                            född int,
                            död int,
                            dödsland nvarchar(50),
                            dödsstad nvarchar(50),
                            mor nvarchar(50),
                            far nvarchar(50),
                            morID int,
                            farID int,
                            barnID int", db.Familjeträd);
                Console.WriteLine($"Skapade databasen {db.Familjeträd}");
                Console.WriteLine("Börja med att skapa personer med hjälp av menyalternativ (1)!");
            }
            else if (db.DoesDatabasExist(db.Familjeträd))
            {
                Console.WriteLine("Kör menyn.");
                Console.WriteLine($"Aktuell databas: {db.Familjeträd}");
            }
        }

        /// <summary>
        /// Huvudmenyn i programmet som sedan körs i Main metoden. Metoden innehåller flera alternativ som användaren kan välja mellan.
        /// </summary>
        public static void Meny()
        {
            bool menyLoop = true;
            while (menyLoop)
            {
                var db = new Databas();
                var crud = new CRUD();
                Console.WriteLine("\n(1) Skapa en ny person.\n(2) Redigera ett värde hos en person.\n(3) Sök på en person.\n(4) Lista upp alla personer.");
                Console.WriteLine("(5) Uppdatera alla värden på en person\n(6) Visa syskon till en person.\n(7) Speciell sökning\n(8) Radera en person.\n");
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
                    crud.Search();
                }
                else if (choice == "4")
                {
                    crud.ListAllPeople();
                }
                else if (choice == "5")
                {
                    crud.UpdateValues();
                }
                else if (choice == "6")
                {
                    crud.ListSiblings();
                }
                else if (choice == "7")
                {
                    crud.SpecialSearch();
                }
                else if (choice == "8")
                {
                    crud.Delete();
                }
            }
        }

        /// <summary>
        /// Denna metoden tar in värden ifrån Person klassen och uppdaterar dom i databasen.
        /// </summary>
        /// <param name="person"></param>
        public void Create(Person person)
        {
            var db = new Databas();
            var sqlString = "INSERT INTO Personer (Förnamn, Efternamn, Ålder, Födelseland, Födelsestad, Född, Död, Dödsland, Dödsstad, Mor, Far, MorID, FarID, BarnID) VALUES(@Förnamn, @Efternamn, @Ålder, @Födelseland, @Födelsestad, @Född, @Död, @Dödsland, @Dödsstad, @Mor, @Far, @MorID, @FarID, @BarnID)";
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
                    cmd.Parameters.AddWithValue("@Födelseland", person.Födelseland);
                    cmd.Parameters.AddWithValue("@Födelsestad", person.Födelsestad);
                    cmd.Parameters.AddWithValue("@Född", person.Född);
                    cmd.Parameters.AddWithValue("@Död", person.Död);
                    cmd.Parameters.AddWithValue("@Dödsland", person.Dödsland);
                    cmd.Parameters.AddWithValue("@Dödsstad", person.Dödsstad);
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

        /// <summary>
        /// Denna metoden tar in värden ifrån Person klassen och uppdaterar dom i databasen.
        /// </summary>
        /// <param name="person"></param>
        public void Update(Person person)
        {
            var db = new Databas();
            db.SQL(@"UPDATE Personer SET Förnamn = @Förnamn, Efternamn = @Efternamn, Ålder = @Ålder,
                    Födelseland = @Födelseland, Födelsestad = @Födelsestad, Född = @Född, Död = @Död, Dödsland = @Dödsland, Dödsstad = @Dödsstad, 
                    Mor = @Mor, Far = @Far, MorID = @MorID, 
                    FarID = @FarID, BarnID = @BarnID WHERE ID = @ID", db.Familjeträd,
                         ("@Förnamn", person.Förnamn),
                         ("@Efternamn", person.Efternamn),
                         ("@Ålder", person.Ålder.ToString()),
                         ("@Födelseland", person.Födelseland),
                         ("@Födelsestad", person.Födelsestad),
                         ("@Född", person.Född.ToString()),
                         ("@Död", person.Död.ToString()),
                         ("@Dödsland", person.Dödsland),
                         ("@Dödsstad", person.Dödsstad),
                         ("@Mor", person.Mor),
                         ("@Far", person.Far),
                         ("@ID", person.ID.ToString()),
                         ("MorID", person.MorID.ToString()),
                         ("FarID", person.FarID.ToString()),
                         ("BarnID", person.BarnID.ToString())
                         );
        }

        /// <summary>
        /// Denna metoden läser in information om en person ifrån databasen med deras för och efternamn.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Denna metoden läser in information om en person ifrån databasen med deras ID.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Person ReadID(int ID)
        {
            var db = new Databas();
            DataTable dt;

            dt = db.GetDataTable("SELECT TOP 1 * FROM Personer WHERE ID = @ID", Familjeträd, ("@ID", ID.ToString()));

            if (dt.Rows.Count == 0)
                return null;

            return GetPersonObject(dt.Rows[0]);
        }

        /// <summary>
        /// Denna metoden listar upp alla personer som finns i databasen.
        /// </summary>
        public void ListAllPeople()
        {
            var list = new List<string>();
            var people = GetDataTable("SELECT * FROM Personer", Familjeträd);
            foreach (DataRow row in people.Rows)
            {
                list.Add($"{row["Förnamn"].ToString()} {row["Efternamn"].ToString()}");
            }
            foreach(var person in list)
            {
                Console.WriteLine(person);
            }
        }

            /// <summary>
            /// Denna metoden visar syskon till en person i databasen.
            /// </summary>
            public void ListSiblings()
        {
            var crud = new CRUD();
            Console.WriteLine("Vem vill du visa syskon till?");
            string namn = Console.ReadLine();
            var person = crud.Read(namn);
            if (person != null)
            {
                var list = crud.GetChild(person.FarID);
                if (list.Count >= 2)
                {
                    foreach (var row in list)
                    {
                        Console.Write($"{row} ");
                    }
                }
                else if (list.Count <= 1)
                {
                    Console.WriteLine("Personen har inga syskon.");
                }
            }
            else if (person == null)
            {
                Console.WriteLine("Personen finns inte i databasen.");
            }
        }

        /// <summary>
        /// Denna metoden innehåller speciella sökningar som användaren kan göra.
        /// </summary>
        public void SpecialSearch()
        {
            var db = new Databas();
            var person = new Person();
            Console.WriteLine("Vad för slags sökning vill du göra?\n\n(1) Lista upp personer som är födda ett visst årtal.");
            Console.WriteLine("(2) Lista upp förnamn som har ett förnamn börjar på en viss bokstav.\n(3) Lista upp personer som är från en viss stad.");
            Console.WriteLine("(4) Lista upp personer som har ett efternamn som börjar på en viss bokstav.\n(5) Lista upp personer som är födda i ett visst land\n");

            string input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("Skriv in årtalet.");
                var choice = Console.ReadLine();
                var people = GetDataTable("SELECT Förnamn, Efternamn FROM Personer WHERE Född = @choice", Familjeträd, ("@choice", choice));
                if (people.Rows.Count == 0)
                {
                    Console.WriteLine($"Det finns inga personer födda {choice}.");
                }
                else if (people.Rows.Count >= 1)
                {
                    foreach (DataRow row in people.Rows)
                    {
                        Console.WriteLine($"{row["Förnamn"].ToString()} {row["Efternamn"].ToString()}");
                    }
                }
            }
            if (input == "2")
            {
                Console.WriteLine("Skriv in bokstaven. (Det går bra att skriva mer än en bokstav, ex. Lu för Luke och Lukas)");
                string charInput = Console.ReadLine();
                string choice = $"{charInput}%";
                var people = GetDataTable("SELECT Förnamn, Efternamn FROM Personer WHERE Förnamn LIKE @choice", Familjeträd, ("@choice", choice));
                if (people.Rows.Count == 0)
                {
                    Console.WriteLine($"Det finns inga personer som börjar på {charInput}.");
                }
                else if (people.Rows.Count >= 1)
                {
                    foreach (DataRow row in people.Rows)
                    {
                        Console.WriteLine($"{row["Förnamn"].ToString()} {row["Efternamn"].ToString()}");
                    }
                }
            }
            if (input == "3")
            {
                Console.WriteLine("Skriv in staden.");
                var choice = Console.ReadLine();
                var people = GetDataTable("SELECT Förnamn, Efternamn FROM Personer WHERE Födelsestad = @choice", Familjeträd, ("@choice", choice));
                if (people.Rows.Count == 0)
                {
                    Console.WriteLine($"Det finns inga personer födda ifrån staden {choice}.");
                }
                else if (people.Rows.Count >= 1)
                {
                    foreach (DataRow row in people.Rows)
                    {
                        Console.WriteLine($"{row["Förnamn"].ToString()} {row["Efternamn"].ToString()}");
                    }
                }
            }
            if (input == "4")
            {
                Console.WriteLine("Skriv in bokstaven. (Det går bra att skriva mer än en bokstav, ex. Sm för Smith och Smithsson)");
                string charInput = Console.ReadLine();
                string choice = $"{charInput}%";
                var people = GetDataTable("SELECT Förnamn, Efternamn FROM Personer WHERE Efternamn LIKE @choice", Familjeträd, ("@choice", choice));
                if (people.Rows.Count == 0)
                {
                    Console.WriteLine($"Det finns inga personer som börjar på {charInput}.");
                }
                else if (people.Rows.Count >= 1)
                {
                    foreach (DataRow row in people.Rows)
                    {
                        Console.WriteLine($"{row["Förnamn"].ToString()} {row["Efternamn"].ToString()}");
                    }
                }
            }
            if (input == "5")
            {
                Console.WriteLine("Skriv in landet.");
                string choice = Console.ReadLine();
                var people = GetDataTable("SELECT Förnamn, Efternamn FROM Personer WHERE Födelseland LIKE @choice", Familjeträd, ("@choice", choice));
                if (people.Rows.Count == 0)
                {
                    Console.WriteLine($"Det finns inga personer som är födda efter {choice}.");
                }
                else if (people.Rows.Count >= 1)
                {
                    foreach (DataRow row in people.Rows)
                    {
                        Console.WriteLine($"{row["Förnamn"].ToString()} {row["Efternamn"].ToString()}");
                    }
                }
            }


        }

        /// <summary>
        /// Denna metoden används för att kunna söka på personer i databasen.
        /// </summary>
        public void Search()
        {
            var crud = new CRUD();
            Console.WriteLine("Vem vill du söka på?");
            string namn = Console.ReadLine();
            var person = crud.Read(namn);
            Print(person);
        }

        /// <summary>
        /// Denna metoden returnerar en lista på barn till en person i databasen.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<string> GetChild(int ID)
        {
            var list = new List<string>();
            var barnFar = GetDataTable("SELECT Förnamn FROM Personer WHERE FarID = @FarID", Familjeträd, ("@FarID", ID.ToString()));
            foreach (DataRow row in barnFar.Rows)
            {
                list.Add($"{row["Förnamn"].ToString().Trim()}");
            }
            var barnMor = GetDataTable("SELECT Förnamn FROM Personer WHERE MorID = @MorID", Familjeträd, ("@MorID", ID.ToString()));
            foreach (DataRow row in barnMor.Rows)
            {
                list.Add($"{row["Förnamn"].ToString().Trim()}");
            }
            return list;
        }


        /// <summary>
        /// Denna metoden skriver ut information om en person i databasen.
        /// </summary>
        /// <param name="person"></param>
        private static void Print(Person person)
        {
            var crud = new CRUD();
            if (person != null)
            {
                // Defaultvärdet hos föräldrar sätts till 0 och " "
                if (person.Ålder == 0 & person.Födelsestad == " ")
                {
                    Console.WriteLine($"{person.Förnamn} {person.Efternamn}");
                    Console.WriteLine($"\nDu har inte angett alla värden till {person.Förnamn} {person.Efternamn}.\nAnvänd menyval (5) för att lägga till mer information!\n");
                }
                else
                {
                    Console.WriteLine($"{person.Förnamn} {person.Efternamn}, är {person.Ålder} år gammal.");
                    Console.WriteLine($"{person.Förnamn} föddes {person.Född} i {person.Födelseland}, {person.Födelsestad}");
                }
                
                if (person.Död > 0)
                {
                    Console.WriteLine($"och dog {person.Död} i {person.Dödsland}, {person.Dödsstad}");
                }

                if (person.Mor.Length > 1)
                {
                    Console.WriteLine($"Mor: {person.Mor} {person.Efternamn}");
                }

                if (person.Far.Length > 1)
                {
                    Console.WriteLine($"Far: {person.Far} {person.Efternamn}");
                }

                if (person.BarnID > 0)
                {
                    Console.WriteLine($"Barn: ");
                    var fatherList = crud.GetChild(person.ID);
                    foreach (var row in fatherList)
                    {
                        Console.Write($"{row} ");
                    }
                    Console.WriteLine();
                }

            }
            else
            {
                Console.WriteLine("Person not found");
            }
        }

        /// <summary>
        /// Denna metoden returnerar variabler till Read och ReadID metoderna.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Person GetPersonObject(DataRow row)
        {
                return new Person
                {
                    Förnamn = row["Förnamn"].ToString(),
                    Efternamn = row["Efternamn"].ToString(),
                    Ålder = (int)row["Ålder"],
                    Födelseland = row["Födelseland"].ToString(),
                    Födelsestad = row["Födelsestad"].ToString(),
                    Född = (int)row["Född"],
                    Död = (int)row["Död"],
                    Dödsland = row["Dödsland"].ToString(),
                    Dödsstad = row["Dödsstad"].ToString(),
                    Mor = row["Mor"].ToString(),
                    Far = row["Far"].ToString(),
                    ID = (int)row["ID"],
                    MorID = (int)row["MorID"],
                    FarID = (int)row["FarID"],
                    BarnID = (int)row["BarnID"]
                };
        }

        /// <summary>
        /// Denna metoden finns till för att radera en person ifrån databasen.
        /// </summary>
        public void Delete()
        {
            Console.WriteLine("Skriv in namnet på personen du vill radera.");
            var namn = Console.ReadLine();
            
            var crud = new CRUD();
            var db = new Databas();
            var person = Read(namn);

            if (person != null)
            {
                var mor = Read($"{person.Mor} {person.Efternamn}");
                var far = Read($"{person.Far} {person.Efternamn}");
                var barn = ReadID(person.BarnID);

                if (mor != null)
                {
                    mor.BarnID = 0;
                    crud.Update(mor);
                }
                else if (far != null)
                {
                    far.BarnID = 0;
                    crud.Update(far);
                }
                else if (barn != null)
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

        /// <summary>
        /// Denna metoden används för metoderna UpdateValues och CreatePerson, poängen med denna metoden är att minska upprepad kod.
        /// </summary>
        /// <param name="person"></param>
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
            Console.WriteLine("Födelseland: ");
            person.Födelseland = Console.ReadLine();
            Console.WriteLine("Födelsestad: ");
            person.Födelsestad = Console.ReadLine();
            Console.WriteLine("Födelseår: ");
            string föddInput = Console.ReadLine();
            person.Född = Convert.ToInt32(föddInput);
            Console.WriteLine("Dödsår: (om personen fortfarande är vid liv, skriv in 0)");
            string dödInput = Console.ReadLine();
            person.Död = Convert.ToInt32(dödInput);

            if (person.Död > 0)
            {
                Console.WriteLine("Dödsland: ");
                person.Dödsland = Console.ReadLine();
                Console.WriteLine("Dödsstad: ");
                person.Dödsstad = Console.ReadLine();
            }
            else if (person.Död == 0)
            {
                person.Dödsland = " ";
                person.Dödsstad = " ";
            }

            Console.WriteLine("Mor: ");
            person.Mor = Console.ReadLine();
            Console.WriteLine("Far: ");
            person.Far = Console.ReadLine();
        }

        /// <summary>
        /// Denna metoden används när användaren vill uppdatera alla värden hos en person som redan existerar.
        /// </summary>
        public void UpdateValues()
        {
            var crud = new CRUD();
            var person = new Person();
            Console.WriteLine("Vem vill du uppdatera?\n");
            var input = Console.ReadLine();
            person = crud.Read(input);

            if (person != null)
            {
                InputValues(person);
                //Console.WriteLine($"{person.Ålder}"); ???
                Update(person);
                Console.WriteLine("Personen har blivit uppdaterad.");
            }
            else if (person == null)
            {
                Console.WriteLine("Personen finns inte i databasen.");
            }
        }

        /// <summary>
        /// Denna metoden används när användaren vill skapa en ny person från grunden.
        /// </summary>
        public void CreatePerson()
        {
            var crud = new CRUD();
            var person = new Person();
            InputValues(person);
            crud.Create(person);
            var readPerson = crud.Read($"{person.Förnamn} {person.Efternamn}");
            var readMor = crud.Read($"{person.Mor} {person.Efternamn}");
            var readFar = crud.Read($"{person.Far} {person.Efternamn}");
            person.ID = readPerson.ID;

            if (readMor == null)
            {
                var mor = new Person();
                mor.Förnamn = person.Mor;
                mor.Efternamn = person.Efternamn;
                mor.BarnID = person.ID;
                DefaultValue(mor);
                crud.Create(mor);
                var newMother = crud.Read($"{mor.Förnamn} {mor.Efternamn}");
                person.MorID = newMother.ID;
            }
            else
            {
                person.MorID = readMor.ID;
            }

            if (readFar == null)
            {
                var far = new Person();
                far.Förnamn = person.Far;
                far.Efternamn = person.Efternamn;
                far.BarnID = person.ID;
                DefaultValue(far);
                crud.Create(far);
                var newFather = crud.Read($"{far.Förnamn} {far.Efternamn}");
                person.FarID = newFather.ID;
            }
            else
            {
                person.FarID = readFar.ID;
            }

            crud.Update(person);

        }
        
        /// <summary>
        /// Denna metoden sätter ett defaultvärde på föräldrar som skapas, vilket man senare kan ändra.
        /// </summary>
        /// <param name="person"></param>
        public void DefaultValue(Person person)
        {
            person.Ålder = 0;
            person.Födelseland = " ";
            person.Födelsestad = " ";
            person.Född = 0;
            person.Död = 0;
            person.Dödsland = " ";
            person.Dödsstad = " ";
            person.Mor = " ";
            person.Far = " ";
        }

        /// <summary>
        /// Denna metoden finns för att man ska kunna ändra ett enskiljt värde hos en person.
        /// Man kan även ändra mor/far till en person som redan finns, om personen inte finns så skapas en ny förälder.
        /// </summary>
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
                Console.WriteLine("\nVad vill du ändra?\n");
                Console.WriteLine("(1) Förnamn\n(2) Efternamn\n(3) Ålder\n(4) Födelseland \n(5) Födelsestad \n(6) Födelseår\n(7) Dödsår\n(8) Dödsland\n(9) Dödsstad\n(10) Mor \n(11) Far\n");
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
                    Console.WriteLine("Skriv in det nya födelselandet.");
                    person.Födelseland = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Födelsedatumet ändrades till {person.Födelseland}");
                }
                else if (input == "5")
                {
                    Console.WriteLine("Skriv in den nya födelsestaden.");
                    person.Födelsestad = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Staden ändrades till {person.Födelsestad}");
                }
                else if (input == "6")
                {
                    Console.WriteLine("Skriv in det nya födelsedatumet.");
                    var föddInput = Console.ReadLine();
                    person.Född = Convert.ToInt32(föddInput);
                    crud.Update(person);
                    Console.WriteLine($"Födelsedatumet ändrades till {person.Född}");
                }
                else if (input == "7")
                {
                    Console.WriteLine("Skriv in det nya dödsdatumet.");
                    var dödInput = Console.ReadLine();
                    person.Död = Convert.ToInt32(dödInput);
                    crud.Update(person);
                    Console.WriteLine($"Dödsdatumet ändrades till {person.Död}");
                }
                else if (input == "8")
                {
                    Console.WriteLine("Skriv in det nya dödslandet.");
                    person.Dödsland = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Landet ändrades till {person.Dödsland}");
                }
                else if (input == "9")
                {
                    Console.WriteLine("Skriv in den nya dödsstaden.");
                    person.Dödsstad = Console.ReadLine();
                    crud.Update(person);
                    Console.WriteLine($"Staden ändrades till {person.Dödsstad}");
                }
                else if (input == "10")
                {
                    bool loop = true;
                    while (loop)
                    {
                        Console.WriteLine($"(1) Redigera bara namnet.\n(2) Ändra till en annan moder.");
                        var morInput = Console.ReadLine();

                        if (morInput == "1")
                        {
                            var mor = crud.ReadID(person.MorID);
                            if (person.MorID >= 1)
                            {
                                Console.WriteLine($"Skriv in det nya namnet på modern till {person.Förnamn}.");
                                var morNamn = Console.ReadLine();
                                person.Mor = morNamn;
                                mor.Förnamn = morNamn;
                                person.ID = mor.BarnID;
                                crud.Update(mor);
                                crud.Update(person);
                                Console.WriteLine($"Moderns namn ändrades till {person.Mor}");
                            }
                            else if (person.MorID == 0)
                            {
                                Console.WriteLine($"{person.Förnamn} har ingen moder i databasen.\nDu kan skapa en moder till {person.Förnamn} i huvudmenyn (1)!");
                            }
                            loop = false;
                        }
                        else if (morInput == "2")
                        {
                            Console.WriteLine($"Skriv in det nya namnet på modern till {person.Förnamn}.");
                            var nameInput = Console.ReadLine();
                            var mor = crud.Read(nameInput);
                            if (mor != null)
                            {
                                mor.Förnamn = nameInput;
                                mor.Efternamn = person.Efternamn;
                                person.MorID = mor.ID;
                                Console.WriteLine($"Moderns namn ändrades till {person.Mor}.");
                                crud.Update(mor);
                                crud.Update(person);
                            }
                            else if (mor == null)
                            {
                                Console.WriteLine("Modern finns inte i databasen.");
                                var createMother = new Person();
                                createMother.Förnamn = nameInput;
                                createMother.Efternamn = person.Efternamn;
                                createMother.BarnID = person.ID;

                                crud.DefaultValue(createMother);
                                crud.Create(createMother);

                                var readMother = crud.Read($"{createMother.Förnamn} {createMother.Efternamn}");
                                person.MorID = readMother.ID;
                                crud.Update(readMother);
                                crud.Update(person);
                                Console.WriteLine($"Skapade modern {readMother.Förnamn} {readMother.Efternamn}");
                            }
                            loop = false;
                        }
                    }
                }

                else if (input == "11")
                {
                    bool loop = true;
                    while (loop)
                    {
                        Console.WriteLine($"(1) Redigera bara namnet.\n(2) Ändra till en annan fader.");
                        var farInput = Console.ReadLine();

                        if (farInput == "1")
                        {
                            var far = crud.ReadID(person.FarID);
                            if (person.FarID >= 1)
                            {
                                Console.WriteLine($"Skriv in det nya namnet på fadern till {person.Förnamn}.");
                                var farNamn = Console.ReadLine();
                                person.Far = farNamn;
                                far.Förnamn = farNamn;
                                person.ID = far.BarnID;
                                crud.Update(far);
                                crud.Update(person);
                                Console.WriteLine($"Faderns namn ändrades till {person.Far}");
                            }
                            else if (person.FarID == 0)
                            {
                                Console.WriteLine($"{person.Förnamn} har ingen far i databasen.\nDu kan skapa en fader till {person.Förnamn} i huvudmenyn (1)!");
                            }
                            loop = false;
                        }
                        else if (farInput == "2")
                        {
                            Console.WriteLine($"Skriv in det nya förnamnet på fadern till {person.Förnamn}.");
                            var nameInput = Console.ReadLine();
                            var far = crud.Read(nameInput);
                            if (far != null)
                            {
                                far.Förnamn = nameInput;
                                person.Far = far.Förnamn;
                                far.Efternamn = person.Efternamn;
                                person.FarID = far.ID;
                                Console.WriteLine($"Ändrade förälder till {person.Far}.");
                                crud.Update(far);
                                crud.Update(person);
                            }
                            else if (far == null)
                            {
                                Console.WriteLine("Fadern finns inte i databasen.");
                                var createFather = new Person();
                                createFather.Förnamn = nameInput;
                                createFather.Efternamn = person.Efternamn;
                                createFather.BarnID = person.ID;

                                crud.DefaultValue(createFather);
                                crud.Create(createFather);

                                var readFather = crud.Read($"{createFather.Förnamn} {createFather.Efternamn}");
                                person.FarID = readFather.ID;
                                crud.Update(readFather);
                                crud.Update(person);
                                Console.WriteLine($"Skapade fadern {readFather.Förnamn} {readFather.Efternamn}.");
                            }
                            loop = false;
                        }
                    }
                }
            }
        }
    }
}
