using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace LeaderboardFunctions
{
    public class Leaderboard
    {

        private readonly LeaderboardContext leaderboardContext;
        public Leaderboard(LeaderboardContext leaderboardContext)
        {
            this.leaderboardContext = leaderboardContext;
        }

        [FunctionName("Leaderboard")]
        public async Task<IActionResult> LeaderboardEntry(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get", Route = null)] HttpRequest req,
              ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<LeaderboardEntry>(requestBody);

            string responseMessage = "";
            if (req.Method.Equals("POST"))
            {
                var leaderboardEntry = new LeaderboardEntry { PlayerID = data.PlayerID, Value = data.Value, DisplayName = data.DisplayName };
                var oldLeaderboardEntry = leaderboardContext.Leaderboard.Find(data.PlayerID);

                if (oldLeaderboardEntry != null)
                {
                    log.LogInformation("old: " + oldLeaderboardEntry.Value + " new: " + data.Value);
                    if (oldLeaderboardEntry.Value < data.Value)
                    {
                        leaderboardContext.Entry(oldLeaderboardEntry).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                        leaderboardContext.Leaderboard.Update(leaderboardEntry);
                    }
                }
                else
                {
                    log.LogInformation("add");
                    leaderboardContext.Entry(leaderboardEntry).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    leaderboardContext.Leaderboard.Add(leaderboardEntry);
                }
                leaderboardContext.SaveChanges();
            }
            else if (req.Method.Equals("GET"))
            {
                List<LeaderboardEntry> list = new List<LeaderboardEntry>();
                list = leaderboardContext.Leaderboard.ToList();

                list.Sort((s1, s2) => s1.Value.CompareTo(s2.Value));
                list.Reverse();

                int maxResultsCount = int.Parse(req.Query["MaxResultsCount"]);
                if (maxResultsCount > list.Count) maxResultsCount = list.Count;

                list = list.GetRange(0, maxResultsCount);

                string result = JsonConvert.SerializeObject(list);
                responseMessage = result;
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
