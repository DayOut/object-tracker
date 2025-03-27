using Microsoft.AspNetCore.SignalR;
public class TrackHub : Hub
{
    private readonly IKeyValidator _keyValidator;

    public TrackHub(IKeyValidator keyValidator)
    {
        _keyValidator = keyValidator;
    }

    public override async Task OnConnectedAsync()
    {
        var key = Context.GetHttpContext()?.Request.Query["key"];
        if (string.IsNullOrEmpty(key) || !await _keyValidator.IsValid(key))
        {
            Context.Abort();
            return;
        }
        await base.OnConnectedAsync();
    }
}