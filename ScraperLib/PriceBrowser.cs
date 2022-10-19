using Nito.AsyncEx;
using PuppeteerSharp;

namespace ScraperLib
{
    sealed internal class PriceBrowser : IAsyncDisposable
    {
        private readonly AsyncLazy<IBrowser> _browser;
        public PriceBrowser()
        {
            _browser = new AsyncLazy<IBrowser>(async () =>
            {
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                return await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false
                });
            });
        }

        public async ValueTask DisposeAsync()
        {
            var browser = await _browser;
            await browser.CloseAsync();
        }

        public async Task<IPage> CreatePageAsync(string url, string waitSelector)
        {
            var browser = await _browser;
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync(waitSelector);
            return page;
        }
    }
}
