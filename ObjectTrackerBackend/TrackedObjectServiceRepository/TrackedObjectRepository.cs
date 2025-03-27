using MongoDB.Driver;
using TrackedObjectServiceRepository.Interface;
using TrackedObjectServiceRepository.Model;

namespace TrackedObjectServiceRepository
{
    public class TrackedObjectRepository : ITrackedObjectRepository
    {
        private readonly IMongoCollection<TrackedObject> _collection;

        public TrackedObjectRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<TrackedObject>(nameof(TrackedObject));
        }

        public async Task<TrackedObject> CreateAsync(TrackedObject model)
        {
            model.Id = Guid.NewGuid().ToString();
            await _collection.InsertOneAsync(model);
            return model;
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<TrackedObject>> GetAllAsync(int offset, int fetch)
        {
            var filter = Builders<TrackedObject>.Filter.Empty;

            return _collection
                .Find(filter)
                .Skip(offset)
                .Limit(fetch)
                .ToList();
        }

        public async Task<TrackedObject> GetByIdAsync(string id)
        {
            return await _collection.Find(model => model.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, TrackedObject model)
        {
            await _collection.FindOneAndUpdateAsync(
                Builders<TrackedObject>.Filter.Eq(c => c.Id, id),
                Builders<TrackedObject>.Update
                    .Set(c => c.Latitude, model.Latitude)
                    .Set(c => c.Longitude, model.Longitude)
                    .Set(c => c.Heading, model.Heading)
                    .Set(c => c.Timestamp, model.Timestamp),
                new FindOneAndUpdateOptions<TrackedObject> { ReturnDocument = ReturnDocument.After });
        }
    }
}
