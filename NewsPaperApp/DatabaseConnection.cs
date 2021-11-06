using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace NewsPaperApp
{
    static class DatabaseConnection
    {
        private static string connectionString = "Server=.;Database=Newspaper;Trusted_Connection=true";
        private static SqlConnection sqlConnection = new SqlConnection(connectionString);

        public static ref SqlConnection GetSqlConnection()
        {
            return ref sqlConnection;
        }

        public static bool CheckLoginCredentials(string username, string hashedPassword)
        {
            /*
             * Checks if the credetials the user set before pressing the login button can be found in the Accounts table in the database
             * Returns true if found else false
             */
            string query = "select * from Accounts where Username = \'" + username + "\' and Password = \'" + hashedPassword + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count == 1) ? true : false);
        }

        public static bool AccountExists(string username)
        {
            string query = "select * from Accounts where Username = \'" + username + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count != 0) ? true : false);
        }

        public static bool RegisterAccount(string username, string hashedPassword, string email, string type)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            sqlConnection.Open();
            if (email != "")
            {
                cmd.CommandText = "insert into Accounts (Username, Password, Email, CreationDate, Type) values ( \'" + username + "\', \'"
                    + hashedPassword + "\', \'" + email + "\', GETDATE(), (select top(1) ID from AccountTypes where Nume = \'" + type + "\'))";
            }
            else
            {
                cmd.CommandText = "insert into Accounts (Username, Password, Email, CreationDate, Type) values ( \'" + username + "\', \'"
                    + hashedPassword + "\', NULL, GETDATE(), ( select top(1) ID from AccountTypes where Nume = '\'" + type + "\' ));";
            }
            string test = cmd.CommandText;

            cmd.ExecuteNonQuery();
            sqlConnection.Close();


            return true;
        }

        public static List<string> GetProfileDetails(string username)
        {
            List<string> details = new List<string>();

            string query = "select CreationDate, Email, AccountTypes.Nume from Accounts inner join AccountTypes on Accounts.Type = AccountTypes.ID where Username = \'" + username + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                details.Add(dt.Rows[i].Field<DateTime>("CreationDate").ToString());
                details.Add(dt.Rows[i].Field<string>("Email"));
                details.Add(dt.Rows[i].Field<string>("Nume"));
            }

            return details;
        }

        public static bool ChangePassword(string username, string newHashedPassword)
        {
            //A check to be sure but it should not get to the stage where the user can change the password if he is not logged in
            if(DatabaseConnection.AccountExists(username) == false)
            {
                return false;
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            
            //Open the connection
            sqlConnection.Open();

            //Query for updating the password
            cmd.CommandText = "update Accounts set Password = \'" + newHashedPassword + "\' where Username = \'" + username + "\'";

            //Execute the query
            cmd.ExecuteNonQuery();

            //Close the connection
            sqlConnection.Close();

            //Return the status of the execution
            return true;
        }

        public static bool ComparePasswords(string username, string newHashedPassword)
        {
            string query = "select * from Accounts where Username = \'" + username + "\' and Password = \'" + newHashedPassword + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count == 0) ? true : false);
        }
    }
}
