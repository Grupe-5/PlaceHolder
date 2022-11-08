using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GP3.Client.Models
{
    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    };
    public class MonthReading
    {
        public MonthReading() {}
        public MonthReading(Month month, double payedAmount, int usedKwh)
        {
            Month = month;
            PayedAmount = payedAmount;
            UsedKwh = usedKwh;
        }
        public Month Month { get; set; }
        public double PayedAmount { get; set; }
        public int UsedKwh { get; set; }
    }
}
