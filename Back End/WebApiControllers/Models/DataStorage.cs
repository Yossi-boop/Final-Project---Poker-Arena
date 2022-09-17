using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classes;
using Server;

namespace WebApiControllers.Models
{
    public static class DataStorage
    {
        //dataBase
        public static List<LogedInUser> ActiveUsers { get; set; }


        public static List<Table> OpenTables { get; set; }
        public static List<Casino> Casinos { get; set; }
        public static SqlHelper Sql { get; set; }

        public class LogedInUser
        {
            public User User { get; set; }
            public DateTime LastActionTime { get; set; }

            public LogedInUser(User i_User)
            {
                this.User = i_User;
                LastActionTime = DateTime.Now;
            }
        }

        static DataStorage()
        {
            Sql = new SqlHelper();
            Casinos = new List<Casino>();
            Casinos.Add(new Casino("1234",2096, 1464, 2560, 1440));
            Casinos[0].Furnitures = new List<FurnitureInstance>
            {
                //The 5 statues
                new FurnitureInstance(2, 3200, 2000, 200, 95),
                new FurnitureInstance(2, 3200, 2400, 200, 95),
                new FurnitureInstance(2, 3600, 2000, 200, 95),
                new FurnitureInstance(2, 3600, 2400, 200, 95),
                new FurnitureInstance(2, 3400, 1800, 200, 95),

                //The 2 poker tables
                new FurnitureInstance(0, 4100, 1800, 150, 300,"1234"),
                new FurnitureInstance(0, 2500, 2500, 150, 300,"1235"),
                new FurnitureInstance(17, 4250, 1800, 75, 150,"1234"),
                new FurnitureInstance(17, 2650, 2500, 75, 150,"1235"),

                //The 2 roulette tables
                new FurnitureInstance(4, 4100, 2500, 150, 300),
                new FurnitureInstance(4, 2500, 1800, 150, 300),
                new FurnitureInstance(18, 4250, 2500, 150, 150),
                new FurnitureInstance(18, 2650, 1800, 150, 150),

                //The 4 brownCouchs
                new FurnitureInstance(9, 2096 + 71, 1465 + 132, 100, 200),
                new FurnitureInstance(19, 2000 + 71, 1400 + 132, 100, 200),
                new FurnitureInstance(10, 2096 + 71, 2804, 100, 200),
                new FurnitureInstance(20, 1980 + 71, 2810, 100, 200),
                new FurnitureInstance(11, 4465, 1465 + 132, 100, 200),
                new FurnitureInstance(21, 4465, 1400 + 132, 100, 200),
                new FurnitureInstance(12, 4456, 2810, 100, 200),
                new FurnitureInstance(22, 4456, 2810, 100, 200),
                new FurnitureInstance(13, 2096 + 71, 1900, 200, 100),
                new FurnitureInstance(13, 2096 + 71, 2150, 200, 100),
                new FurnitureInstance(13, 2096 + 71, 2400, 200, 100),
                new FurnitureInstance(14, 4556, 1900, 200, 100),
                new FurnitureInstance(14, 4556, 2150, 200, 100),
                new FurnitureInstance(14, 4556, 2400, 200, 100),
                new FurnitureInstance(15, 2500, 1465 + 132, 100, 200),
                new FurnitureInstance(15, 3400, 1465 + 132, 100, 200),
                new FurnitureInstance(15, 4000, 1465 + 132, 100, 200),
                new FurnitureInstance(16, 2500, 2810, 100, 200),
                new FurnitureInstance(16, 3400, 2810, 100, 200),
                new FurnitureInstance(16, 4000, 2810, 100, 200),

                //The 2 trees after the top statue
                new FurnitureInstance(1, 2800, 1700, 400, 340),
                new FurnitureInstance(1, 3700, 1700, 400, 340)
            };
            ActiveUsers = new List<LogedInUser>();
            OpenTables = new List<Table>();
            Casinos[0].CheckIfTimeForChest();
            Casinos[0].Tables.Add(new Table(9, new Setting(200, 5000, 20, 10, 30)));
            Casinos[0].Tables[0].Id = "1234";
            Casinos[0].Tables.Add(new Table(9, new Setting(2000, 50000, 200, 100, 30)));
            Casinos[0].Tables[1].Id = "1235";
        }

        internal static void AddStats(string i_Email)
        {
            Sql.AddStats(i_Email);
        }

        internal static void AddUser(User user)
        {
            Sql.AddUser(user.Email, user.Name, user.Password, 1);
        }

        internal static void UpdateUserDetails(string i_Email, User user)
        {
            Sql.UpdateUser(user);
        }

        internal static void UpdataStats(string i_Email, Stats i_Stats)
        {
            Sql.UpdateStats(i_Stats);
        }

        internal static bool checkIfUserOnline(string i_Email)
        {
            LogedInUser user = GetActiveUserByMail(i_Email);

            if(user != null && DateTime.Now.Subtract(user.LastActionTime).TotalSeconds <= 10)
            {
                return true;
            }

            return false;


        }

        public static Table GetTable(string i_TableId, string i_CasinoId)
        {
            Casino casino = GetCasino(i_CasinoId);
            foreach (Table table in casino.Tables)
            {
                if (table.Id.Equals(i_TableId))
                {
                    return table;
                }
            }

            return null;
        }

        public static Casino GetCasino(string i_CasinoId)
        {
            foreach (Casino casino in Casinos)
            {
                if (casino.Id.Equals(i_CasinoId))
                {
                    return casino;
                }
            }

            return null;
        }

        public static LogedInUser GetActiveUserByMail(string i_Email)
        {
            foreach (LogedInUser logedInUser in ActiveUsers)
            {
                if (logedInUser.User.Email.Equals(i_Email))
                {
                    return logedInUser;
                }
            }

            return null;
        }

        public static User GetUserByMail(string i_Email)
        {
            return Sql.GetUser(i_Email);
        }

        public static Stats GetStatsByMail(string i_Email)
        {
            return Sql.GetStats(i_Email);
        }


    }
}
