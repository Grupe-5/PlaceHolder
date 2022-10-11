using System;
using System.Net;
using System.Reflection.Metadata;
using System.Linq;
using ScraperLib;
using Common;

namespace ScraperClient
{
    internal class Program
    {
        static async Task Main(string[] _)
        {
            IFetcher fetcher = new PriceFetcher();
            await fetcher.InitAsync();

            Console.WriteLine("Enter start date (yyyy-MM-dd): ");
            var start = Console.ReadLine();
            var startDate = DateTime.Parse(start);

            Console.WriteLine("Enter end date (yyyy-MM-dd): ");
            var end = Console.ReadLine();
            var endDate = DateTime.Parse(end);

            var prices = await fetcher.GetDayPricesAsync(startDate, endDate);
            foreach(var price in prices)
            {
                Console.WriteLine($"{price.Date:yyyy-MM-dd} {string.Join(", ", price.HourlyPrices)}");
            }

            await fetcher.DeinitAsync();
        }
    }
}