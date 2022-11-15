using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GP3.Common.Entities
{
    public static class DateTimeExtension
    {
        public static long DaysSinceUnixEpoch(this DateTime date)
            => (long)(date.Date - DateTime.UnixEpoch).TotalDays;

        public static DateTime FromUnixEpoch(long days)
            => DateTime.UnixEpoch.AddDays(days).Date;
    }

    /*
     * TODO: Overhaul for compatibility with different timezones.
     * Should save time in UTC in DB with price
     * Then, App can query for specific hour interval converted to UTC
     * Any non-available hours can be displayed in-app as empty graph
     */
    public class DayPrice : IComparable<DayPrice>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DaysSinceUnixEpoch { get; }
        public double[] HourlyPrices { get; }
        public DateTime Date => DateTimeExtension.FromUnixEpoch(DaysSinceUnixEpoch);

        [JsonConstructor]
        public DayPrice(long daysSinceUnixEpoch, double[] hourlyPrices)
        {
            if (hourlyPrices == null || hourlyPrices.Length != 24)
            {
                throw new ArgumentException("Prices should have be non-null and have a length of 24!");
            }

            DaysSinceUnixEpoch = daysSinceUnixEpoch;
            HourlyPrices = hourlyPrices.ToArray();
        }

        public DayPrice(DateTime date, double[] prices) : this(date.DaysSinceUnixEpoch(), prices) { }

        public int CompareTo(DayPrice? other)
        {
            if (other == null)
            {
                return 1;
            }

            if (DaysSinceUnixEpoch < other.DaysSinceUnixEpoch)
            {
                return -1;
            }
            else if (DaysSinceUnixEpoch > other.DaysSinceUnixEpoch)
            {
                return 1;
            }

            return 0;
        }

        public bool HasSamePrices(DayPrice other)
            => HourlyPrices.SequenceEqual(other.HourlyPrices);
    }
}