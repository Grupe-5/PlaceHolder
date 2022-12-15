using GP3.Scraper;
using Microsoft.Extensions.Logging.Abstractions;

namespace GP3.Tests.Scraper
{
    public class PricePageTest
    {
        static readonly PriceBrowser browser = new PriceBrowser(NullLogger<PriceBrowser>.Instance, null);

        public static IPricePage CreatePricePage()
        {
            return new PricePage(browser, NullLogger<PricePage>.Instance);
        }

        private static bool isValid(PageData page)
        {
            return page.tableHead.Length == 8 && page.tableBody.Length == 248;
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task TestGetTodayPrice()
        {
            var page = CreatePricePage();
            var data = await page.GetPageDataAsync(DateTime.Now);
            Assert.That(isValid(data));
        }

        [Test]
        public async Task TestSameDay()
        {
            var page = CreatePricePage();
            var data = await page.GetPageDataAsync(DateTime.Now);
            var data2 = await page.GetPageDataAsync(DateTime.Now);
            Assert.Multiple(() =>
            {
                Assert.That(isValid(data));
                Assert.That(isValid(data2));
                Assert.That(data.tableHead, Is.EqualTo(data2.tableHead));
                Assert.That(data.tableBody, Is.EqualTo(data2.tableBody));
            });
        }

        [Test]
        public async Task TestInvalidDateTime()
        {
            var page = CreatePricePage();
            var data = await page.GetPageDataAsync(DateTime.Now.AddDays(30));
            Assert.That(!isValid(data));
        }
    }
}
