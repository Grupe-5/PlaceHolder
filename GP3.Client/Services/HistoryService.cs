using Client.Model;

namespace Client.Services
{
    public class HistoryService
    {
        List<MonthReading> monthReadingsList = new List<MonthReading>();
        public async Task<List<MonthReading>> GetReadings()
        {

            if (monthReadingsList?.Count > 0)
                return monthReadingsList;

            MonthReading monthReadingItem;
            for (int i = 0; i < 5; i++)
            {
                monthReadingItem = new MonthReading();
                monthReadingItem.payedAmount = 420.5 + i;
                monthReadingItem.usedKwh = 69;
                monthReadingItem.month = MonthReading.Months.March.ToString();
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
            Console.WriteLine(monthReading.month);
            return true;
        }


    }
}
