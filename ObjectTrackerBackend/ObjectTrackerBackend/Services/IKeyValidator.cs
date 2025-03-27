public interface IKeyValidator
{
    Task<bool> IsValid(string key);
}