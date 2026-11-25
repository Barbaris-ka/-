using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    private static readonly Random random = new Random();

    private static readonly string[] statuses = {"Processing","Shipped","Delivered"};

    static async Task<string> CheckOrderStatusAsync(int orderId)
    {

        int delay = random.Next(300, 1001);
        await Task.Delay(delay);
        string status = statuses[random.Next(statuses.Length)];
        return $"Order {orderId}: {status}";
    }

    static async Task Main(string[] args)
    {
        int[] orderIds = { 101, 102, 103 };

        foreach (int id in orderIds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await CheckOrderStatusAsync(id);
            stopwatch.Stop();
            Console.WriteLine(
                $"{result} Время запроса: {stopwatch.ElapsedMilliseconds} мс"
            );
        }

        Console.WriteLine("Всё проверено");
    }
}
