using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GameAnalyticsFunctions
{
    public class GameAnalytics
    {
        [FunctionName("GameAnalyticsFunc")]
        [return: EventHub("outputEventHubMessage", Connection = "EVENTHUB_CONNECTION_STRING")]
        public async Task<string> Run(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
              ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            return requestBody;
        }
    }
}