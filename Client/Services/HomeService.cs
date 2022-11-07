
namespace Client.Services
{
    public  class HomeService
    {
        List<Double> DayPrices = new();


        // TO DO
        // Async Task<List<Monkey>>
        public List<Double> GetPrices()
        {
            return DayPrices;
        }
    }
}
