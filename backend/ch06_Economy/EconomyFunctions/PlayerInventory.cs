using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EconomyFunctions 
{
    public class PlayerInventory {
        
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("PlayerId")]
        public string PlayerId { get; set; }

        [BsonElement("Items")]
        
        public List<Item> Items { get; set; }  
    }
}