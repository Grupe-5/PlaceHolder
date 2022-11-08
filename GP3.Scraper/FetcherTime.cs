using GP3.Common.Entities;

namespace GP3.Scraper
{
    public static class FetcherTime
    {
        public const int NewDataHour = 13;
        public static readonly DateTime MinDate = DateTime.ParseExact("2021-01-01", "yyyy-MM-dd", null);
        public static DateTime UTCtoCEST(this DateTime utcTime)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
        }

        public static DateTime CESTtoUTC(this DateTime cestTime)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(cestTime, DateTimeKind.Unspecified), tzi);
        }

        public static DateTime LowestPriceTimeUTC(this DayPrice price)
        {
            var hour = Array.IndexOf(price.HourlyPrices, price.HourlyPrices.Min());
            return price.Date.Date.AddHours(hour).CESTtoUTC();
        }

        public static DateTime HighestPriceTimeUTC(this DayPrice price)
        {
            var hour = Array.IndexOf(price.HourlyPrices, price.HourlyPrices.Max());
            return price.Date.Date.AddHours(hour).CESTtoUTC();
        }

        public static bool IsTommorowDataAvailable()
            => DateTime.UtcNow.UTCtoCEST().Hour >= NewDataHour;
    }
}
