using Microsoft.EntityFrameworkCore;

using DBT = GP3.Common.DB.HistoryRegistrationDbContext;
namespace GP3.Tests.Common.DB
{
    public class TestableHistoryDb : DBT
    {
        public TestableHistoryDb(DbContextOptions<DBT> options) : base(options)
        {
        }

        public void TestModelCreation(ModelBuilder model)
        {
            OnModelCreating(model);
        }
    }

    public class HistoryRegistrationDbContextTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void OnModelCreatingTest()
        {
            var mb = new ModelBuilder();
            DbContextOptions<DBT> options = new();
            var db = new TestableHistoryDb(options);
            db.TestModelCreation(mb);
        }
    }
}
