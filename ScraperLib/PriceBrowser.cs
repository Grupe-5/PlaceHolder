using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PuppeteerSharp;

namespace ScraperLib
{
    public sealed class PriceBrowser : IPriceBrowser, IAsyncDisposable, IDisposable
    {
        private readonly AsyncLazy<IBrowser> _browser;
        private readonly ILogger<PriceBrowser> _logger;

        public PriceBrowser(ILogger<PriceBrowser> logger)
        {
            _logger = logger;
            _browser = new AsyncLazy<IBrowser>(async () =>
            {
                BrowserFetcherOptions opts = new()
                {
                    Path = Path.GetTempPath()
                };
                var fetcher = Puppeteer.CreateBrowserFetcher(opts);
                fetcher.DownloadProgressChanged += Fetcher_DownloadProgressChanged;
                var revInfo = await fetcher.DownloadAsync();
                LaunchOptions launchOptions = new()
                {
                    Headless = true,
                    ExecutablePath = fetcher.GetExecutablePath(revInfo.Revision),
                };
                return await Puppeteer.LaunchAsync(launchOptions);
            });
        }

        private int lastPercentage = -10;
        private void Fetcher_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= lastPercentage + 10)
            {
                lastPercentage = e.ProgressPercentage;
                _logger.LogInformation($"Browser download: {e.ProgressPercentage}% ({e.BytesReceived / 1000}kB / {e.TotalBytesToReceive / 1000}kB)");
            }
        }

        public async Task<IPage> CreatePageAsync()
        {
            var browser = await _browser;
            return await browser.NewPageAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_browser.IsStarted)
            {
                var browser = await _browser;
                await browser.DisposeAsync();
            }
        }

        public void Dispose()
        {
            if (_browser.IsStarted)
            {
                _browser.Task.Wait();
                var browser = _browser.Task.Result;
                browser.Dispose();
            }
        }

    }
}
