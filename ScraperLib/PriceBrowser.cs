using Nito.AsyncEx;
using PuppeteerSharp;

namespace ScraperLib
{
    public sealed class PriceBrowser : IPriceBrowser
    {
        private readonly LaunchOptions ops = new() { Headless = true };
        private readonly AsyncLazy<IBrowser> _browser;
        public PriceBrowser()
        {
            _browser = new AsyncLazy<IBrowser>(async () =>
            {
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                return await Puppeteer.LaunchAsync(ops);
            });
        }

        public async Task<IPage> CreatePageAsync()
        {
            var browser = await _browser;
            return await browser.NewPageAsync();
        }
    }
}
