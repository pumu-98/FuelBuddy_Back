using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FuelQueManagement_Service.Models
{
    //Implementing queue model
    public class QueueModel
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("vehicleType")]
        public string VehicleType { get; set; }
        [BsonElement("vehicleOwner")]
        public string VehicleOwner { get; set; }
        [BsonElement("stationsId")]
        public string? StationsId { get; set; }
        [BsonElement("fuelType")]
        public string? FuelType { get; set; }
        [BsonElement("arivalTime")]
        public string? ArivalTime { get; set; }
        [BsonElement("departTime")]
        public string? DepartTime { get; set; }

        public QueueModel(string vehicleType, string owner, string? arivalTime, string? departTime)
        {
            VehicleType = vehicleType;
            VehicleOwner = owner;
            ArivalTime = arivalTime;
            DepartTime = departTime;
        }

        public QueueModel()
        {
        }
    }
}
