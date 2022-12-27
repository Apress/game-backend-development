using System.ComponentModel.DataAnnotations;

namespace LeaderboardFunctions
{
    public class LeaderboardEntry
    {
        [Key]
        public string PlayerID { get; set; }
        public int Value { get; set; }
        public string DisplayName { get; set; }

    }
}
