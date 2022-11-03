using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PuppeteerSharp;
using System.Globalization;

namespace GP3.Scraper
{
    public sealed class PricePage : IPricePage
    {
        private readonly AsyncLazy<IPage> _page;
        private readonly ILogger<PricePage> _logger;
        public PricePage(IPriceBrowser browser, ILogger<PricePage> logger)
        {
            _logger = logger;
            _page = new AsyncLazy<IPage>(async () =>
            {
                var page = await browser.CreatePageAsync();
                await page.GoToAsync("https://www.nordpoolgroup.com/en/Market-data1/Dayahead/Area-Prices/LT/Hourly/?view=table");
                await page.WaitForSelectorAsync("#datatable");
                return page;
            });
        }

        private async Task<DateTime?> GetPageDateAsync()
        {
            var page = await _page;
            if (!DateTime.TryParse((await page.EvaluateExpressionAsync($"document.querySelector('#data-end-date').value;")).ToString(), out var pageDate))
            {
                _logger.LogError("Failed to parse page date!");
                return null;
            }

            return pageDate;
        }

        private async Task<bool> SetPageDateAsync(DateTime date)
        {
            var currDate = await GetPageDateAsync();
            if (currDate == null)
            {
                return false;
            }

            if (date.Date == currDate.Value.Date)
            {
                _logger.LogInformation("Page already has desired date");
                return true;
            }


            _logger.LogInformation($"Setting page date to {date:yyyy-MM-dd}");
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
            }
            catch {}


            var pageDate = await GetPageDateAsync();
            if (pageDate == null)
            {
                return false;
            }

            _logger.LogInformation($"Page date is: {pageDate:yyyy-MM-dd}");
            return pageDate.Value.Date == date.Date;
        }

        public async Task<PageData> GetPageDataAsync(DateTime date)
        {
            var page = await _page;
            if (!await SetPageDateAsync(date))
            {
                _logger.LogError("Failed to set page date!");
            }

            var jsSelectDates = @"Array.from(document.querySelector('#datatable').querySelectorAll('table thead tr th')).filter(th => !th.classList.contains('row-name')).map(th => th.innerText);";
            var jsSelectPrices = @"Array.from(document.querySelector('#datatable').querySelectorAll('table tbody tr td')).filter(td => !td.classList.contains('row-name')).map(td => td.innerText);";

            PageData ret;
            ret.tableHead = await page.EvaluateExpressionAsync<string[]>(jsSelectDates);
            ret.tableBody = await page.EvaluateExpressionAsync<string[]>(jsSelectPrices);
            return ret;
        }
    }

}
