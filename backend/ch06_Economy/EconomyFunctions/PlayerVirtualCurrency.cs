using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EconomyFunctions 
{
    public class PlayerVirtualCurrency {
        
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("PlayerId")]
        public string PlayerId { get; set; }

        [BsonElement("VirtualCurrencies")]
        
        public List<VirtualCurrency> VirtualCurrencies { get; set; }
    }
}