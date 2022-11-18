using GP3.Common.Entities;
using GP3.Common.Extensions;
using GP3.Common.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GP3.Tests.Common.Repositories
{
    public class CachedDayPriceRepositoryTest
    {
        private Mock<IDayPriceRepository> repoMock = new();
        private Mock<IDistributedCache> cacheMock = new();
        private CachedDayPriceRepository cachedRepo;

        private DayPrice GetDayPrice(DateTime date)
        {
            var prices = Enumerable.Range(0, 24).Select(i => 0.0).ToArray();
            return new DayPrice(date, prices);
        }

        private DayPrice GetDayPrice()
        {
            var prices = Enumerable.Range(0, 24).Select(i => 0.0).ToArray();
            var date = new DateTime(2022, 01, 01);
            return new DayPrice(date, prices);
        }

        [SetUp]
        public void Setup()
        {
            cachedRepo = new CachedDayPriceRepository(repoMock.Object, cacheMock.Object, NullLogger<CachedDayPriceRepository>.Instance);
            repoMock.Reset();
            cacheMock.Reset();
        }

        [Test]
        public void GetBetweenNoCacheDateTime()
        {
            var startDate = new DateTime(2022, 01, 01);
            var endDate = new DateTime(2022, 01, 02);
            cachedRepo.GetBetween(startDate, endDate);
            cacheMock.VerifyNoOtherCalls();
            repoMock.Verify(l => l.GetBetween(startDate, endDate), Times.Once);
        }

        [Test]
        public void GetBetweenNoCacheUnixTime()
        {
            var startDate = new DateTime(2022, 01, 01).DaysSinceUnixEpoch();
            var endDate = new DateTime(2022, 01, 02).DaysSinceUnixEpoch();
            cachedRepo.GetBetween(startDate, endDate);
            cacheMock.VerifyNoOtherCalls();
            repoMock.Verify(l => l.GetBetween(startDate, endDate), Times.Once);
        }

        [Test]
        public async Task AddAsyncCache()
        {
            var price = GetDayPrice();

            await cachedRepo.AddAsync(price);
            cacheMock.Verify(c => c.SetAsync(price.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddAsync(price), Times.Once);
        }

        [Test]
        public async Task AddAsyncCacheException()
        {
            var price = GetDayPrice();
            cacheMock.Setup(c => c.SetAsync(price.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("test exception"));

            await cachedRepo.AddAsync(price);
            cacheMock.Verify(c => c.SetAsync(price.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddAsync(price), Times.Once);
        }

        [Test]
        public async Task AddMultipleAsync()
        {
            var price1 = GetDayPrice(new DateTime(2022, 01, 01));
            var price2 = GetDayPrice(new DateTime(2022, 01, 02));

            cacheMock.Setup(c => c.SetAsync(price1.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
            cacheMock.Setup(c => c.SetAsync(price2.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));

            var lst = new List<DayPrice>();
            lst.Add(price1);
            lst.Add(price2);
            await cachedRepo.AddMultipleAsync(lst);
            cacheMock.Verify(c => c.SetAsync(price1.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            cacheMock.Verify(c => c.SetAsync(price1.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddMultipleAsync(lst), Times.Once);
        }

        [Test]
        public async Task AddMultipleAsyncException()
        {
            var price1 = GetDayPrice(new DateTime(2022, 01, 01));
            var price2 = GetDayPrice(new DateTime(2022, 01, 02));

            cacheMock.Setup(c => c.SetAsync(price1.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("test exception1"));
            cacheMock.Setup(c => c.SetAsync(price2.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("test exception2"));

            var lst = new List<DayPrice>();
            lst.Add(price1);
            lst.Add(price2);
            await cachedRepo.AddMultipleAsync(lst);
            cacheMock.Verify(c => c.SetAsync(price1.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            cacheMock.Verify(c => c.SetAsync(price1.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddMultipleAsync(lst), Times.Once);
        }

        [Test]
        public async Task GetDayPriceAsyncNonExistant()
        {
            var price = GetDayPrice();

            cacheMock.Setup(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default))
                .ReturnsAsync((byte[])null);
            repoMock.Setup(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch))
                .ReturnsAsync((DayPrice?)null);

            var val = await cachedRepo.GetDayPriceAsync(price.Date);
            cacheMock.Verify(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default), Times.Once);
            repoMock.Verify(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch), Times.Once);
            Assert.That(val, Is.Null);
        }

        [Test]
        public async Task GetDayPriceAsyncNotInCache()
        {
            var price = GetDayPrice();
            cacheMock.Setup(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default))
                .ReturnsAsync((byte[])null);
            repoMock.Setup(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch))
                .ReturnsAsync(price);

            var val = await cachedRepo.GetDayPriceAsync(price.Date);
            cacheMock.Verify(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default), Times.Once);
            cacheMock.Verify(c => c.SetAsync(price.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch), Times.Once);
            Assert.That(val, Is.EqualTo(price));
        }

        [Test]
        public async Task GetDayPriceAsyncNotInCacheException()
        {
            var price = GetDayPrice();
            cacheMock.Setup(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default))
                .ReturnsAsync((byte[])null);
            repoMock.Setup(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch))
                .ReturnsAsync(price);
            cacheMock.Setup(c => c.SetAsync(price.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("test exception2"));

            var val = await cachedRepo.GetDayPriceAsync(price.Date);
            cacheMock.Verify(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default), Times.Once);
            cacheMock.Verify(c => c.SetAsync(price.DaysSinceUnixEpoch.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch), Times.Once);
            Assert.That(val, Is.EqualTo(price));
        }

        [Test]
        public async Task GetDayPriceAsyncInCache()
        {
            var price = GetDayPrice();
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(price, new JsonSerializerOptions()));
            cacheMock.Setup(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default))
                .ReturnsAsync(bytes);

            var val = await cachedRepo.GetDayPriceAsync(price.Date);
            cacheMock.Verify(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default), Times.Once);
            cacheMock.Verify(c => c.RefreshAsync(price.DaysSinceUnixEpoch.ToString(), default), Times.Once);
            Assert.That(val, Is.Not.Null);
        }

        [Test]
        public async Task GetDayPriceAsyncInCacheException()
        {
            var price = GetDayPrice();
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(price, new JsonSerializerOptions()));
            cacheMock.Setup(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default))
                .ThrowsAsync(new Exception("test exception"));
            repoMock.Setup(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch))
                .ReturnsAsync(price);

            var val = await cachedRepo.GetDayPriceAsync(price.Date);
            cacheMock.Verify(c => c.GetAsync(price.DaysSinceUnixEpoch.ToString(), default), Times.Once);
            repoMock.Verify(r => r.GetDayPriceAsync(price.DaysSinceUnixEpoch), Times.Once);
            Assert.That(val, Is.EqualTo(price));
        }
    }
}
