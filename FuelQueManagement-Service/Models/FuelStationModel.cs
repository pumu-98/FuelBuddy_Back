using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FuelQueManagement_Service.Models;
//Implementing FuelStation  model
public class FuelStationModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("name")]
    public string Name { get; set; }
    [BsonElement("location")]
    public string Location { get; set; }
    [BsonElement("fuel")]
    public FuelModel[]? Fuel { get; set; }
    [BsonElement("queue")]
    public QueueModel[]? Queue { get; set; }
    [BsonElement("stationOwner")]
    public string StationOwner { get; set; }
    [BsonElement("dieselStatus")]
    public bool DieselStatus { get; set; }
    [BsonElement("petrolStatus")]
    public bool PetrolStatus { get; set; }
    [BsonElement("totalPetrol")]
    public int TotalPetrol { get; set; }
    [BsonElement("totalDiesel")]
    public int TotalDiesel { get; set; }
    [BsonElement("queueHistory")]
    public QueueModel[]? QueueHistory { get; set; }
    [BsonElement("lastModified")]
    public string? LastModified { get; set; }

    public FuelStationModel(string name, string location, FuelModel[]? fuel, QueueModel[]? queue, string stationOwner, string? lastModified)
    {
        Name = name;
        Location = location;
        Fuel = fuel;
        Queue = queue;
        StationOwner = stationOwner;
        LastModified = lastModified;
    }

    public FuelStationModel()
    {
    }
}
