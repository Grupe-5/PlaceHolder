using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;
using Nito.AsyncEx;

namespace ScraperLib
{

    /* TODO: Implement IAsyncDisposable */
    internal sealed class PricePage
    {
        private readonly AsyncLazy<IPage> _page = new AsyncLazy<IPage> (async () =>
        {
            return await PriceBrowser.CreatePageAsync("https://www.nordpoolgroup.com/en/Market-data1/Dayahead/Area-Prices/LT/Hourly/?view=table", "#datatable");
        });

        public struct PageData
        {
            public String[] tableHead;
            public String[] tableBody;
        }

        public async Task SetPageDate(DateTime date)
        {
            var page = await _page;
            String dateVal = date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            await page.EvaluateExpressionAsync("document.querySelector('#data-end-date').removeAttribute('readonly');");
            await page.EvaluateExpressionAsync($"document.querySelector('#data-end-date').value = '{dateVal}';");
            await page.EvaluateExpressionAsync("document.querySelector('#data-end-date').dispatchEvent(new Event('change'));");
            await page.WaitForNetworkIdleAsync();
        }

        public async Task<PageData> GetPageDataAsync()
        {
            var page = await _page;
            PageData ret;
            var jsSelectDates = @"Array.from(document.querySelector('#datatable').querySelectorAll('table thead tr th')).filter(th => !th.classList.contains('row-name')).map(th => th.innerText);";
            ret.tableHead = await page.EvaluateExpressionAsync<string[]>(jsSelectDates);

            var jsSelectPrices = @"Array.from(document.querySelector('#datatable').querySelectorAll('table tbody tr td')).filter(td => !td.classList.contains('row-name')).map(td => td.innerText);";
            ret.tableBody = await page.EvaluateExpressionAsync<string[]>(jsSelectPrices);
            return ret;
        }
    }

}
