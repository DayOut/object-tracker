using MongoDB.Driver;
using UserServiceRepository.Interface;
using UserServiceRepository.Model;

namespace UserServiceRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<User>(nameof(User));
        }

        public async Task<User> CreateAsync(User model)
        {
            model.Id = Guid.NewGuid().ToString();
            model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var tmp = BCrypt.Net.BCrypt.HashPassword("admin");

            await _collection.InsertOneAsync(model);
            model.Password = ""; // hide pass before sending back
            return model;
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync(int offset, int fetch)
        {
            var filter = Builders<User>.Filter.Empty;

            return _collection
                .Find(filter)
                .Skip(offset)
                .Limit(fetch)
                .ToList();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _collection.Find(model => model.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, User model)
        {
            var update = Builders<User>.Update
                .Set(c => c.Name, model.Name)
                .Set(c => c.Username, model.Username)
                .Set(c => c.Email, model.Email)
                .Set(c => c.PhoneNumber, model.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                update = update.Set(c => c.Password, hashedPassword);
            }

            //string hashedPassword = 
            await _collection.FindOneAndUpdateAsync(
                Builders<User>.Filter.Eq(c => c.Id, id),
                update,
                new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After });
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();

            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return user;
        }
    }
}