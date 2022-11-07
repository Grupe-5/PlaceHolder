using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UserSettings
{
    public UserSettings()
    {
        
    }

    public Boolean priceChangeNotf { get; set; }
    public Boolean lowestPriceNotf { get; set; }
    public String[] locations { get; set; }
    public String userLocation { get; set; }

}