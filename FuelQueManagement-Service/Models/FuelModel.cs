using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FuelQueManagement_Service.Models
{
    //Implementing Fuel Model Class
    public class FuelModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("amount")]
        public int Amount { get; set; }
        [BsonElement("date")]
        public string Date { get; set; }
        [BsonElement("time")]
        public string Time { get; set; }
        [BsonElement("stationsId")]
        public string? StationsId { get; set; }
        [BsonElement("lastModified")]
        public string? LastModified { get; set; }

        public FuelModel(string type, int amount, string? lastModified)
        {
            Type = type;
            Amount = amount;
            LastModified = lastModified;
        }

        public FuelModel()
        {
         
        }
    }
}
