using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using ScraperLib;

namespace ScrapeFunc.Prefetch
{
    public class FetcherWrapper
    {
        private readonly IBrowserFetcher _browserFetcher;
        public FetcherWrapper(string path)
        {
            BrowserFetcherOptions opts = new BrowserFetcherOptions
            {
                Path = path
            };
            _browserFetcher = PriceBrowser.CreateFetcher(opts);
            _browserFetcher.DownloadProgressChanged += Fetcher_DownloadProgressChanged;
        }

        public async Task DownloadAsync()
        {
            await _browserFetcher.DownloadAsync();
        }

        private int lastPercentage = -10;
        private void Fetcher_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= lastPercentage + 10)
            {
                lastPercentage = e.ProgressPercentage;
                Console.WriteLine($"\rBrowser download: {e.ProgressPercentage}% ({e.BytesReceived / 1000}kB / {e.TotalBytesToReceive / 1000}kB)");
            }
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Should be one argument!");
                return;
            }

            Console.WriteLine($"Prefetching browser to {args[0]} directory");
            var fetcher = new FetcherWrapper(args[0]);
            await fetcher.DownloadAsync();
        }
    }
}