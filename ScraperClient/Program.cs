using ScraperLib;
using Common;
using System.Globalization;

namespace ScraperClient
{
    public static class StringExtension
    {
        public static DateTime? IsValidDate(this string str, string format)
        {
            if (!DateTime.TryParseExact(str, format, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date))
            {
                return null;
            }

            return date;
        }
    }

    internal class Program
    {
        static DateTime ReadDate(string message)
        {
            DateTime? startDate = null;
            while (!startDate.HasValue)
            {
                Console.WriteLine(message);
                var text = Console.ReadLine();
                if (text != null)
                {
                    startDate = text.IsValidDate("yyyy-MM-dd");
                }
            }

            return startDate.Value;
        }

        static async Task Main(string[] _)
        {
            IFetcher fetcher = new PriceFetcher();
            var startDate = ReadDate("Enter start date (yyyy-MM-dd): ");
            var endDate = ReadDate("Enter end date (yyyy-MM-dd): ");

            var prices = await fetcher.GetDayPricesAsync(startDate, endDate);
            foreach(var price in prices)
            {
                Console.WriteLine($"{price.Date:yyyy-MM-dd} {string.Join(", ", price.HourlyPrices)}");
            }
        }
    }
}