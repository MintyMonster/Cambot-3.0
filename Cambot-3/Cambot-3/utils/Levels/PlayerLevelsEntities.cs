using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cambot_3.utils.Levels
{
    public partial class PlayerLevelsEntities : DbContext
    {
        public virtual DbSet<LevelsLeaderboardModel> PlayersDB { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = AppContext.BaseDirectory + "PlayersDB.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }
    }
}