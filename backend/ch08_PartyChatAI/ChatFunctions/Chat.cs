using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using Microsoft.Extensions.Logging;

namespace ChatFunctions
{
    public class Chat
    {
        [FunctionName("Subscribe")]
        public WebPubSubConnection Subscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [WebPubSubConnection(Hub = "chat")] WebPubSubConnection connection, ILogger log)
        {
            return connection;
        }


        [FunctionName("Publish")]
        public async Task Publish(
            [WebPubSubTrigger("chat", WebPubSubEventType.User, "message")]
             UserEventRequest request, BinaryData data, WebPubSubDataType dataType,
            ILogger log,
            [WebPubSub(Hub = "chat")] IAsyncCollector<WebPubSubAction> actions)
        {
            await actions.AddAsync(WebPubSubAction.CreateSendToAllAction(request.Data.ToString(), WebPubSubDataType.Text));
        }

    }
}