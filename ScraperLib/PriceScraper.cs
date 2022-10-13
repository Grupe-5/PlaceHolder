using System.Globalization;
using Common;
using Nito.AsyncEx;

namespace ScraperLib
{
    internal sealed class PriceScraper
    {
        private readonly Lazy<PricePage> page = new Lazy<PricePage> (() => new PricePage ());

        /* Returns 7 (or less) days of data, starting from date and moving backwards */
        async private Task<IList<DayPrices>> FetchWeekPrices(DateTime date)
        {
            await page.Value.SetPageDate(date);
            PricePage.PageData data = await page.Value.GetPageDataAsync();

            DayPrices[] prices = new DayPrices[data.tableHead.Length];
            for (int i = 0; i < data.tableHead.Length; i++)
            {
                var dayDate = DateTime.ParseExact(data.tableHead[i], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var priceArr = new Double[24];
                for (int y = 0; y < priceArr.Length; y++)
                {
                    var str = data.tableBody[y * data.tableHead.Length + i];
                    Double val = 0;
                    try
                    {
                        val = Double.Parse(str.Replace(',', '.'), CultureInfo.InvariantCulture);
                    }
                    catch { }
                    priceArr[y] = val;
                }

                prices[i] = new DayPrices(dayDate, priceArr);
            }

            return prices;
        }

        /* Note: This could return more entries than are expected. */
        async public Task<IEnumerable<DayPrices>> FetchPrices(DateTime begin, DateTime? endOpt = null)
        {
            /* If the param wasn't passed then end is null, which means we only want care about begin */
            begin = begin.Date;
            DateTime end = endOpt ?? begin.Date;

            if (end < begin)
            {
                return Array.Empty<DayPrices>();
            }

            int dayCount = (int)((end - begin).TotalDays) + 1;
            List<DayPrices> priceList = new List<DayPrices>(dayCount);

            int queryCount = (int) Math.Ceiling(dayCount / 8.0);
            for (int i = 0; i < queryCount; i++)
            {
                IList<DayPrices> currPrices = await FetchWeekPrices(end.AddDays(-8 * i));
                priceList.AddRange(currPrices);
            }

            /* Remove duplicate dates */
            return priceList.GroupBy(x => x.DaysSinceUnixEpoch).Select(x => x.First());
            /* Or, in query syntax: */
            /*
            return from x in (from x in priceList
                   group x by x.Date)
                   select x.First();
            */
        }
    }

}