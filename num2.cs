using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static readonly Random random = new Random();
    private static readonly object randomLock = new object();
    private static readonly string[] statuses =
    {"Processing","Shipped","Delivered"};

    static async Task<string> CheckOrderStatusAsync(int orderId)
    {
        int delay;
        string status;
        lock (randomLock)
        {
            delay = random.Next(300, 1001);
            status = statuses[random.Next(statuses.Length)];
        }

        await Task.Delay(delay);

        return $"Order {orderId}: {status}";
    }

    static async Task<string> CheckOrderWithSemaphoreAsync(
        int orderId,
        SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();

        try
        {
            return await CheckOrderStatusAsync(orderId);
        }
        finally
        {
            semaphore.Release();
        }
    }

    static async Task Main(string[] args)
    {
        int[] orderIds = Enumerable.Range(1, 10).ToArray();
        using SemaphoreSlim semaphore = new SemaphoreSlim(3);

        Stopwatch totalStopwatch = Stopwatch.StartNew();
        Task<string>[] tasks = orderIds
            .Select(id => CheckOrderWithSemaphoreAsync(id, semaphore))
            .ToArray();

        int completedCount = 0;
        var remainingTasks = tasks.ToList();

        while (remainingTasks.Count > 0)
        {
            Task<string> completedTask = await Task.WhenAny(remainingTasks);
            string result = await completedTask;
            completedCount++;
            Console.WriteLine(
                $"{result} Выполнено: {completedCount}/10"
            );
            remainingTasks.Remove(completedTask);
        }
        await Task.WhenAll(tasks);

        totalStopwatch.Stop();

        Console.WriteLine("\nВсе запросы завершены!");
        Console.WriteLine($"Всего выполнено запросов: {completedCount}");
        Console.WriteLine(
            $"Общее время выполнения: {totalStopwatch.ElapsedMilliseconds} мс"
        );
    }
}
