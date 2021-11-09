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
            /*
             *  Checks if an account with the username passed as a parameter exists in the database
             *  Returns true if it exists else it returns false
             */
            string query = "select * from Accounts where Username = \'" + username + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count != 0) ? true : false);
        }

        public static bool RegisterAccount(string username, string hashedPassword, string email, string type)
        {
            /*
             * Inserts an account into the database
             */
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

        public static List<Newspaper> GetAvailableNewspapers()
        {
            List<Newspaper> newspapers = new List<Newspaper>();

            string query = "select Name, PublishingHouse, PublishingDate from NewsPaper";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                newspapers.Add(new Newspaper(dt.Rows[i].Field<string>("Name"), dt.Rows[i].Field<string>("PublishingHouse"), dt.Rows[i].Field<DateTime>("PublishingDate")));
            }

            return newspapers;
        }

        public static List<Article> GetNewspaperArticles(string name)
        {
            List<Article> articles = new List<Article>();
            string query = @"Title, Accounts.Username, Content from Article
                            inner join Accounts
                            on Accounts.ID = Article.PublisherID
                            inner join NewsPaper
                            on NewsPaper.ID = Article.NewsPaperID
                            where NewsPaper.Name = '" + name + "'";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                articles.Add(new Article(dt.Rows[i].Field<string>("Title"), dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content")));
            }

            return articles;
        }
    }
}
