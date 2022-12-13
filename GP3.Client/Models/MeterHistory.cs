
using GP3.Common.Entities;

namespace GP3.Client.Models
{
    public class MeterHistory
    {
        public MeterHistory(double currentDraw, double dayUsed, double dayEstCost, double monthUsed, double monthEstCost)
        {
            this.currentDraw = currentDraw;
            this.dayUsed = dayUsed;
            this.dayEstCost = dayEstCost;
            this.monthUsed = monthUsed;
            this.monthEstCost = monthEstCost;
        }
        public ProviderSelection electricityProvider { get; set; }
        public string apiToken { get; set; }
        public double currentDraw { get; set; }
        public double dayUsed { get; set; }
        public double dayEstCost { get; set; }
        
        public double monthUsed { get; set; }
        public double monthEstCost { get; set; }
    }


    public enum ElectricityProviders
    {
        Eso,
        Ignitis,
        Perlas
    }
}
