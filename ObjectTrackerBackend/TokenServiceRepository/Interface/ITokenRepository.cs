namespace TokenServiceRepository.Interface
{
    public interface ITokenRepository
    {
        Task StoreTokenAsync(string token, string userId, TimeSpan expiresIn);
        Task<string> GetUserIdByTokenAsync(string token);
        Task RemoveTokenAsync(string token);
    }
}
