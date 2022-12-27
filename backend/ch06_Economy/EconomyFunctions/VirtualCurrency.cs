using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EconomyFunctions 
{
    public class VirtualCurrency {
        
        [BsonElement("VC_Amount")]
        public int VC_Amount { get; set; }

        [BsonElement("VC_Name")]
        public string VC_Name { get; set; }
    }
}