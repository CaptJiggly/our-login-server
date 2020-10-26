using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;
namespace Our_Login_Server
{
    class SQL
    {
        //This will hold our SqlConnection
        public static SqlCeConnection Connection;

        //This is our path to our queries
        public const string Sql_Query_Path = "SQL\\Queries";

        //We will use this to sync lock our database when it is accessed.
        public static object Lock = new object();

        public static void Open(string path)
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
            {
                return;
            }
            //Create the new connection.
            Connection = new SqlCeConnection("Data Source=" + path);

            //Open it so its ready for use.
            Connection.Open();
        }

        public static void Close()
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Closed)
            {
                return;
            }

            //Close the connection.
            Connection.Close();
            Connection = null;
        }
        public static string GetQuery(string name, params object[] args)
        {
            //We will read the query from the disk
            string query = File.ReadAllText(Path.Combine(Sql_Query_Path, name + ".sql"));
            //If we have any arguments placed, we will use string.Format and set our arguments in the actual query.
            if (args.Length > 0)
            {
                query = string.Format(query, args);
            }

            return query;
        }


    }
}
