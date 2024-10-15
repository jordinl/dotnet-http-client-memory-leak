using System.Net;

class Program
{
    private static readonly int Concurrency = int.Parse(Environment.GetEnvironmentVariable("CONCURRENCY") ?? "10");
    private static readonly int Limit = int.Parse(Environment.GetEnvironmentVariable("LIMIT") ?? "1000");

    static async Task Main()
    {
        Console.WriteLine("Starting:");
        Console.WriteLine($" * Concurrency: {Concurrency}");
        Console.WriteLine($" * Limit: {Limit}");

        var start = DateTime.Now;
        var urls = File.ReadLines("urls.txt").Take(Limit);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Concurrency };

        await Parallel.ForEachAsync(urls, parallelOptions, async (url, cancellationToken) =>
        {
            string code;

            using SocketsHttpHandler socketsHttpHandler = new();
            socketsHttpHandler.PooledConnectionLifetime = TimeSpan.Zero;
            socketsHttpHandler.PooledConnectionIdleTimeout = TimeSpan.Zero;

            using HttpClient httpClient = new(socketsHttpHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            try
            {
                using var response = await httpClient.GetAsync(url, cancellationToken);
                code = ((int)response.StatusCode).ToString();
            }
            catch (Exception ex)
            {
                code = (ex.InnerException ?? ex).GetType().Name;
            }

            Console.WriteLine($"{url}: {code}");
        });

        Console.WriteLine($"Finished: {DateTime.Now - start}");
    }
}