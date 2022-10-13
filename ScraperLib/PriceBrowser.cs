using Nito.AsyncEx;
using PuppeteerSharp;

namespace ScraperLib
{
    /* TODO: Implement OnProcessExit */
    sealed internal class PriceBrowser
    {
        private PriceBrowser() { }

        /* TODO: How to use dependency injection here? */
        private static readonly AsyncLazy<IBrowser> _browser = new AsyncLazy<IBrowser> (async () =>
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            return await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
        });

        public static async Task<IPage> CreatePageAsync(String url, String waitSelector)
        {
            var browser = await _browser;
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync(waitSelector);
            return page;
        }
    }
}
