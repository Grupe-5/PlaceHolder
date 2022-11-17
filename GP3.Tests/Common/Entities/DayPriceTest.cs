using GP3.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Tests.Common.Entities
{
    public class DayPriceTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void FromUnixEpochTest()
        {
            var days = DateTime.Today.DaysSinceUnixEpoch();
            Assert.Multiple(() =>
            {
                Assert.That(DateTimeExtension.FromUnixEpoch(0), Is.EqualTo(DateTime.UnixEpoch));
                Assert.That(DateTimeExtension.FromUnixEpoch(days), Is.EqualTo(DateTime.Today));
            });
        }

        [Test]
        public void DaysSinceUnixEpochTest()
        {
            var start = DateTime.UnixEpoch;
            Assert.Multiple(() =>
            {
                Assert.That(start.DaysSinceUnixEpoch(), Is.EqualTo(0));
                Assert.That(DateTime.Today.DaysSinceUnixEpoch(), Is.EqualTo((DateTime.Today - DateTime.UnixEpoch).TotalDays));
            });
        }

        [Test]
        public void BothConstructorsEqual()
        {
            var randPrices = Enumerable.Range(0, 24).Select(i => Random.Shared.NextDouble()).ToArray();
            var pricesDate = new DayPrice(DateTime.Today, randPrices);
            var pricesLong = new DayPrice(DateTime.Today.DaysSinceUnixEpoch(), randPrices);

            Assert.Multiple(() =>
            {
                Assert.That(pricesDate.CompareTo(pricesLong), Is.EqualTo(0));
                Assert.That(pricesDate.HasSamePrices(pricesLong), Is.True);
                Assert.That(pricesDate.HourlyPrices, Is.EqualTo(pricesLong.HourlyPrices));
                Assert.That(pricesDate.DaysSinceUnixEpoch, Is.EqualTo(pricesLong.DaysSinceUnixEpoch));
                Assert.That(pricesDate.Date, Is.EqualTo(pricesLong.Date));
            });
        }

        [Test]
        public void CompareToTest()
        {
            var randPrices = Enumerable.Range(0, 24).Select(i => Random.Shared.NextDouble()).ToArray();
            var pricesDate = new DayPrice(DateTime.Today, randPrices);
            var yesterdayDate = new DayPrice(DateTime.Today.AddDays(-1), randPrices);
            var tommorowDate = new DayPrice(DateTime.Today.AddDays(1), randPrices);
            Assert.Multiple(() =>
            {
                Assert.That(pricesDate.CompareTo(null), Is.EqualTo(1));
                Assert.That(pricesDate.CompareTo(yesterdayDate), Is.EqualTo(1));
                Assert.That(pricesDate.CompareTo(tommorowDate), Is.EqualTo(-1));
                Assert.That(pricesDate.CompareTo(pricesDate), Is.EqualTo(0));
            });
        }

        [Test]
        public void PricesNullException()
        {
            var randPrices = Enumerable.Range(0, 10).Select(i => Random.Shared.NextDouble()).ToArray();
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new DayPrice(DateTime.Today, randPrices));
                Assert.Throws<ArgumentException>(() => new DayPrice(DateTime.Today, null));
            });
        }
    }
}
