using Nito.AsyncEx;
using PuppeteerSharp;

namespace ScraperLib
{
    public sealed class PriceBrowser : IPriceBrowser
    {
        private readonly AsyncLazy<IBrowser> _browser;
        public PriceBrowser()
        {
            _browser = new AsyncLazy<IBrowser>(async () =>
            {
                var bfConf = new BrowserFetcherOptions
                {
                    Path = Path.GetTempPath()
                };
                var fetcher = Puppeteer.CreateBrowserFetcher(bfConf);
                var revInfo = await fetcher.DownloadAsync();
                LaunchOptions launchOptions = new()
                {
                    Headless = true,
                    ExecutablePath = fetcher.GetExecutablePath(revInfo.Revision),
                };
                return await Puppeteer.LaunchAsync(launchOptions);
            });
        }

        public async Task<IPage> CreatePageAsync()
        {
            var browser = await _browser;
            return await browser.NewPageAsync();
        }
    }
}
