using GP3.Scraper;
using Microsoft.Extensions.Logging.Abstractions;
using PuppeteerSharp;

namespace GP3.Tests.Scraper
{
    public class PriceBrowserTest
    {
        private static IPriceBrowser CreateBrowser()
        {
            return new PriceBrowser(NullLogger<PriceBrowser>.Instance, null);
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task TestCreatePage()
        {
            var browser = CreateBrowser();
            var pricePage = await browser.CreatePageAsync();
            Assert.NotNull(pricePage);
        }

        [Test]
        public async Task TestDisposeAsync()
        {
            var browser = (PriceBrowser)CreateBrowser();
            var _ = await browser.CreatePageAsync();
            await browser.DisposeAsync();
        }

        [Test]
        public async Task TestDispose()
        {
            var browser = (PriceBrowser)CreateBrowser();
            var _ = await browser.CreatePageAsync();
            browser.Dispose();
        }

        [Test]
        public void TestCreateFetcher()
        {
            BrowserFetcherOptions options = new BrowserFetcherOptions();
            IBrowserFetcher x = PriceBrowser.CreateFetcher(options);
        }
    }
}
