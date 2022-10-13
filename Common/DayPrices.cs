using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Common
{
    /* TODO: Either DaysSinceUnixEpoch or DayPrices Date get property is returning off by one! (For now, incrementing DaysSince fixes it)*/
    public static class DateTimeExtension
    {
        public static long DaysSinceUnixEpoch(this DateTime date)
        {
            return (long)(date.Date - DateTimeOffset.UnixEpoch).TotalDays + 1;
        }
    }

    public class DayPrices : IComparable<DayPrices>
    {
        [Required]
        public long DaysSinceUnixEpoch { get; }
        [Required]
        public Double[] HourlyPrices { get; }
        public DateTime Date { get => DateTimeOffset.UnixEpoch.AddDays(DaysSinceUnixEpoch).Date; }  


        [JsonConstructor]
        public DayPrices(long daysSinceUnixEpoch, double[] hourlyPrices)
        {
            if (hourlyPrices == null || hourlyPrices.Length != 24)
            {
                throw new ArgumentException("Prices should have be non-null and have a length of 24!");
            }

            DaysSinceUnixEpoch = daysSinceUnixEpoch;
            HourlyPrices = hourlyPrices;
        }

        public DayPrices(DateTime date, Double[] prices) : this(date.DaysSinceUnixEpoch(), prices) {}

        /* Compares DayPrice dates */
        public int CompareTo(DayPrices? other)
        {
            if (other == null)
            {
                return 1;
            }

            if (this.DaysSinceUnixEpoch < other.DaysSinceUnixEpoch)
            {
                return -1;
            }
            else if (this.DaysSinceUnixEpoch > other.DaysSinceUnixEpoch)
            {
                return 1;
            }

            return 0;
        }

        /* Returns true if the prices between two objects are the same */
        public bool HasSamePrices(DayPrices other)
        {
            return this.HourlyPrices.SequenceEqual(other.HourlyPrices);
        }
    }
}