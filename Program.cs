class Program
{
    private static readonly HttpClient HttpClient = new();
    private static readonly int Concurrency = int.Parse(Environment.GetEnvironmentVariable("CONCURRENCY") ?? "10");
    private static readonly int Limit = int.Parse(Environment.GetEnvironmentVariable("LIMIT") ?? "1000");

    static async Task Main()
    {
        Console.WriteLine("Starting:");
        Console.WriteLine($" * Concurrency: {Concurrency}");
        Console.WriteLine($" * Limit: {Limit}");

        HttpClient.Timeout = TimeSpan.FromSeconds(5);

        var start = DateTime.Now;
        var urls = File.ReadLines("urls.txt").Take(Limit);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Concurrency };

        await Parallel.ForEachAsync(urls, parallelOptions, async (url, cancellationToken) =>
        {
            string code;

            try
            {
                using var response = await HttpClient.GetAsync(url, cancellationToken);
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