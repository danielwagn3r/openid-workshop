namespace RefreshClient;

public class Worker : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, IHttpClientFactory factory)
    {
        _logger = logger;
        _httpClient = factory.CreateClient("client");

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using (var result = await _httpClient.GetAsync("double/3"))
            {
                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                _logger.LogInformation("Result: {Content}", content);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}