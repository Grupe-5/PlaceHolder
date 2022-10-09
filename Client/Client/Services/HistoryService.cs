using Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                monthReadingItem.PayedAmount = 420.5 + i;
                monthReadingItem.UsedKwh = 69;
                monthReadingItem.Month = MonthReading.Months.March.ToString();
                monthReadingsList.Add(monthReadingItem);
            }
            return monthReadingsList;
        }
    }
}
