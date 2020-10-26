using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace Our_Login_Server
{
    class UserManagement
    {
        public static bool Exists(string username)
        {
            bool res = false;

            lock (SQL.Lock)
            {
                //We grab our query and set it into the SqlCommand.
                SqlCeCommand command = new SqlCeCommand(SQL.GetQuery("user_check", username), SQL.Connection);
                //We will get the rows from our executed command.
                SqlCeResultSet rs = command.ExecuteResultSet(ResultSetOptions.Scrollable);
                //If it has rows, that means the username exists.
                if (rs.HasRows)
                {
                    res = true;
                }

                //Cleanup
                rs.Dispose();
                command.Dispose();
            }

            return res;
        }

        public static bool Register(string username, string password)
        {
            bool res = false;

            lock (SQL.Lock)
            {
                //Grab the register query and execute it.
                SqlCeCommand command = new SqlCeCommand(SQL.GetQuery("user_register", username, password), SQL.Connection);
                int i = command.ExecuteNonQuery();
                //If i == 1, that means the registration was successful.
                if (i == 1)
                {
                    res = true;
                }
                //Cleanup
                command.Dispose();
            }

            return res;
        }

        public static bool Login(string username, string password)
        {
            bool res = false;

            lock (SQL.Lock)
            {
                //Grab the login query and execute it.
                SqlCeCommand command = new SqlCeCommand(SQL.GetQuery("user_login", username, password), SQL.Connection);
                var rs = command.ExecuteResultSet(ResultSetOptions.Scrollable);
                //If it has rows, the login combo exists
                if (rs.HasRows)
                {
                    res = true;
                }
                //Cleanup
                rs.Dispose();
                command.Dispose();
            }

            return res;
        }
    }
}
