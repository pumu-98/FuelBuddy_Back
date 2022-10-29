using FuelQueManagement_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FuelQueManagement_Service.Services
{
    public class FuelStationService
    {
        //Implementing  the database connection
        private readonly IMongoCollection<FuelStationModel> _Collection;
        public FuelStationService(IOptions<DatabaseConnection> datbaseConnection)
        {
            var mongoClient = new MongoClient(
                datbaseConnection.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                datbaseConnection.Value.DatabaseName);

            _Collection = mongoDatabase.GetCollection<FuelStationModel>(
                datbaseConnection.Value.CollectionName);
        }

        // refers for  the  create a fuel station
        public async Task<FuelStationModel> Create(FuelStationModel request)
        {
            FuelStationModel fuelStation = new FuelStationModel();
            fuelStation.Name = request.Name;
            fuelStation.Location = request.Location;
            fuelStation.StationOwner = request.StationOwner;
            fuelStation.LastModified = DateTime.Now.ToString();
            fuelStation.DieselStatus = false;
            fuelStation.PetrolStatus = false;
            fuelStation.TotalPetrol = 0;
            fuelStation.TotalDiesel = 0;
            fuelStation.Fuel = new FuelModel[0];
            fuelStation.Queue = new QueueModel[0];
            fuelStation.QueueHistory = new QueueModel[0];

            await _Collection.InsertOneAsync(fuelStation);
            var res = _Collection.Find(_ => true).Limit(1).SortByDescending(i => i.Id).ToList();

            return res[0];
        }

        // refers  to get all fuel stations
        public async Task<List<FuelStationModel>> GetFuelStations()
        {
            var res = _Collection.Find(_ => true).ToList();
            return res;
        }

        // refers to get fuel station by id
        public async Task<FuelStationModel> GetFuelStationById(string id)
        {
            var res = await _Collection.FindAsync(c => c.Id == id);
            return res.ToList()[0];
        }

        // refers the update diesel status
        public async Task<FuelStationModel> UpdateDieselStatus(bool status, string id)
        {
            FuelStationModel fuelStation = new FuelStationModel();
            fuelStation.DieselStatus = status;

            var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, id);
            var updateDefinition = Builders<FuelStationModel>.Update
                .Set(u => u.DieselStatus, fuelStation.DieselStatus);
            var updatedResult = await _Collection
                .UpdateOneAsync(firstStationFilter,
                updateDefinition);
            var res = _Collection.Find(_ => true).Limit(1).SortByDescending(i => i.Id).ToList();

            return res[0];
        }

        // This is required to update petrol status
        public async Task<FuelStationModel> UpdatePetrolStatus(bool status, string id)
        {
            FuelStationModel fuelStation = new FuelStationModel();
            fuelStation.PetrolStatus = status;

            var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, id);
            var updateNameDefinition = Builders<FuelStationModel>.Update
                .Set(u => u.PetrolStatus, fuelStation.PetrolStatus);
            var updateNameResult = await _Collection
                .UpdateOneAsync(firstStationFilter,
                updateNameDefinition);
            var res = _Collection.Find(_ => true).Limit(1).SortByDescending(i => i.Id).ToList();

            return res[0];
        }

        public async Task<int> getCurrentFuelAmount(string stationId, string type)
        {
            var res = await _Collection.FindAsync<FuelStationModel>(c => c.Id == stationId);
            return (type == "Diesel") ? res.ToList()[0].TotalDiesel : res.ToList()[0].TotalPetrol;
        }

        // This is required to update the total fuel amount
        public async void UpdateTotalFuelAmount(string stationId, int amount, string type, int currentAmount)
        {
            FuelStationModel fuelStation = new FuelStationModel();
            var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, stationId);

            if (type == "Diesel")
            {
                fuelStation.TotalDiesel = (currentAmount + amount);
                var updateDefinition = Builders<FuelStationModel>.Update
                    .Set(u => u.TotalDiesel, fuelStation.TotalDiesel);
                var updatedResult = await _Collection
                    .UpdateOneAsync(firstStationFilter, updateDefinition);
            }
            else
            {
                fuelStation.TotalPetrol = (currentAmount + amount);
                var updateDefinition = Builders<FuelStationModel>.Update
                    .Set(u => u.TotalPetrol, fuelStation.TotalPetrol);
                var updatedResult = await _Collection
                    .UpdateOneAsync(firstStationFilter, updateDefinition);
            }

        }

        // This is required to reduce the total fuel amount when the queue updated
        public async void ReduceFromTotalFuelAmount(string stationId, int amount, string type, int currentAmount)
        {
            FuelStationModel fuelStation = new FuelStationModel();
            var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, stationId);

            if (type == "Diesel")
            {
                fuelStation.TotalDiesel = (currentAmount - amount);
                var updateDefinition = Builders<FuelStationModel>.Update
                    .Set(u => u.TotalDiesel, fuelStation.TotalDiesel);
                var updatedResult = await _Collection
                    .UpdateOneAsync(firstStationFilter, updateDefinition);
            }
            else
            {
                fuelStation.TotalPetrol = (currentAmount - amount);
                var updateDefinition = Builders<FuelStationModel>.Update
                    .Set(u => u.TotalPetrol, fuelStation.TotalPetrol);
                var updatedResult = await _Collection
                    .UpdateOneAsync(firstStationFilter, updateDefinition);
            }
        }

        // This is required to get station by queue id 
        public async Task<FuelStationModel> GetStationByQueueId(string queueId)
        {
            var stationWithQueue = await _Collection
                .Find(t => t.Queue
                    .Any(c => c.Id == queueId)).ToListAsync();

            return stationWithQueue[0];
        }
    }
}
