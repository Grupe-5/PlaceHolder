using GP3.Common.Entities;
using GP3.Scraper;

namespace GP3.Tests.Scraper
{
    public class FetcherTimeTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ConversionBetweenUTCandCEST()
        {
            var utcDate1 = new DateTime(2022, 08, 01, 0, 0, 0, DateTimeKind.Utc);
            var cestDate1 = utcDate1.AddHours(2);
            var utcDate2 = new DateTime(2022, 11, 10, 0, 0, 0, DateTimeKind.Utc);
            var cestDate2 = utcDate2.AddHours(1);
            Assert.Multiple(() =>
            {
                Assert.That(cestDate1, Is.EqualTo(utcDate1.UTCtoCEST()));
                Assert.That(cestDate2, Is.EqualTo(utcDate2.UTCtoCEST()));

                Assert.That(utcDate1, Is.EqualTo(cestDate1.CESTtoUTC()));
                Assert.That(utcDate2, Is.EqualTo(cestDate2.CESTtoUTC()));
            });
        }

        [Test]
        public void TestGetLowestPrice()
        {
            var cestDate = new DateTime(2022, 08, 01, 0, 0, 0, DateTimeKind.Unspecified);
            var values = Enumerable.Range(0, 24).Select(i => Random.Shared.NextDouble()).ToArray();
            var dayPrice = new DayPrice(cestDate, values);
            Assert.Multiple(() =>
            {
                var index = (int)(dayPrice.LowestPriceTimeUTC() - cestDate.CESTtoUTC()).TotalHours;
                Assert.That(values.Min(i => i), Is.EqualTo(values[index]));
            });
        }

        [Test]
        public void TestGetHighestPrice()
        {
            var cestDate = new DateTime(2022, 08, 01, 0, 0, 0, DateTimeKind.Unspecified);
            var values = Enumerable.Range(0, 24).Select(i => Random.Shared.NextDouble()).ToArray();
            var dayPrice = new DayPrice(cestDate, values);
            Assert.Multiple(() =>
            {
                var index = (int)(dayPrice.HighestPriceTimeUTC() - cestDate.CESTtoUTC()).TotalHours;
                Assert.That(values.Max(i => i), Is.EqualTo(values[index]));
            });
        }
    }
}
