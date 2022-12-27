using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MatchmakerFunctions
{
    public class Matchmaker
    {

        private readonly MatchmakerContext matchmakerContext;
        public Matchmaker(MatchmakerContext matchmakerContext)
        {
            this.matchmakerContext = matchmakerContext;
        }

        [FunctionName("Ticket")]
        public async Task<IActionResult> Ticket(
              [HttpTrigger(AuthorizationLevel.Anonymous, "post", "delete", "get", Route = null)] HttpRequest req,
              ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Ticket>(requestBody);
            string responseMessage = "";
            if (req.Method.Equals("POST"))
            {
                var ticketID = Guid.NewGuid().ToString();
                var ticket = new Ticket { TicketID = ticketID, Skill_level = data.Skill_level, State = "Waiting", ServerAddress = "not available", ServerName = "not available" };

                matchmakerContext.Tickets.Add(ticket);
                matchmakerContext.SaveChanges();
                FindMatch(ticket);
                responseMessage = ticketID;
            }
            else if (req.Method.Equals("DELETE"))
            {
                string ticketID = req.Query["TicketID"];
                if (!String.IsNullOrEmpty(ticketID))
                {
                    var ticket = matchmakerContext.Tickets.Find(ticketID);

                    matchmakerContext.Tickets.Remove(ticket);
                    matchmakerContext.SaveChanges();

                    var otherTicket = matchmakerContext.Tickets.Where(p => p.ServerName.Equals(ticket.ServerName)).SingleOrDefault();

                    if (otherTicket==null) {
                         await ShutdownServer(ticket.ServerName);
                    }
                    responseMessage = ticket.TicketID + " was deleted.";
                }
                else
                {
                    responseMessage = "TicketID is not available.";
                }
            }
           else if (req.Method.Equals("GET")) 
            {
                string ticketID = req.Query["TicketID"];
                if (!String.IsNullOrEmpty(ticketID)) {            
                    var currentTicket = matchmakerContext.Tickets.Find(ticketID);
                    responseMessage = currentTicket.ServerAddress;
                } else {
                    responseMessage = "TicketID is not available.";
                }
            }

            return new OkObjectResult(responseMessage);
        }


        public void FindMatch(Ticket currentTicket)
        {
            var tickets = matchmakerContext.Tickets;
            var CurrentTicket = matchmakerContext.Tickets.Find(currentTicket.TicketID);

            foreach (var ticket in tickets)
            {
                if (!ticket.TicketID.Equals(CurrentTicket.TicketID))
                {
                    if (ticket.State.Equals("Waiting"))
                    {
                        if (ticket.Skill_level.Equals(CurrentTicket.Skill_level))
                        {
                            ticket.State = "Matched";
                            CurrentTicket.State = "Matched";

                            Server server = AllocateReadyServer().Result;
                            ticket.ServerAddress = server.IP + ":" + server.Port;
                            ticket.ServerName = server.Name;
                            CurrentTicket.ServerAddress = server.IP + ":" + server.Port;
                            CurrentTicket.ServerName = server.Name;
                            break; // Only match 2 players
                        }
                    }
                }
            }
            matchmakerContext.SaveChanges();
        }

        public async Task<Server> AllocateReadyServer()
        {
            const string data = @"{""apiVersion"":""allocation.agones.dev/v1"",""kind"":""GameServerAllocation"",""spec"":{""required"":{""matchLabels"":{""agones.dev/fleet"":""mygame-server""}}}}";
            
            string kubernetesApiAccessToken = Environment.GetEnvironmentVariable("Kubernetes_API_Access_Token");
            string kubernetesApiAddress = Environment.GetEnvironmentVariable("Kubernetes_API_Address_Game_Allocation");

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            HttpClient httpClient = new HttpClient(clientHandler);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", kubernetesApiAccessToken);

            var requestData = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(kubernetesApiAddress, requestData);
            var result = await response.Content.ReadAsStringAsync();

            JObject o = JObject.Parse(result);
            Server server = new Server();
            
            server.IP = (string)o.SelectToken("status.address");
            server.Port = (string)o.SelectToken("status.ports[0].port");
            server.Name = (string)o.SelectToken("metadata.name"); 
         
            return server;
        }

          public async Task<String> ShutdownServer(string serverName)
        {
            string kubernetesApiAccessToken = Environment.GetEnvironmentVariable("Kubernetes_API_Access_Token");
            string kubernetesApiAddress = Environment.GetEnvironmentVariable("Kubernetes_API_Address_Game_Server") + serverName;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            HttpClient httpClient = new HttpClient(clientHandler);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", kubernetesApiAccessToken);
            
            var response = await httpClient.DeleteAsync(kubernetesApiAddress);
            var result = await response.Content.ReadAsStringAsync();
            
            return result;
        }

    }
}
