using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PuppeteerSharp;

namespace GP3.Scraper
{
    public sealed class PriceBrowser : IPriceBrowser, IAsyncDisposable, IDisposable
    {
        private readonly AsyncLazy<IBrowser> _browser;
        private readonly ILogger<PriceBrowser> _logger;

        public PriceBrowser(ILogger<PriceBrowser> logger, BrowserFetcherOptions? opts = null)
        {
            _logger = logger;
            _browser = new AsyncLazy<IBrowser>(async () =>
            {
                var fetcher = CreateFetcher(opts);
                fetcher.DownloadProgressChanged += Fetcher_DownloadProgressChanged;
                if (!fetcher.LocalRevisions().Any())
                {
                    _logger.LogInformation("Downloading chromium");
                    await fetcher.DownloadAsync();
                }
                else
                {
                    _logger.LogInformation("Using already available chromium");
                }

                if (!fetcher.LocalRevisions().Any())
                {
                    throw new FileNotFoundException("Failed to download browser!");
                }

                var revInfo = fetcher.LocalRevisions().FirstOrDefault();
                LaunchOptions launchOptions = new()
                {
                    Headless = true,
                    ExecutablePath = fetcher.GetExecutablePath(revInfo)
                };
                return await Puppeteer.LaunchAsync(launchOptions);
            });
        }

        public static IBrowserFetcher CreateFetcher(BrowserFetcherOptions? opts = null)
        {
            IBrowserFetcher fetcher;
            if (opts == null)
            {
                fetcher = new BrowserFetcher();
            }
            else
            {
                fetcher = Puppeteer.CreateBrowserFetcher(opts);
            }
            return fetcher;
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
