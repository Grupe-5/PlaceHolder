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
            Assert.Pass();
        }

        [Test]
        public void DaysSinceUnixEpochTest()
        {
            var start = DateTime.UnixEpoch;
            Assert.That(start.DaysSinceUnixEpoch(), Is.EqualTo(0));
            Assert.Pass();
        }
    }
}
