using System;
using System.Net;
using System.Reflection.Metadata;
using System.Linq;
using ScraperLib;
using Common;

namespace ScraperClient
{
    /* 4) 6) 8-widening 9-queries */
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IFetcher fetcher = new PriceFetcher();

            Console.WriteLine("Enter start date (yyyy-mm-dd): ");
            var line = Console.ReadLine();

            var date = DateTime.Parse(line);
            for (int i = 0; i < 10; i++)
            {
                var priceRes = fetcher.GetDayPrices(date.AddDays(-1 * i));
                if (priceRes != null)
                {
                    var price = priceRes.GetValueOrDefault();
                    Console.WriteLine($"{price.Date.ToString("yyyy-MM-dd")} {string.Join(", ", price.HourlyPrices)}");
                }
                else
                {
                    Console.WriteLine("Failed to get data for that date");
                }
            }
        }
    }
}