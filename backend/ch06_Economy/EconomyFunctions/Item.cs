using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EconomyFunctions
{
    public class Item
    {
        [BsonElement("ItemId")]
        public string ItemId { get; set; }
        
        [BsonElement("DisplayName")]
        public string DisplayName { get; set; }

        [BsonElement("Price")]
        public int Price { get; set; }

        [BsonElement("Currency")]
        public string Currency { get; set; }
    }
}