using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace ScraperLib
{
    internal sealed class PricePage
    {
        private IBrowser browser;
        private IPage page;
        public struct PageData
        {
            public String[] tableHead;
            public String[] tableBody;
        }

        private PricePage() {}

        public async Task CloseAsync()
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
        private async Task<PricePage> InitializeAsync(String url, String waitSelector)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync(waitSelector);
            return this;
        }

        public static async Task<PricePage> CreateAsync()
        {
            var ret = new PricePage();
            return await ret.InitializeAsync("https://www.nordpoolgroup.com/en/Market-data1/Dayahead/Area-Prices/LT/Hourly/?view=table", "#datatable");
        }

        public async Task SetPageDate(DateTime date)
        {
            String dateVal = date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            await page.EvaluateExpressionAsync("document.querySelector('#data-end-date').removeAttribute('readonly');");
            await page.EvaluateExpressionAsync($"document.querySelector('#data-end-date').value = '{dateVal}';");
            await page.EvaluateExpressionAsync("document.querySelector('#data-end-date').dispatchEvent(new Event('change'));");
            await page.WaitForNetworkIdleAsync();
        }

        public async Task<PageData> GetPageDataAsync()
        {
            PageData ret = new PageData();
            var jsSelectDates = @"Array.from(document.querySelector('#datatable').querySelectorAll('table thead tr th')).filter(th => !th.classList.contains('row-name')).map(th => th.innerText);";
            ret.tableHead = await page.EvaluateExpressionAsync<string[]>(jsSelectDates);

            var jsSelectPrices = @"Array.from(document.querySelector('#datatable').querySelectorAll('table tbody tr td')).filter(td => !td.classList.contains('row-name')).map(td => td.innerText);";
            ret.tableBody = await page.EvaluateExpressionAsync<string[]>(jsSelectPrices);
            return ret;
        }
    }

}
