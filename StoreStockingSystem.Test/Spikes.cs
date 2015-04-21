using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public void InsertSeedData() // Runs for a long time (1+ min) if sales data is large (1000+ sales)
        {
            TestData.BuildData();
        }
    }
}