using Microsoft.AspNetCore.SignalR;
using TrackedObjectServiceRepository.Model;

public class TrackingService : BackgroundService
{
    private readonly IHubContext<TrackHub> _hubContext;
    private readonly Dictionary<string, TrackedObject> _objects = new();
    private readonly Random _rand = new();


    public TrackingService(IHubContext<TrackHub> hubContext)
    {
        _hubContext = hubContext;
        init();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const double speed = 0.001;

        int disabledCount = 0;
        var startTime = DateTime.UtcNow;
       

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            // Кожні 10 секунд збільшуємо кількість вимкнених об'єктів
            if ((now - startTime).TotalSeconds >= (disabledCount + 1) * 10)
            {
                disabledCount++;
            }

            foreach (var obj in _objects.Values)
            {
                // Легка зміна напрямку
                obj.Heading += (_rand.NextDouble() - 0.5) * 10;
                obj.Heading = (obj.Heading + 360) % 360;

                // Рух вперед за напрямком
                var headingRad = obj.Heading * Math.PI / 180.0;
                obj.Latitude += Math.Cos(headingRad) * speed;
                obj.Longitude += Math.Sin(headingRad) * speed;

                obj.Timestamp = now;
            }

            var objectsToSend = _objects.Values
                .OrderBy(o => o.Id)
                .Skip(disabledCount)
                .ToList();

            await _hubContext.Clients.All.SendAsync("ReceiveObjects", objectsToSend, cancellationToken: stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void init()
    {
        // Here we can receive this objects from Redis or Mongo

        var rand = new Random();

        for (int i = 1; i <= 200; i++)
        {
            string id = $"obj-{i:D3}";
            double heading = rand.NextDouble() * 360.0;

            AddOrUpdate(id, heading);
        }
        Console.WriteLine(_objects.ToString());
    }

    public void AddOrUpdate(string id, double heading = 0)
    {
        if (!_objects.ContainsKey(id))
        {
            _objects[id] = new TrackedObject
            {
                Id = id,
                
                Latitude = 50.4501,
                Longitude = 30.5234,
                Heading = heading,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}