using Microsoft.EntityFrameworkCore;

namespace MatchmakerFunctions
{
    public class MatchmakerContext : DbContext
    {
        public MatchmakerContext(DbContextOptions<MatchmakerContext> options) : base(options)
        {
        }        public DbSet<Ticket> Tickets { get; set; }
    }
}
