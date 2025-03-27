using TokenServiceRepository.Interface;
using StackExchange.Redis;

namespace TokenServiceRepository
{
    public class RedisTokenRepository : ITokenRepository
    {
        private readonly IDatabase _db;

        public RedisTokenRepository(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public Task StoreTokenAsync(string token, string userId, TimeSpan expiresIn)
        {
            return _db.StringSetAsync(token, userId, expiresIn);
        }

        public async Task<string> GetUserIdByTokenAsync(string token)
        {
            return await _db.StringGetAsync(token);
        }

        public Task RemoveTokenAsync(string token)
        {
            return _db.KeyDeleteAsync(token);
        }
    }
}
