using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ProfilesModels;
using PlayFab.Samples;

namespace CloudScriptFunctions
{
    public class CloudScript
    {
        [FunctionName("GetPlayFabIdFromEntityId")]
        public async Task<dynamic> Run(
               [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
               ILogger log)
        {
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;

            PlayFab.PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable("PLAYFAB_TITLE_ID");
            PlayFab.PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable("PLAYFAB_DEV_SECRET_KEY");

            var entityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(new PlayFab.AuthenticationModels.GetEntityTokenRequest());

            EntityKey entityKey = new EntityKey
            {
                Id = args["entityId"],
                Type = "title_player_account"
            };

            var request = new GetEntityProfileRequest { Entity = entityKey };
            var getProfileAsyncResult = await PlayFab.PlayFabProfilesAPI.GetProfileAsync(request);

            return getProfileAsyncResult.Result.Profile.Lineage.MasterPlayerAccountId;
        }
    }
}
