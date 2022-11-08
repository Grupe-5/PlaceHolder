using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Client.Models
{
    public class HourPriceFormated
    {
        public double priceNumber { get; }
        public string priceHour { get; }

        public HourPriceFormated(double price, int hourInt)
        {
            priceHour = FormatHour(hourInt) + " - " + FormatHour(hourInt + 1);
            priceNumber = price;
        }

        private string FormatHour(int hourInt)
        {
            string hour;
            if (hourInt > 9)
            {
                hour = hourInt.ToString();
            }
            else
            {
                hour = "0" + hourInt.ToString();
            }
            return hour;
        }
    }
}
