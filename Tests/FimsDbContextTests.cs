using NUnit.Framework;
using randomfilm_backend.Models;

namespace randomfilm_backend.Tests
{
    [TestFixture]
    public class FimsDbContextTests
    {
        /// <summary>
        /// Тест-кейс проверяющий проходит ли подключение к БД
        /// </summary>
        [Test]
        public void TestDBConnection()
        {
            RandomFilmDBContext db = new RandomFilmDBContext();
            if (db.Database.CanConnect() || db.Roles.Local.Count > 0)
                Assert.Pass();
            else
                Assert.Fail();
        }
    }
}
