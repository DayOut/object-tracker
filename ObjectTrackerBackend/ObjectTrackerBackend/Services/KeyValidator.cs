using TokenServiceRepository.Interface;

public class KeyValidator : IKeyValidator
{
    private readonly ITokenRepository _tokenRepository;


    public KeyValidator(ITokenRepository tokenRepository) 
    { 
        _tokenRepository = tokenRepository;
    }

    public async Task<bool> IsValid(string token)
    {
        var userId = await _tokenRepository.GetUserIdByTokenAsync(token);
        if (userId == null)
            return false;

        return userId != null;
    }
}