using Microsoft.EntityFrameworkCore;

using DBT = GP3.Common.DB.IntegrationDbContext;
namespace GP3.Tests.Common.DB
{
    public class TestableIntegrationDb : DBT
    {
        public TestableIntegrationDb(DbContextOptions<DBT> options) : base(options)
        {
        }

        public void TestModelCreation(ModelBuilder model)
        {
            OnModelCreating(model);
        }
    }

    public class IntegrationDbContextTest
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
            var db = new TestableIntegrationDb(options);
            db.TestModelCreation(mb);
        }
    }
}
