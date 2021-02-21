using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Familjeträd
{
    class Databas
    {
        /// <summary>
        /// ConnectionString används för att koppla upp sig till Familjeträd och Master. 
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=DESKTOP-NJ9EFR0; Integrated Security = True;database={0}";
        public string Familjeträd { get; set; } = "Familjeträd";
        public string Master { get; set; } = "master";

        /// <summary>
        /// Denna metoden kör kopplar upp sig till SQL databasen och skickar in ett kommando med hjälp av parametrar.
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="databaseName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal long SQL(string sqlString, string databaseName, params (string, string)[] parameters)
        {
            long rows = 0;
            try
            {
                var conn = string.Format(ConnectionString, databaseName);
                using (var cnn = new SqlConnection(conn))
                {
                    cnn.Open();
                    using (var command = new SqlCommand(sqlString, cnn))
                    {
                        Parameters(parameters, command);
                        rows = command.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return rows;
        }

        /// <summary>
        /// Denna metoden hämtar ett table ifrån SQL databasen.
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="databaseName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal DataTable GetDataTable(string sqlString, string databaseName, params (string, string)[] parameters)
        {
            var dt = new DataTable();
            var connString = string.Format(ConnectionString, databaseName);
            try
            {
                using (var cnn = new SqlConnection(connString))
                {
                    cnn.Open();
                    using (var command = new SqlCommand(sqlString, cnn))
                    {
                        Parameters(parameters, command);

                        try
                        {
                            using (var adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(dt);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return dt;
        }

        /// <summary>
        /// Denna metoden skapar parametrar för SQL och GetDataTable metoderna. 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="command"></param>
        private static void Parameters((string, string)[] parameters, SqlCommand command)
        {
            foreach (var item in parameters)
            {
                command.Parameters.AddWithValue(item.Item1, item.Item2);
            }
        }
        /// <summary>
        /// Denna metoden kollar om en databas finns eller inte.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool DoesDatabasExist(string name)
        {
            var databas = GetDataTable("SELECT name FROM sys.databases WHERE name = @name", Master, ("@name", name));
            return databas.Rows.Count > 0;
        }

        /// <summary>
        /// Denna metoden skapar en databas.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="databaseConnection"></param>
        internal void CreateDatabase(string databaseName, string databaseConnection)
        {
            SQL($"CREATE DATABASE {databaseName}", databaseConnection);
        }

        /// <summary>
        /// Denna metoden väljer vilken databas som programmet kommer kommunicera till.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="databaseConnection"></param>
        internal void UseDatabase(string databaseName, string databaseConnection)
        {
            SQL($"USE {databaseName}", databaseConnection);
        }

        /// <summary>
        /// Skapar ett table i SQL databasen.
        /// </summary>
        /// <param name="datatableName"></param>
        /// <param name="fields"></param>
        /// <param name="databaseConnection"></param>
        internal void CreateDatatable(string datatableName, string fields, string databaseConnection)
        {
            SQL($"CREATE TABLE {datatableName} ({fields});", databaseConnection);
        }

    }
}
