using FuelQueManagement_Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FuelQueManagement_Service.Services
{
    public class QueueService
    {
        //creating the database connection
        private readonly IMongoCollection<FuelStationModel> _Collection;
        private readonly IConfiguration _Configuration;
        public QueueService(IOptions<DatabaseConnection> datbaseConnection, IConfiguration iConfig)
        {
            var mongoClient = new MongoClient(
                datbaseConnection.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                datbaseConnection.Value.DatabaseName);

            _Collection = mongoDatabase.GetCollection<FuelStationModel>(
                datbaseConnection.Value.CollectionName);

            _Configuration = iConfig;
        }

        //This is required to create a queue object 
        public async Task<FuelStationModel> Create(QueueModel request)
        {
                QueueModel queueModel = new QueueModel();
                queueModel.Id = ObjectId.GenerateNewId().ToString();
                queueModel.VehicleType = request.VehicleType;
                queueModel.VehicleOwner = request.VehicleOwner;
                queueModel.FuelType = request.FuelType;
                queueModel.StationsId = request.StationsId;
                queueModel.ArivalTime = DateTime.Now.ToString();

                var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, request.StationsId);
                var multiUpdate = Builders<FuelStationModel>.Update
                    .Push(u => u.Queue, queueModel);
                var result = await _Collection.UpdateOneAsync(firstStationFilter, multiUpdate);
                var results = _Collection.Find(i => i.Id == request.StationsId).ToList();

                return results[0];
        }

        //This is required to delete a queue object 
        public async Task<FuelStationModel> Delete(string fuelType, string stationId, string queueId)
        {
                // create a filter
                var fuelStationObject = Builders<FuelStationModel>
                    .Filter.ElemMatch(t => t.Queue,
                    queue => queue.FuelType == fuelType);
                var pullVehicle = Builders<FuelStationModel>.Update
                    .PullFilter(t => t.Queue,
                        queue => queue.Id == queueId);
                var result = await _Collection
                    .UpdateManyAsync(fuelStationObject, pullVehicle);
                var res = _Collection.Find(_ => true).Limit(1).SortByDescending(i => i.Id).ToList();

                return res[0];
        }

        //This is required to create a queue history object 
        public async void UpdateQueueHistory(string stationId, QueueModel queueHistory)
        {
            FuelStationModel fuelStation = new FuelStationModel();
            var firstStationFilter = Builders<FuelStationModel>.Filter.Eq(a => a.Id, stationId);
            QueueModel history = new QueueModel();
            history.Id = ObjectId.GenerateNewId().ToString();
            history.VehicleType = queueHistory.VehicleType;
            history.VehicleOwner = queueHistory.VehicleOwner;
            history.StationsId = queueHistory.StationsId;
            history.FuelType = queueHistory.FuelType;
            history.ArivalTime = queueHistory.ArivalTime;
            history.DepartTime = DateTime.Now.ToString();

            var multiUpdateDefinition = Builders<FuelStationModel>.Update
                .Push(u => u.QueueHistory, history);
            var pushNotificationsResult = await _Collection.UpdateOneAsync(firstStationFilter, multiUpdateDefinition);
        }

        // This is required to get the queue time 
        public async Task<string> GetQueueTime(QueueModel queue)
        {
            var arivalTime = DateTime.Parse(queue.ArivalTime).ToString("hh:mm tt");
            var currentTime = DateTime.Now.ToString("hh:mm tt");
            var timeDefferance = DateTime.Parse(currentTime).Subtract(DateTime.Parse(arivalTime));
            return timeDefferance.ToString();
        }

        // This is required to getthe queue length
        public async Task<Array> GetQueueLength(FuelStationModel station)
        {
            int[] queueLengthArray = new int[2];
            int letersPerVehicle =  _Configuration.GetValue<int>("LetersPerVehicle");
            int petrolQueueLength = station.TotalPetrol / letersPerVehicle;
            int dieselQueueLength =  station.TotalDiesel / letersPerVehicle;
            queueLengthArray[0] = petrolQueueLength;
            queueLengthArray[1] = dieselQueueLength;

            return queueLengthArray;

        }

    }
}
