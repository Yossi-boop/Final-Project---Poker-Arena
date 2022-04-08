using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Classes;

namespace WebApiControllers.data
{
    public class GameUsersDbContext : DbContext
    {
        public DbSet<Stats> Stat { get; set; }
        public DbSet<User> User { get; set; }
    }
}