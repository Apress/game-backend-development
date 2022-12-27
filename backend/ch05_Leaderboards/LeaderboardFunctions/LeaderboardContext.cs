using Microsoft.EntityFrameworkCore;

namespace LeaderboardFunctions
{
    public class LeaderboardContext : DbContext
    {
        public LeaderboardContext(DbContextOptions<LeaderboardContext> options) : base(options)
        {
        }
        public DbSet<LeaderboardEntry> Leaderboard { get; set; }
    }
}
