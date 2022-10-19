using System.Globalization;
using PuppeteerSharp;
using Nito.AsyncEx;

namespace ScraperLib
{
    internal struct PageData
    {
        public string[] tableHead;
        public string[] tableBody;
    }

    internal sealed class PricePage
    {
        private readonly AsyncLazy<IPage> _page;
        private readonly SemaphoreSlim _pageSemaphore = new(1, 1);
        public PricePage(PriceBrowser browser)
        {
            _page = new AsyncLazy<IPage>(async () =>
            {
                return await browser.CreatePageAsync("https://www.nordpoolgroup.com/en/Market-data1/Dayahead/Area-Prices/LT/Hourly/?view=table", "#datatable");
            });
        }

        /* FIXME: If request is for already set date, then this will timeout. */
        private async Task<bool> SetPageDate(DateTime date)
        {
            var page = await _page;
            var dateVal = date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            await page.EvaluateExpressionAsync("document.querySelector('#data-end-date').removeAttribute('readonly');");
            await page.EvaluateExpressionAsync($"document.querySelector('#data-end-date').value = '{dateVal}';");
            await page.EvaluateExpressionAsync("document.querySelector('#data-end-date').dispatchEvent(new Event('change'));");
            try
            {
                await page.WaitForSelectorAsync("#dashboard-column > div > div.dashboard-box > div.dashboard-tabs.chart-size.dashboard-indent > div.dashboard-tab.dashboard-tab-table.market-data-table > div.data-loading-message.ng-scope");
                await page.WaitForSelectorAsync("#dashboard-column > div > div.dashboard-box > div.dashboard-tabs.chart-size.dashboard-indent > div.dashboard-tab.dashboard-tab-table.market-data-table > div.dashboard-unit-update > div.dashboard-table-unit.ng-binding");
                await page.WaitForNetworkIdleAsync();
                return true;
            }
            catch { return false;  }
        }

        public async Task<PageData> GetPageDataAsync(DateTime date)
        {
            using var sem = await _pageSemaphore.LockAsync();

            var page = await _page;
            Console.WriteLine($"Started getting {date:yyyy-MM-dd}");
            await SetPageDate(date);

            var jsSelectDates = @"Array.from(document.querySelector('#datatable').querySelectorAll('table thead tr th')).filter(th => !th.classList.contains('row-name')).map(th => th.innerText);";
            var jsSelectPrices = @"Array.from(document.querySelector('#datatable').querySelectorAll('table tbody tr td')).filter(td => !td.classList.contains('row-name')).map(td => td.innerText);";

            PageData ret;
            ret.tableHead = await page.EvaluateExpressionAsync<string[]>(jsSelectDates);
            ret.tableBody = await page.EvaluateExpressionAsync<string[]>(jsSelectPrices);
            Console.WriteLine($"Finished getting getting {date:yyyy-MM-dd}");
            return ret;
        }
    }

}
