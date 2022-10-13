using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IFetcher
    {
        Task<DayPrices?> GetDayPricesAsync(DateTime date);
        Task<IList<DayPrices>> GetDayPricesAsync(DateTime begin, DateTime end);
    }
}
