
namespace Client.Model;

public class MonthReading
{
    public enum Months
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
    

    public String Month { get; set; }
    public double PayedAmount { get; set; }
    public int UsedKwh { get; set; }

}

