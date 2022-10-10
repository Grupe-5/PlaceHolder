using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IFetcher
    {
        DayPrices? GetDayPrices(DateTime date);
        IList<DayPrices> GetDayPrices(DateTime begin, DateTime end);
    }
}
