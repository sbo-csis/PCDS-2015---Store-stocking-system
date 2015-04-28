using NUnit.Framework;
using StoreStockingSystem.Data;

namespace StoreStockingSystem.Test
{
    [TestFixture]
    [Ignore]
    public class Spikes
    {
        [Test]
        [Ignore]
        public void InsertSeedData()
        {
            TestData.BuildData();
        }
    }
}