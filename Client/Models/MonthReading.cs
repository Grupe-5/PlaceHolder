
namespace Client.Model;

public class MonthReading
{
    public MonthReading()
    {

    }
    public MonthReading(string month, double payedAmount, int usedKwh)
    {
        this.month = month;
        this.payedAmount = payedAmount;
        this.usedKwh = usedKwh;
    }
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
    

    public String month { get; set; }
    public double payedAmount { get; set; }
    public int usedKwh { get; set; }

}

