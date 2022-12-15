using GP3.Common.Entities;
using GP3.Common.Repositories;
using GP3.Funcs.Services;
using GP3.Funcs.Services.HistoryProviders;
using Microsoft.Extensions.Configuration.UserSecrets;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Tests.Funcs.Services
{
    public class ProviderServiceTest
    {
        private const string userId = "abc";
        private readonly ProviderFactory factory = new(new(), new(), new());
        private readonly Mock<IHistoryRegistrationRepository> repoMock = new();
        private ProviderService service;

        private void MockRegistration(ProviderSelection select, bool isNull = false)
        {
            repoMock.Setup(c => c.GetRegistration(userId)).ReturnsAsync(isNull ? null: new HistoryRegistration(userId, "tok", select));
        }

        [SetUp]
        public void Setup()
        {
            service = new ProviderService(factory, repoMock.Object);
            repoMock.Reset();
        }

        [Test]
        public async Task TestGetCurrentDraw()
        {
            foreach(ProviderSelection x in Enum.GetValues(typeof(ProviderSelection)))
            {
                MockRegistration(x);
                var draw = await service.GetCurrentDraw(userId);
                Assert.NotNull(draw);
                MockRegistration(x, true);
                var nullDraw = await service.GetCurrentDraw(userId);
                Assert.IsNull(nullDraw);
            }
        }

        [Test]
        public async Task TestMonthlyUsage()
        {
            foreach(ProviderSelection x in Enum.GetValues(typeof(ProviderSelection)))
            {
                MockRegistration(x);
                var draw = await service.GetMonthyUsage(userId);
                Assert.NotNull(draw);
                MockRegistration(x, true);
                var nullDraw = await service.GetMonthyUsage(userId);
                Assert.IsNull(nullDraw);
            }
        }

        [Test]
        public async Task TestDailyUsage()
        {
            foreach(ProviderSelection x in Enum.GetValues(typeof(ProviderSelection)))
            {
                MockRegistration(x);
                var draw = await service.GetDailyUsage(userId);
                Assert.NotNull(draw);
                MockRegistration(x, true);
                var nullDraw = await service.GetDailyUsage(userId);
                Assert.IsNull(nullDraw);
            }
        }

        [Test]
        public async Task TestIsRegistered()
        {
            repoMock.Setup(c => c.UserIsRegistered(userId)).ReturnsAsync(true);
            Assert.That(await service.IsRegistered(userId), Is.True);
            repoMock.Setup(c => c.UserIsRegistered(userId)).ReturnsAsync(false);
            Assert.That(await service.IsRegistered(userId), Is.False);
        }

        [Test]
        public async Task TestIsValidToken()
        {
            foreach(ProviderSelection x in Enum.GetValues(typeof(ProviderSelection)))
            {
                Assert.IsTrue(await service.IsValidToken("a", x));
            }
        }

        [Test]
        public async Task TestRegisterProvider()
        {
            var tok = "a";
            foreach(ProviderSelection x in Enum.GetValues(typeof(ProviderSelection)))
            {
                repoMock.Setup(c => c.RegisterProvider(userId, tok, x));
                Assert.IsTrue(await service.RegisterProvider(userId, tok, x));
                repoMock.Setup(c => c.RegisterProvider(userId, tok, x)).Throws(new Exception(""));
                Assert.IsFalse(await service.RegisterProvider(userId, tok, x));
            }
        }

        [Test]
        public async Task TestUnregisterProvider()
        {
            foreach(ProviderSelection x in Enum.GetValues(typeof(ProviderSelection)))
            {
                repoMock.Setup(c => c.UnregisterProvider(userId));
                Assert.IsTrue(await service.UnregisterProvider(userId));
                repoMock.Setup(c => c.UnregisterProvider(userId)).Throws(new Exception(""));
                Assert.IsFalse(await service.UnregisterProvider(userId));
            }
        }
    }
}
