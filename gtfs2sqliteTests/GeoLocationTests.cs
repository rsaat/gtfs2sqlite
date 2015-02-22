using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Gtfs2Sqlite.Util;

namespace gtfs2sqliteTests
{
    [TestFixture]
    public class GeoLocationTests
    {

        [Test]
        public void DistanceTest()
        {
            var g = new Geolocation();
            var d = g.Distance(-23.4416351318359, -46.6167221069336, -23.4945163726807, -46.5690116882324);
            Assert.That(d, Is.EqualTo(7.632627056843751).Within(0.01));
        }

    }
}
