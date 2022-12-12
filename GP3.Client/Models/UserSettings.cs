namespace GP3.Client.Models;
public class UserSettings
{
    public UserSettings()
    {
        /* Set defaults here */
        lowPriceMark = 250;
        priceChangeNotf = false;
        lowestPriceNotf = false;
    }

    public double lowPriceMark { get; set; }
    public bool priceChangeNotf { get; set; }
    public bool lowestPriceNotf { get; set; }
    public String[] locations { get; set; }
    public String userLocation { get; set; }

}