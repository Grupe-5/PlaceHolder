namespace GP3.Client.Models
{
    public class HourPriceFormated
    {
        public double priceNumber { get; }
        public int intHour { get;  }

        public HourPriceFormated(double price, int hourInt)
        {
            priceNumber = price;
            intHour = hourInt;
        }
    }
}
