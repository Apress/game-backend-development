using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Driver;

namespace EconomyFunctions
{
    public class Economy
    {

        [FunctionName("Catalog")]
        public async Task<IActionResult> Catalog(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", Route = null)] HttpRequest req,
                ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Catalog data = JsonConvert.DeserializeObject<Catalog>(requestBody);

            var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBConnectionString"));
            var database = client.GetDatabase("mygame-cosmos-mongo-db");
            var collection = database.GetCollection<Catalog>("Catalog");

            string responseMessage = "";

            if (req.Method.Equals("POST"))
            {
                var catalogFilter = new BsonDocument { { "CatalogId", data.CatalogId } };
                var res = await collection.Find(catalogFilter).FirstOrDefaultAsync();
                if (res != null) // Catalog exists
                {
                    bool itemExists = false;
                    foreach (var item in data.Items)
                    {
                        var fittingVirtualCurrency = res.Items.Find(x => x.ItemId == item.ItemId);
                        if (fittingVirtualCurrency != null) itemExists = true;
                    }
                    if (itemExists) // Item exists
                    {
                        responseMessage = "Item is already existing in the catalog.";

                    }
                    else // Item not exists
                    {
                        var filter = Builders<Catalog>.Filter.Where(x => x.CatalogId == data.CatalogId);
                        foreach (var item in data.Items)
                        {
                            var update = Builders<Catalog>.Update.Push<Item>(x => x.Items, item);
                            var result = collection.UpdateOneAsync(filter, update).Result;
                        }
                        responseMessage = "There was no such item(s) in this catalog so far. Item(s) added.";
                    }
                }
                else // Catalog not exists
                {
                    collection.InsertOne(data);
                    responseMessage = "This catalog hasn't existed so far. Added " + data.CatalogId + ".";
                }
            }
            else if (req.Method.Equals("GET"))
            {
                string catalogId = req.Query["CatalogId"];
                var filter = new BsonDocument { { "CatalogId", catalogId } };
                var res = await collection.Find(filter).FirstOrDefaultAsync();
                if (res == null)
                {
                    responseMessage = "This catalog has no items.";
                }
                else
                {
                    responseMessage = JsonConvert.SerializeObject(res);
                }
            }
            else if (req.Method.Equals("DELETE"))
            {

                foreach (var item in data.Items)
                {

                    var filter = Builders<Catalog>.Filter.Where(x => x.CatalogId == data.CatalogId && x.Items.Any(i => i.ItemId == item.ItemId));
                    var update = Builders<Catalog>.Update.Pull<Item>(x => x.Items, item);
                    var result = collection.UpdateOneAsync(filter, update).Result;

                    if (result.ModifiedCount > 0)
                    {
                        responseMessage += item.ItemId + " is deleted. ";
                    }
                    else if (result.ModifiedCount == 0)
                    {
                        responseMessage += item.ItemId + " is not existing. ";
                    }

                }
            }
            return (ActionResult)new OkObjectResult(responseMessage);
        }

