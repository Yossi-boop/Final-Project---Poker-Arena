using Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SqlHelper
    {
        public SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        public SqlHelper()
        {
            builder.DataSource = "pokerarenafirsttest.database.windows.net";
            builder.UserID = "YossiHagever";
            builder.Password = "Meir1991";
            builder.InitialCatalog = "PokerArena";
        }

        public void AddUser(string i_Email, string i_Name, string i_Password, int i_Figure)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = String.Format("INSERT INTO [dbo].[Users] ([Email], [Name], [Password], [Figure]) VALUES (N'{0}', N'{1}', N'{2}', {3})", i_Email, i_Name, i_Password, i_Figure);

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                    //using (SqlDataReader reader = command.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                    //    }
                    //}
                }

            }

        }

        public void AddStats(string i_Email)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = String.Format("INSERT INTO [dbo].[Stats] ([UserEmail], [Money], [Level], [Xp], [NumberOfHandsPlay], [NumberOfHandsWon], [VictoryPercentage], [BiggestPot], [Card1], [Card2], [Card3], [Card4], [Card5]) VALUES (N'{0}', 5000, 1, 0, 0, 0, 0, 0, 52, 52, 52, 52, 52)", i_Email);

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                    //using (SqlDataReader reader = command.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                    //    }
                    //}
                }

            }

        }

        public User GetUser(string i_Email)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = String.Format("SELECT * FROM [dbo].[Users] WHERE Email ='{0}';", i_Email);
                User user = new User();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Email = reader.GetString(0);
                            user.Name = reader.GetString(1);
                            user.Password = reader.GetString(2);
                            user.Figure = reader.GetInt32(3);
                        }
                    }
                }

                if (user.Email == null)
                {
                    user = null;
                }
                return user;

            }
        }

        public Stats GetStats(string i_Email)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = String.Format("SELECT * FROM[dbo].[Stats] WHERE UserEmail = '{0}';", i_Email);

                Stats stat = new Stats();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stat.UserEmail = reader.GetString(0);
                            stat.Money = reader.GetInt32(1);
                            stat.Level = reader.GetInt32(2);
                            stat.Xp = reader.GetInt32(3);
                            stat.NumberOfHandsPlay = reader.GetInt32(4);
                            stat.NumberOfHandsWon = reader.GetInt32(5);
                            stat.VictoryPercentage = reader.GetFloat(6);
                            stat.BiggestPot = reader.GetFloat(7);
                            stat.Card1 = reader.GetInt32(8);
                            stat.Card2 = reader.GetInt32(9);
                            stat.Card3 = reader.GetInt32(10);
                            stat.Card4 = reader.GetInt32(11);
                            stat.Card5 = reader.GetInt32(12); ;
                        }
                    }
                }
                if (stat.UserEmail == null)
                {
                    stat = null;
                }
                return stat;

            }
        }

        public void UpdateStats(Stats i_Stats)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = String.Format("UPDATE [dbo].[Stats] SET Money = {1}, Level = {2} ,Xp = {3}," +
                        "NumberOfHandsPlay = {4}, NumberOfHandsWon = {5} , VictoryPercentage = {6}, BiggestPot = {7}, " +
                        "Card1 = {8}, Card2 = {9} , Card3 = {10}, Card4 = {11}, Card5 = {12}" +
                        " WHERE UserEmail = '{0}';", i_Stats.UserEmail, i_Stats.Money, i_Stats.Level, i_Stats.Xp, i_Stats.NumberOfHandsPlay, i_Stats.NumberOfHandsWon, i_Stats.VictoryPercentage,
                        i_Stats.BiggestPot, i_Stats.Card1, i_Stats.Card2, i_Stats.Card3, i_Stats.Card4, i_Stats.Card5);

                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    connection.Open();

                    command.ExecuteNonQuery();

                }

            }
        }

        public void UpdateUser(User i_User)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = String.Format("UPDATE [dbo].[Users] SET Name = '{1}', Figure = {3} WHERE Email = '{0}';", i_User.Email, i_User.Name, i_User.Password, i_User.Figure);

                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    connection.Open();

                    command.ExecuteNonQuery();

                }

            }
        }
    }
}
