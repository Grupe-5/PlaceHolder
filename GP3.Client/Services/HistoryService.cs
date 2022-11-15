using GP3.Client.Refit;
using GP3.Client.Models;

namespace GP3.Client.Services
{
    public class HistoryService
    {
        public HistoryService()
        {
        }

        List<MonthReading> monthReadingsList = new List<MonthReading>();
        public async Task<List<MonthReading>> GetReadings()
        {
            /* TODO: API doesn't have such endpoint yet
            var readings = await _readingApi.GetReadingsAsync();
            */

            if (monthReadingsList?.Count > 0)
                return monthReadingsList;

            MonthReading monthReadingItem;
            for (int i = 0; i < 5; i++)
            {
                monthReadingItem = new MonthReading
                {
                    PayedAmount = 420.5 + i,
                    UsedKwh = 69,
                    Month = Month.March
                };
                monthReadingsList.Add(monthReadingItem);
            }
            // GET API
            return monthReadingsList;
        }

        public async Task<bool> DeleteReading(MonthReading monthReading)
        {
            monthReadingsList.Remove(monthReading);
            // DELETE API
            return true;
        }

        public async Task<bool> AddMonth(MonthReading monthReading)
        {
            monthReadingsList.Add(monthReading);
            // PUT API
            return true;
        }
    }
}
