using System.Globalization;
using PuppeteerSharp;
using Nito.AsyncEx;

namespace ScraperLib
{
    public sealed class PricePage : IPricePage
    {
        private readonly AsyncLazy<IPage> _page;
        public PricePage(IPriceBrowser browser)
        {
            _page = new AsyncLazy<IPage>(async () =>
            {
                var page = await browser.CreatePageAsync();
                await page.GoToAsync("https://www.nordpoolgroup.com/en/Market-data1/Dayahead/Area-Prices/LT/Hourly/?view=table");
                await page.WaitForSelectorAsync("#datatable");
                return page;
            });
        }

        /* FIXME: If second request is for same date, then this will timeout. */
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
            catch { return false; }
        }

        public async Task<PageData> GetPageDataAsync(DateTime date)
        {
            var page = await _page;
            await SetPageDate(date);

            var jsSelectDates = @"Array.from(document.querySelector('#datatable').querySelectorAll('table thead tr th')).filter(th => !th.classList.contains('row-name')).map(th => th.innerText);";
            var jsSelectPrices = @"Array.from(document.querySelector('#datatable').querySelectorAll('table tbody tr td')).filter(td => !td.classList.contains('row-name')).map(td => td.innerText);";

            PageData ret;
            ret.tableHead = await page.EvaluateExpressionAsync<string[]>(jsSelectDates);
            ret.tableBody = await page.EvaluateExpressionAsync<string[]>(jsSelectPrices);
            return ret;
        }
    }

}
