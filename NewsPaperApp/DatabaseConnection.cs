using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace NewsPaperApp
{
    static class DatabaseConnection
    {
        private static string connectionString = "Server=.;Database=Newspaper;Trusted_Connection=true";
        private static SqlConnection sqlConnection = new SqlConnection(connectionString);

        private static void FillError(object sender, FillErrorEventArgs args)
        {
            //Handler for when an error occured during the fill method of the sqlDataAdapter
            /*The DataAdapter issues the FillError event when an error occurs during a Fill operation.
             * This type of error commonly occurs when the data in the row being added could not be 
             * converted to a .NET Framework type without some loss of precision.
             * If an error occurs during a Fill operation, the current row is not added to the DataTable. 
             * The FillError event enables you to resolve the error and add the row, or to ignore the 
             * excluded row and continue the Fill operation.
             * ^- Taken from https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/handling-dataadapter-events
             */

            if (args.Errors.GetType() == typeof(System.OverflowException))
            {
                MessageBox.Show("Error encountered during the filling of " + args.DataTable.TableName, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Continue = false;
            }
        }

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
            da.FillError += new FillErrorEventHandler(FillError);
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
            da.FillError += new FillErrorEventHandler(FillError);
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

            try
            {
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
            }
            catch(SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(),MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch(InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            return true;
        }

        public static List<string> GetProfileDetails(string username)
        {
            List<string> details = new List<string>();

            string query = "select CreationDate, Email, AccountTypes.Nume from Accounts inner join AccountTypes on Accounts.Type = AccountTypes.ID where Username = \'" + username + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
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

            try
            {
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
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //Return the status of the execution
            return true;
        }

        public static bool ComparePasswords(string username, string newHashedPassword)
        {
            string query = "select * from Accounts where Username = \'" + username + "\' and Password = \'" + newHashedPassword + "\'";
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return ((dt.Rows.Count == 0) ? true : false);
        }

        public static List<Newspaper> GetAvailableNewspapers()
        {
            List<Newspaper> newspapers = new List<Newspaper>();

            string query = "select Name, PublishingHouse, PublishingDate from NewsPaper";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
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
            string query = @"select Title, Accounts.Username, Content, ArticleCategories.Name from Article
                            inner join Accounts
                            on Accounts.ID = Article.PublisherID
                            inner join ArticleCategories
                            on ArticleCategories.ID = Article.CategoryID
                            inner join NewsPaper
                            on NewsPaper.ID = Article.NewsPaperID
                            where NewsPaper.Name =" + "\'" + name + "\'";


            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                articles.Add(new Article(dt.Rows[i].Field<string>("Title"), dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content"), dt.Rows[i].Field<string>("Name")));
            }

            return articles;
        }

        
        public static bool CreateNewspaper(string name, string publishingHouse, DateTime publishingDate)
        {
            /*
             * Creates a new newspaper with 0 articles 
             */
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                cmd.CommandText = "insert into NewsPaper (Name, PublishingHouse, PublishingDate) values ( \'" + name + "\', \'"
                    + publishingHouse + "\', " + publishingDate.ToString() + ")";
                string test = cmd.CommandText;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static bool CreateArticle(string newspaperName, string articleCategory, string publisher, string title, string content, string photoPath)
        {
            /*
             * Creates an article which will be contained in a newspaper
             * If the insertion was successful then this function will return true
             * Else it will return false
             */

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                cmd.CommandText = "insert into Article (Title, Content, PhotoPath, PublisherID, NewsPaperID, CategoryID) " +
                    "values ( \'" + title + "\', \'" + content + "\', \'" + photoPath + "\', (select ID from Accounts where Username = \'" + publisher + "\'), (select ID from NewsPaper where Name = \'" + newspaperName + "\' )"
                    + "(select ID from ArticleCategories where Name = \'" + articleCategory + "\'))";
                string test = cmd.CommandText;

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static List<Article> GetArticlesOfCategory(string newspaperName, string categoryName)
        {
            List<Article> articles = new List<Article>();
            string query = @"select Title, Accounts.Username, Content, ArticleCategories.Name from Article
                            inner join Accounts
                            on Accounts.ID = Article.PublisherID
                            inner join ArticleCategories
                            on ArticleCategoies.ID = Article.CategoryID
                            inner join NewsPaper
                            on NewsPaper.ID = Article.NewsPaperID
                            where NewsPaper.Name =" + "\'" + newspaperName + "\' and ArticleCategories.Name = \'" + categoryName + "\'";


            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                articles.Add(new Article(dt.Rows[i].Field<string>("Title"), dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content"), dt.Rows[i].Field<string>("Name")));
            }

            return articles;
        }

        public static List<Comment> GetNewspaperComments(string name)
        {
            List<Comment> comments = new List<Comment>();
            string query = @"select Content, Comments.PublishingDate, Accounts.Username from Comments
                             inner join NewsPaper
                             on NewsPaper.ID = Comments.NewsPaperID
                             inner join Accounts
                             on Accounts.ID = Comments.UserID
                             where NewsPaper.Name = \'" + name + "\'";


            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comments.Add(new Comment(dt.Rows[i].Field<string>("Username"), dt.Rows[i].Field<string>("Content"), dt.Rows[i].Field<DateTime>("PublishingDate")));
            }

            return comments;
        }

        public static bool AddCommentToNewsPaper(string newspaperName, string publisherName, string content)
        {
            if (content == null || content.Length == 0)
                return false;

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            try
            {
                sqlConnection.Open();
                cmd.CommandText = @"insert into Comments values(UserID, Content, NewsPaperID, PublishingDate)
                                values ((select ID from Accounts where Name = \'" + publisherName + "\'), \'" + content + "\', (select ID from NewsPaper where Name = \'" + newspaperName + "\'), GETDATE())";

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static float GetNewspaperRating(string name)
        {
            /*
             * Returns the rating of a newspaper 
             */
            string query = @"select Rating from Ratings
                             inner join NewsPaper
                             on NewsPaper.ID = Ratings.NewsPaperID
                             where NewsPaper.Name = \'" + name + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt.Rows[0].Field<float>("Rating");
        }

        public static bool RateNewspaper(string name, int grade)
        {
            /*
            * Adds a grade to the rating then it recalculates the rating 
            * Returns true if the operation was successful
            * Else it returns false
            */

            if (grade > 10 || grade < 0)
            {
                return false; 
            }

            //TO DO...Check if the account hasn't already rated the newspaper

            //First select the current rating and the ratings given
            string query1 = @"select Rating, RatingsGiven from Ratings
                             inner join Newspaper
                             on Newspaper.ID = Ratings.NewsPaperID
                             where Newspaper.Name = \'" + name + "\'";

            SqlDataAdapter da = new SqlDataAdapter(query1, connectionString);
            da.FillError += new FillErrorEventHandler(FillError);
            DataTable dt = new DataTable();
            da.Fill(dt);

            float rating = dt.Rows[0].Field<float>("Rating");
            int ratingsGiven = dt.Rows[0].Field<int>("RatingsGiven");

            //Calculate the new rating
            float newRating = rating * ratingsGiven;
            newRating += grade;
            ratingsGiven++;
            newRating = newRating / ratingsGiven;

            //Update the database with the new value
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();
                cmd.CommandText = @"update R
                                set Rating = " + newRating.ToString() + ", RatingsGiven = " + ratingsGiven.ToString() +
                                    "from Ratings as R" +
                                    "inner join NewsPaper" +
                                    "on NewsPaper.ID = Ratings.NewsPaperID" +
                                    "where NewsPaper.Name = \'" + name + "\'";

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (SqlException sqlException)
            {
                MessageBox.Show(sqlException.Message, "Error " + sqlException.ErrorCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error ", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
