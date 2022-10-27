using Common;
using System.Globalization;
using Nito.AsyncEx;

namespace ScraperLib
{
    static class StringExtension
    {
        public static double ParseDoubleFallback(this string str, double fallback)
        {
            double.TryParse(str.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out fallback);
            return fallback;
        }
    }

    public class PriceFetcher : IFetcher
    {
        private readonly IPricePage _page;
        public PriceFetcher(IPricePage page)
        {
            _page = page;
        }

        private static DayPrices ParseSingleDay(PageData data, int dayOffset)
        {
            var dayDate = DateTime.ParseExact(data.tableHead[dayOffset], "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var priceArr = Enumerable
                .Range(0, 24)
                .Select(y => data.tableBody[y * data.tableHead.Length + dayOffset].ParseDoubleFallback(0.0));
            return new DayPrices(dayDate, priceArr.ToArray());
        }

        public async Task<IEnumerable<DayPrices>> GetWeekPricesAsync(DateTime date)
        {
            var data = await _page.GetPageDataAsync(date);
            return Enumerable
                .Range(0, data.tableHead.Length)
                .Select(i => ParseSingleDay(data, i));
        }
    }
}