        [FunctionName("VirtualCurrency")]
        public async Task<IActionResult> VirtualCurrency(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
                ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PlayerVirtualCurrency data = JsonConvert.DeserializeObject<PlayerVirtualCurrency>(requestBody);

            var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBConnectionString"));
            var database = client.GetDatabase("mygame-cosmos-mongo-db");
            var collection = database.GetCollection<PlayerVirtualCurrency>("PlayerVirtualCurrency");

            string responseMessage = "";

            if (req.Method.Equals("POST"))
            {
                var playerFilter = new BsonDocument { { "PlayerId", data.PlayerId } };
                var res = await collection.Find(playerFilter).FirstOrDefaultAsync();
                if (res != null) // Player exists
                {
                    var fittingVirtualCurrency = res.VirtualCurrencies.Find(x => x.VC_Name == data.VirtualCurrencies[0].VC_Name);
                    if (fittingVirtualCurrency != null) // VC exists
                    {

                        if (data.VirtualCurrencies.Count > 1)
                        {
                            responseMessage = "Update only one virtual currency at once.";
                        }
                        else
                        {
                            var filter = Builders<PlayerVirtualCurrency>.Filter.Where(x => x.PlayerId == data.PlayerId && x.VirtualCurrencies.Any(i => i.VC_Name == data.VirtualCurrencies[0].VC_Name));
                            var update = Builders<PlayerVirtualCurrency>.Update.Set(x => x.VirtualCurrencies[-1].VC_Amount, data.VirtualCurrencies[0].VC_Amount);
                            var result = collection.UpdateOneAsync(filter, update).Result;
                            if (result.ModifiedCount > 0)
                            {
                                responseMessage = "Modified " + data.VirtualCurrencies[0].VC_Name + " to " + data.VirtualCurrencies[0].VC_Amount + ".";
                            }
                            else if (result.ModifiedCount == 0)
                            {
                                responseMessage = "Same amount of " + data.VirtualCurrencies[0].VC_Name + " is available. Nothing has changed.";
                            }
                        }
                    }
                    else // VC not exists
                    {
                        var filter = Builders<PlayerVirtualCurrency>.Filter.Where(x => x.PlayerId == data.PlayerId);
                        var update = Builders<PlayerVirtualCurrency>.Update.Push<VirtualCurrency>(x => x.VirtualCurrencies, data.VirtualCurrencies[0]);
                        var result = collection.UpdateOneAsync(filter, update).Result;
                        responseMessage = "There was no such virtual currency for this player so far.  I added " + data.VirtualCurrencies[0].VC_Name + ".";
                    }
                }
                else // Player not exists
                {
                    collection.InsertOne(data);
                    responseMessage = "This player didn't have any virtual currency so far. Added " + data.PlayerId + ".";
                }
            }
            else if (req.Method.Equals("GET"))
            {
                string playerId = req.Query["PlayerId"];
                var filter = new BsonDocument { { "PlayerId", playerId } };
                var res = await collection.Find(filter).FirstOrDefaultAsync();
                if (res == null)
                {
                    responseMessage = "This player has no virtual currency.";
                }
                else
                {
                    responseMessage = JsonConvert.SerializeObject(res);
                }
            }
            return (ActionResult)new OkObjectResult(responseMessage);
        }



        [FunctionName("Inventory")]
        public async Task<IActionResult> Inventory(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
                ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PlayerInventory data = JsonConvert.DeserializeObject<PlayerInventory>(requestBody);

            var client = new MongoClient(System.Environment.GetEnvironmentVariable("MongoDBConnectionString"));
            var database = client.GetDatabase("mygame-cosmos-mongo-db");
            var collection = database.GetCollection<PlayerInventory>("PlayerInventory");

            string responseMessage = "";

            if (req.Method.Equals("POST"))
            {
                var playerFilter = new BsonDocument { { "PlayerId", data.PlayerId } };
                var res = await collection.Find(playerFilter).FirstOrDefaultAsync();
                if (res != null) // Player exists
                {
                    var fittingItem = res.Items.Find(x => x.ItemId == data.Items[0].ItemId);
                    if (fittingItem != null) // Item exists
                    {
                        responseMessage = "Item is already existing in the inventory.";
                    }
                    else // Item not exists
                    {
                        var filter = Builders<PlayerInventory>.Filter.Where(x => x.PlayerId == data.PlayerId);
                        var update = Builders<PlayerInventory>.Update.Push<Item>(x => x.Items, data.Items[0]);
                        var result = collection.UpdateOneAsync(filter, update).Result;
                        responseMessage = "There was no such item for this player so far. I added " + data.Items[0].ItemId + ".";
                    }
                }
                else // Player not exists
                {
                    collection.InsertOne(data);
                    responseMessage = "This player didn't have any item so far. Added " + data.PlayerId + ".";
                }
            }
            else if (req.Method.Equals("GET"))
            {
                string playerId = req.Query["PlayerId"];
                var filter = new BsonDocument { { "PlayerId", playerId } };
                var res = await collection.Find(filter).FirstOrDefaultAsync();
                if (res == null)
                {
                    responseMessage = "This player has no items so far.";
                }
                else
                {
                    responseMessage = JsonConvert.SerializeObject(res);
                }
            }
            return (ActionResult)new OkObjectResult(responseMessage);
        }
    }
}



