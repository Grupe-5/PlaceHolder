using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UserSettings
{
    public UserSettings()
    {
        lowPriceMark = 250;
    }

    public double lowPriceMark { get; set; }
    public bool priceChangeNotf { get; set; }
    public bool lowestPriceNotf { get; set; }
    public String[] locations { get; set; }
    public String userLocation { get; set; }

}