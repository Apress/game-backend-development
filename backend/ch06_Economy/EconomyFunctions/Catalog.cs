using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EconomyFunctions 
{
    public class Catalog {
        
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("CatalogId")]
        public string CatalogId { get; set; }

        [BsonElement("Items")]
        public List<Item> Items { get; set; }
    }    
}
