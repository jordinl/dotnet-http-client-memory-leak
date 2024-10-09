class Program
{
    private static readonly HttpClient HttpClient = new();
    private static readonly int Concurrency = int.Parse(Environment.GetEnvironmentVariable("CONCURRENCY") ?? "20");
    private static readonly int Limit = int.Parse(Environment.GetEnvironmentVariable("LIMIT") ?? "1000");

    static async Task Main()
    {
        Console.WriteLine("Starting:");
        Console.WriteLine($" * Concurrency: {Concurrency}");
        Console.WriteLine($" * Limit: {Limit}");

        HttpClient.Timeout = TimeSpan.FromSeconds(5);

        var start = DateTime.Now;
        var urls = File.ReadLines("urls.txt").Take(Limit);
        var semaphore = new SemaphoreSlim(Concurrency, Concurrency);

        var tasks = urls.Select(async url =>
        {
            await semaphore.WaitAsync();
            string code;

            try
            {
                using var response = await HttpClient.GetAsync(url);
                code = ((int)response.StatusCode).ToString();
            }
            catch (Exception ex)
            {
                code = (ex.InnerException ?? ex).GetType().Name;
            }
            finally
            {
                semaphore.Release();
            }

            Console.WriteLine($"{url}: {code}");
        });

        await Task.WhenAll(tasks);
        Console.WriteLine($"Finished: {DateTime.Now - start}");
    }
}