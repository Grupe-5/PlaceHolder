using GP3.Common.DB;
using Microsoft.EntityFrameworkCore;

namespace GP3.Tests.Common.DB
{
    public class TestableDayPriceDb : DayPriceDbContext
    {
        public TestableDayPriceDb(DbContextOptions<DayPriceDbContext> options) : base(options)
        {
        }

        public void TestModelCreation(ModelBuilder model)
        {
            OnModelCreating(model);
        }
    }

    public class DayPriceDbContextTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void OnModelCreatingTest()
        {
            var mb = new ModelBuilder();
            DbContextOptions<DayPriceDbContext> options = new();
            var db = new TestableDayPriceDb(options);
            db.TestModelCreation(mb);
        }
    }
}
