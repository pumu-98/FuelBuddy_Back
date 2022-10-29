using FuelQueManagement_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FuelQueManagement_Service.Services
{
    public class FuelService
    {
        //create the database connection
        private readonly IMongoCollection<FuelStationModel> _Collection;
        public FuelService(IOptions<DatabaseConnection> datbaseConnection)
        {
            var mongoClient = new MongoClient(
                datbaseConnection.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                datbaseConnection.Value.DatabaseName);

            _Collection = mongoDatabase.GetCollection<FuelStationModel>(
                datbaseConnection.Value.CollectionName);
        }

        // create a fuel object
        public async Task<FuelStationModel> Create(FuelModel request)
        {
            FuelModel fuelModel = new FuelModel();
            fuelModel.Id = ObjectId.GenerateNewId().ToString();
            fuelModel.Type = request.Type.ToString();
            fuelModel.Amount = request.Amount;
            fuelModel.Date = request.Date;
            fuelModel.Time = request.Time;
            fuelModel.LastModified = DateTime.Now.ToString();
            fuelModel.StationsId = request.StationsId ?? null;

            var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, request.StationsId);
            var multiUpdateDefinition = Builders<FuelStationModel>.Update
                .Push(u => u.Fuel, fuelModel);
            var pushNotificationsResult = await _Collection.UpdateOneAsync(firstStationFilter, multiUpdateDefinition);
            var results = _Collection.Find(i => i.Id == request.StationsId).ToList();

            return results[0];
        }

    }
}
