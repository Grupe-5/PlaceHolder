using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common
{
    public static class DateTimeExtension
    {
        public static long DaysSinceUnixEpoch(this DateTime date)
        {
            return (long)(date.Date - DateTime.UnixEpoch).TotalDays;
        }

        public static DateTime FromUnixEpoch(long days)
        {
            return DateTime.UnixEpoch.AddDays(days).Date;
        }
    }

    public class DayPrices : IComparable<DayPrices>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DaysSinceUnixEpoch { get; }
        public double[] HourlyPrices { get; }
        public DateTime Date => DateTimeExtension.FromUnixEpoch(DaysSinceUnixEpoch);

        [JsonConstructor]
        public DayPrices(long daysSinceUnixEpoch, double[] hourlyPrices)
        {
            if (hourlyPrices == null || hourlyPrices.Length != 24)
            {
                throw new ArgumentException("Prices should have be non-null and have a length of 24!");
            }

            DaysSinceUnixEpoch = daysSinceUnixEpoch;
            HourlyPrices = hourlyPrices.ToArray();
        }

        public DayPrices(DateTime date, double[] prices) : this(date.DaysSinceUnixEpoch(), prices) {}

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

        public bool HasSamePrices(DayPrices other)
        {
            return this.HourlyPrices.SequenceEqual(other.HourlyPrices);
        }
    }
}