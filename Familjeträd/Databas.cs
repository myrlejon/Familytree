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
        public string ConnectionString { get; set; } = "Data Source=DESKTOP-NJ9EFR0; Integrated Security = True;database={0}";
        public string Familjeträd { get; set; } = "Familjeträd";
        public string Master { get; set; } = "master";
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

        private static void Parameters((string, string)[] parameters, SqlCommand command)
        {
            foreach (var item in parameters)
            {
                command.Parameters.AddWithValue(item.Item1, item.Item2);
            }
        }
        internal bool DoesDatabasExist(string name)
        {
            var databas = GetDataTable("SELECT name FROM sys.databases WHERE name = @name", Master, ("@name", name));
            return databas.Rows.Count > 0;
        }

        internal void CreateDatabase(string databaseName, string databaseConnection)
        {
            SQL($"CREATE DATABASE {databaseName}", databaseConnection);
        }

        internal void UseDatabase(string databaseName, string databaseConnection)
        {
            SQL($"USE {databaseName}", databaseConnection);
        }

        internal void CreateDatatable(string datatableName, string fields, string databaseConnection)
        {
            SQL($"CREATE TABLE {datatableName} ({fields});", databaseConnection);
        }
    }
}
